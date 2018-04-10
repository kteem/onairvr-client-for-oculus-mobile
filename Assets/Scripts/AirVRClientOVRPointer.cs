/***********************************************************

  Copyright (c) 2017-2018 Clicked, Inc.

  Licensed under the MIT license found in the LICENSE file 
  in the root folder of the project.

 ***********************************************************/

using UnityEngine;
using UnityEngine.EventSystems;

public class AirVRClientOVRPointer : MonoBehaviour
{
    [Header("OVR Input Module")]
    [SerializeField] private OVRInputModule _inputModule;
    [SerializeField] private OVRGazePointer _gazePointer;

    [Header("OVR Rig Transforms")]
    [SerializeField] private Transform _trackingSpace = null;
    [SerializeField] private Transform _centerEyeAnchor = null;
    [SerializeField] private Transform _leftHandAnchor = null;
    [SerializeField] private Transform _rightHandAnchor = null;
    
    [Header("Selection Ray")]
    public bool ShowSelectionRay = true;
    [SerializeField] private LineRenderer _lineRenderer = null;
    [SerializeField] private float _maxRayLength = 0.5f;

    private Transform _rayTransform;

    public bool ControllerIsConnected
    {
        get
        {
            OVRInput.Controller controller = OVRInput.GetConnectedControllers() & (OVRInput.Controller.LTrackedRemote | OVRInput.Controller.RTrackedRemote);
            return controller == OVRInput.Controller.LTrackedRemote || controller == OVRInput.Controller.RTrackedRemote;
        }
    }

    public OVRInput.Controller Controller
    {
        get
        {
            OVRInput.Controller controller = OVRInput.GetConnectedControllers();
            if ((controller & OVRInput.Controller.LTrackedRemote) == OVRInput.Controller.LTrackedRemote)
            {
                return OVRInput.Controller.LTrackedRemote;
            }
            else if ((controller & OVRInput.Controller.RTrackedRemote) == OVRInput.Controller.RTrackedRemote)
            {
                return OVRInput.Controller.RTrackedRemote;
            }
            return OVRInput.GetActiveController();
        }
    }

    private void Awake()
    {
        _rayTransform = _centerEyeAnchor;       
    }

    private void Update()
    {
        Vector3 worldStartPoint = Vector3.zero;
        Vector3 worldEndPoint = Vector3.zero;

        if (_lineRenderer != null)
        {
            _lineRenderer.enabled = ControllerIsConnected && ShowSelectionRay && !AirVRClient.connected;
        }

        if (ControllerIsConnected && !AirVRClient.connected)
        {
            if (_rayTransform == _centerEyeAnchor)
            {
                if (Controller == OVRInput.Controller.LTrackedRemote)
                {
                    _rayTransform = _leftHandAnchor;
                }
                else if (Controller == OVRInput.Controller.RTrackedRemote)
                {
                    _rayTransform = _rightHandAnchor;
                }

                _gazePointer.rayTransform = _rayTransform;
                _inputModule.rayTransform = _rayTransform;
            }

            if (_trackingSpace != null)
            {
                Matrix4x4 localToWorld = _trackingSpace.localToWorldMatrix;
                Quaternion orientation = OVRInput.GetLocalControllerRotation(Controller);

                Vector3 localStartPoint = OVRInput.GetLocalControllerPosition(Controller);
                Vector3 localEndPoint = localStartPoint + orientation * Vector3.forward * _maxRayLength;

                worldStartPoint = localToWorld.MultiplyPoint(localStartPoint);
                worldEndPoint = localToWorld.MultiplyPoint(localEndPoint);
            }

            if (_lineRenderer != null)
            {
                _lineRenderer.SetPosition(0, worldStartPoint);
                _lineRenderer.SetPosition(1, worldEndPoint);
            }
        }
        else
        {
            if (_rayTransform != _centerEyeAnchor)
            {
                _rayTransform = _centerEyeAnchor;

                _gazePointer.rayTransform = _rayTransform;
                _inputModule.rayTransform = _rayTransform;
            }
        }
    }
}
