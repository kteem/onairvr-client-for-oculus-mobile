/***********************************************************

  Copyright (c) 2017-2018 Clicked, Inc.

  Licensed under the MIT license found in the LICENSE file 
  in the Docs folder of the distributed package.

 ***********************************************************/

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Assertions;
using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class AirVRClient : MonoBehaviour, AirVRClientStateMachine.Context {
	public interface EventHandler {
        void AirVRClientFailed(string reason);
		void AirVRClientConnected();
		void AirVRClientPlaybackStarted();
		void AirVRClientPlaybackStopped();
		void AirVRClientDisconnected();
        void AirVRClientUserDataReceived(byte[] userData);
	}

    public const string LibPluginName = "onAirVRClientPlugin";

	private static AirVRClient _instance;
    private static AirVRClientEventDispatcher _eventDispatcher;
    private static bool _checkCopyright;

	[DllImport(LibPluginName)]
	private static extern void onairvr_SetProfile(string profile);

	[DllImport(LibPluginName)]
	private static extern int onairvr_Init(string licenseFilePath, int audioOutputSampleRate, bool hasInput);

    [DllImport(LibPluginName)]
    private static extern void onairvr_RequestConnect(string address, int port);

	[DllImport(LibPluginName)]
	private static extern void onairvr_RequestPlay();

	[DllImport(LibPluginName)]
	private static extern void onairvr_RequestStop();

	[DllImport(LibPluginName)]
	private static extern void onairvr_RequestDisconnect();
	
	[DllImport(LibPluginName)]
	private static extern void onairvr_Cleanup();

	[DllImport(LibPluginName)]
	private static extern bool onairvr_IsConnected();

	[DllImport(LibPluginName)]
	private static extern bool onairvr_IsPlaying();

    [DllImport(LibPluginName)]
    private static extern IntPtr onairvr_PrepareRender_RenderThread_Func();

    [DllImport(LibPluginName)]
    private static extern bool onairvr_GetVideoRenderTargetTexture(ref System.IntPtr texture, ref int width, ref int height);

	[DllImport(LibPluginName)]
	private static extern void onairvr_EnableCopyrightCheck(bool enable);

	public delegate void OnAirVRMessageReceiveHandler(AirVRClientMessage messsage);
	public static event OnAirVRMessageReceiveHandler MessageReceived;

	public static EventHandler Delegate { private get; set; }

	public static bool connected {
		get { 
			return onairvr_IsConnected();
		}
	}

	public static bool playing {
		get { 
			return onairvr_IsPlaying();
		}
	}

    public static void LoadOnce(AirVRProfileBase profile, AirVRCameraBase camera) {
        if (_instance == null) {
            GameObject go = new GameObject("AirVRClient");
            go.AddComponent<AirVRClient>();

            Assert.IsTrue(_instance != null);
            _instance._profile = profile;
            if (profile.useSeperateVideoRenderTarget) {
                _instance._videoFrameRenderer = new AirVRVideoFrameRenderer(go, profile, camera);
            }

			onairvr_SetProfile(JsonUtility.ToJson(profile.GetSerializable()));
        }
    }

    public static void Connect(string address, int port, string userID = "") {
		if (_instance != null) {
			_instance._profile.userID = userID;
			onairvr_SetProfile(JsonUtility.ToJson(_instance._profile.GetSerializable()));
			
			onairvr_RequestConnect(address, port);
		}
    }

	public static void Play() {
		if (_instance != null) {
			_instance._stateMachine.TriggerPlayRequested();
		}
	}

	public static void Stop() {
		if (_instance != null) {
			_instance._stateMachine.TriggerStopRequested();
		}
	}

	public static void Disconnect() {
		onairvr_RequestDisconnect();
	}
		
    private AirVRProfileBase _profile;
    private AirVRVideoFrameRenderer _videoFrameRenderer;
	private AirVRClientStateMachine _stateMachine;
        
    private void Awake() {
		if (_instance != null) {
			throw new UnityException("[ERROR] There must exist only one instance of AirVRClient.");
		}
		_instance = this;
		GameObject.DontDestroyOnLoad(gameObject);

        if (_eventDispatcher == null) {
            _eventDispatcher = new AirVRClientEventDispatcher();
            _eventDispatcher.MessageReceived += onAirVRMessageReceived;
        }
    }

    private void Start() {
        if (Application.isEditor) {
            return;
        }

        if (Delegate == null) {
            throw new UnityException("[ERROR] Must set AirVRClient.Delegate.");
        }
        AirVRClientLicenseFile licenseFile = new AirVRClientLicenseFile("client.license");
        int result = onairvr_Init(licenseFile.path, AudioSettings.outputSampleRate, _profile.hasInput);
        if (result < 0 && result != -4) {
            Delegate.AirVRClientFailed("failed to init AirVRClient : " + result);
        }
        licenseFile.Release();
	    
	    _stateMachine = new AirVRClientStateMachine(this, _profile.delayToResumePlayback);
	}

    private void Update() {
        if (Application.isEditor) {
            return;
        }

        _eventDispatcher.DispatchEvent();
	    _stateMachine.Update(_profile.isUserPresent, Time.deltaTime);
    }

    private void LateUpdate() {
        if (_videoFrameRenderer != null) {
            _videoFrameRenderer.Update();
        }
    }

	private void OnApplicationPause(bool pauseStatus) {
		if (Application.isEditor) {
			return;
		}
		
		_stateMachine.UpdatePauseStatus(pauseStatus);
	}

	private void OnDestroy() {
		if (Application.isEditor) {
			return;
		}
		
        if (_eventDispatcher != null) {
            _eventDispatcher.MessageReceived -= onAirVRMessageReceived;
        }
		onairvr_Cleanup();
	}

	// handle AirVRMessages
	private void onAirVRMessageReceived(AirVRMessage message) {
        AirVRClientMessage clientMessage = message as AirVRClientMessage;
        Assert.IsNotNull(clientMessage);

		if (MessageReceived != null) {
            MessageReceived(clientMessage);
		}

        if (clientMessage.IsSessionEvent()) {
            if (clientMessage.Name.Equals(AirVRClientMessage.NameSetupResponded)) {
                onAirVRSetupResponded(clientMessage);
            }
            else if (clientMessage.Name.Equals(AirVRClientMessage.NameRenderPrepared)) {
                onAirVRRenderPrepared(clientMessage);
            }
            else if (clientMessage.Name.Equals(AirVRClientMessage.NamePlayResponded)) {
                onAirVRPlayResponded(clientMessage);
            }
            else if (clientMessage.Name.Equals(AirVRClientMessage.NameStopResponded)) {
                onAirVRStopResponded(clientMessage);
            }
            else if (clientMessage.Name.Equals(AirVRClientMessage.NameDisconnected)) {
                onAirVRDisconnected(clientMessage);
            }
        }
		else if (message.Type.Equals(AirVRMessage.TypeUserData)) {
			onAirVRUserDataReceived(message);
		}
	}
	
    private void onAirVRSetupResponded(AirVRClientMessage message) {
		GL.IssuePluginEvent(onairvr_PrepareRender_RenderThread_Func(), _profile.useSeperateVideoRenderTarget ? 1 : 0);
	}

    private void onAirVRRenderPrepared(AirVRClientMessage message) {
        if (_videoFrameRenderer != null) {
            System.IntPtr texture = System.IntPtr.Zero;
            int width = 0, height = 0;

            if (onairvr_GetVideoRenderTargetTexture(ref texture, ref width, ref height)) {
                _videoFrameRenderer.SetVideoFrameTexture(Texture2D.CreateExternalTexture(width, height, TextureFormat.RGBA32, false, false, texture));
                _videoFrameRenderer.enabled = true;
            }
        }
	    
	    _stateMachine.TriggerConnected();
	    Delegate.AirVRClientConnected();
	}

    private void onAirVRPlayResponded(AirVRClientMessage message) {
        Delegate.AirVRClientPlaybackStarted();
	}

    private void onAirVRStopResponded(AirVRClientMessage message) {
        Delegate.AirVRClientPlaybackStopped();
	}
	
    private void onAirVRDisconnected(AirVRClientMessage message) {
        if (_videoFrameRenderer != null) {
            _videoFrameRenderer.SetVideoFrameTexture(null);
            _videoFrameRenderer.enabled = false;
        }
	    
	    _stateMachine.TriggerDisconnected();
	    Delegate.AirVRClientDisconnected();
	}

    private void onAirVRUserDataReceived(AirVRMessage message) {
		Delegate.AirVRClientUserDataReceived(message.Data_Decoded);
    }
	
	// implements AirVRClientStateMachine.Context
	void AirVRClientStateMachine.Context.RequestPlay() {
		onairvr_RequestPlay();
	}

	void AirVRClientStateMachine.Context.RequestStop() {
		onairvr_RequestStop();
	}
}
