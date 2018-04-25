/***********************************************************

  Copyright (c) 2017-2018 Clicked, Inc.

  Licensed under the MIT license found in the LICENSE file 
  in the Docs folder of the distributed package.

 ***********************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using System.Runtime.InteropServices;
using UnityEngine.Assertions;

public abstract class AirVRCameraBase : MonoBehaviour {
    private const uint RenderEventMaskClearColor    = 0x00800000U;

    private enum FrameType {
        StereoLeft  = 0,
        StereoRight,
        Mono
    }

    private abstract class RenderCommand {
        public static RenderCommand Create(AirVRProfileBase profile, Camera camera) {
            return profile.useSeperateVideoRenderTarget ? new RenderCommandImmediate() as RenderCommand : 
                                                          new RenderCommandOnCameraEvent(camera, CameraEvent.BeforeForwardOpaque) as RenderCommand;
        }

        public abstract void Issue(System.IntPtr renderFuncPtr, int arg);
        public abstract void Clear();
    }

    private class RenderCommandOnCameraEvent : RenderCommand {
        public RenderCommandOnCameraEvent(Camera camera, CameraEvent cameraEvent) {
            _commandBuffer = new CommandBuffer();
            camera.AddCommandBuffer(cameraEvent, _commandBuffer);
        }

        private CommandBuffer _commandBuffer;

        public override void Issue(System.IntPtr renderFuncPtr, int arg) {
            _commandBuffer.IssuePluginEvent(renderFuncPtr, arg);
        }

        public override void Clear() {
            _commandBuffer.Clear();
        }
    }

    private class RenderCommandImmediate : RenderCommand {
        public override void Issue(System.IntPtr renderFuncPtr, int arg) {
            GL.IssuePluginEvent(renderFuncPtr, arg);
        }

        public override void Clear() {}
    }

    private class HeadTrackerInputDevice : AirVRInputDevice {
        public HeadTrackerInputDevice(Transform cameraTransform) {
            _cameraTransform = cameraTransform;
        }

        private Transform _cameraTransform;

        // implements AirVRInputDevice
        protected override string deviceName {
            get {
                return AirVRInputDeviceName.HeadTracker;
            }
        }

        protected override bool connected { get { return true; } }

        protected override void PendInputs(AirVRInputStream inputStream) {
            inputStream.PendTransform(this, (byte)AirVRHeadTrackerKey.Transform, _cameraTransform.localPosition, _cameraTransform.localRotation);
        }
    }
    
    [DllImport(AirVRClient.LibPluginName)]
    private static extern void onairvr_EnableNetworkTimeWarp(bool enable);

    [DllImport(AirVRClient.LibPluginName)]
    private static extern void onairvr_SetCameraOrientation(float x, float y, float z, float w, ref int viewNumber);

    [DllImport(AirVRClient.LibPluginName)]
    private static extern System.IntPtr onairvr_PreRenderVideoFrame_RenderThread_Func();

    [DllImport(AirVRClient.LibPluginName)]
    private static extern System.IntPtr onairvr_RenderVideoFrame_RenderThread_Func();

    private HeadTrackerInputDevice _headTracker;
    private Transform _thisTransform;
    private Matrix4x4 _worldToLocalWithLocalRotationAsIdentity;
    private Camera _camera;
    private int _viewNumber;
    private RenderCommand _renderCommand;
    private bool _renderingRight;
    private bool _aboutToDestroy;
    private Vector2 _savedCameraClipPlanes;

    [SerializeField] private bool _enableAudio = true;
    [SerializeField] private AudioMixerGroup _audioMixerGroup;
    
    protected GameObject defaultTrackedControllerModel { get; private set; }

    private IEnumerator CallEndOfFrame() {
        while (_aboutToDestroy == false) {
            yield return null;

            _renderingRight = false;
        }
    }

    private int renderEvent(FrameType frameType, bool clearColor) {
        return (int)(((int)frameType << 24) + (clearColor ? RenderEventMaskClearColor : 0));
    }

    private void saveCameraClipPlanes() {
        _savedCameraClipPlanes.x = _camera.nearClipPlane;
        _savedCameraClipPlanes.y = _camera.farClipPlane;
    }

    private void restoreCameraClipPlanes() {
        _camera.nearClipPlane = _savedCameraClipPlanes.x;
        _camera.farClipPlane = _savedCameraClipPlanes.y;
    }

    private void setCameraClipPlanes(float nearClip, float farClip) {
        _camera.nearClipPlane = Mathf.Min(nearClip, _camera.nearClipPlane);
        _camera.farClipPlane = Mathf.Max(farClip, _camera.farClipPlane);
    }

    protected virtual void Awake() {
        _thisTransform = transform;
        _worldToLocalWithLocalRotationAsIdentity = Matrix4x4.identity;
        _worldToLocalWithLocalRotationAsIdentity.SetTRS(-_thisTransform.position, 
                                                        _thisTransform.parent != null ? Quaternion.Inverse(_thisTransform.parent.rotation) : Quaternion.identity, 
                                                        Vector3.one);
        _camera = gameObject.GetComponent<Camera>();

        if (_enableAudio) {
            GameObject go = new GameObject("AirVRAudioSource");
            go.transform.parent = transform;
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;

            AudioSource audioSource = go.AddComponent<AudioSource>();
            audioSource.loop = false;

            if (_audioMixerGroup != null) {
                audioSource.outputAudioMixerGroup = _audioMixerGroup;

                // workaround for applying audio mixer group change
                audioSource.Stop();
                audioSource.Play();
            }

            go.AddComponent<AirVRClientAudioSource>();
        }
        _headTracker = new HeadTrackerInputDevice(_thisTransform);
    }

    protected virtual void Start() {
        defaultTrackedControllerModel = Resources.Load<GameObject>("trackedControllerModel");
        
        _renderCommand = RenderCommand.Create(profile, _camera);

        AirVRClient.LoadOnce(profile, this);
        AirVRInputManager.LoadOnce();

		AirVRClient.MessageReceived += onAirVRMesageReceived;
        AirVRInputManager.RegisterInputDevice(_headTracker);

        StartCoroutine(CallEndOfFrame());

        saveCameraClipPlanes(); // workaround for the very first disconnected event
    }

    private void OnPreRender() {
        if (Application.isEditor) {
            return;
        }

		if (AirVRClient.playing) {
            if (_renderingRight == false) {
                onairvr_SetCameraOrientation(_thisTransform.localRotation.x, 
                                             _thisTransform.localRotation.y,
                                             _thisTransform.localRotation.z,
                                             _thisTransform.localRotation.w,
                                             ref _viewNumber);
                GL.IssuePluginEvent(onairvr_PreRenderVideoFrame_RenderThread_Func(), _viewNumber);
            }

            // clear color the texture only for the right eye when using single texture for the two eyes
            _renderCommand.Issue(onairvr_RenderVideoFrame_RenderThread_Func(),
                                 renderEvent(_renderingRight ? FrameType.StereoRight : FrameType.StereoLeft, 
                                             profile.useSingleTextureForEyes == false || _renderingRight == false));
        }

        _renderingRight = true;
    }

    private void OnPostRender() {
        _renderCommand.Clear();
    }

    private void OnDestroy() {
        _aboutToDestroy = true;

		AirVRClient.MessageReceived -= onAirVRMesageReceived;
    }

    protected abstract AirVRProfileBase profile { get; }
    protected abstract void RecenterPose();

    public Matrix4x4 worldToLocalMatrix {
        get {
            return _worldToLocalWithLocalRotationAsIdentity;
        }
    }

    // handle AirVRMessage
    private void onAirVRMesageReceived(AirVRClientMessage message) {
        if (message.IsSessionEvent()) {
            if (message.Name.Equals(AirVRClientMessage.NameConnected)) {
                saveCameraClipPlanes();
                
                onairvr_EnableNetworkTimeWarp(true);
            }
            else if (message.Name.Equals(AirVRClientMessage.NameDisconnected)) {
                restoreCameraClipPlanes();
            }
        }
        else if (message.IsMediaStreamEvent()) {
            if (message.Name.Equals(AirVRClientMessage.NameCameraClipPlanes)) {
                setCameraClipPlanes(message.NearClip, message.FarClip);
            }
            else if (message.Name.Equals(AirVRClientMessage.NameEnableNetworkTimeWarp)) {
                onairvr_EnableNetworkTimeWarp(message.Enable);
            }
        }
        else if (message.IsInputStreamEvent() && message.Name.Equals(AirVRClientMessage.NameRecenterPose)) {
            RecenterPose();
        }
	}
}
