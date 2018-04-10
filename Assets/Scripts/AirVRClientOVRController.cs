/***********************************************************

  Copyright (c) 2017-2018 Clicked, Inc.

  Licensed under the MIT license found in the LICENSE file 
  in the root folder of the project.

 ***********************************************************/

using UnityEngine;

public class AirVRClientOVRController : MonoBehaviour
{
    public GameObject _model;

    public OVRInput.Controller _controller;

    private bool _prevControllerConnected = false;
    private bool _prevControllerConnectedCached = false;

    void Update()
    {                   
        bool controllerConnected = OVRInput.IsControllerConnected(_controller);

#if UNITY_ANDROID && !UNITY_EDITOR
        controllerConnected = controllerConnected && !AirVRClient.connected;
#endif

        if ((controllerConnected != _prevControllerConnected) || !_prevControllerConnectedCached)
        {
            _model.SetActive(controllerConnected);
            _prevControllerConnected = controllerConnected;
            _prevControllerConnectedCached = true;
        }

        if (!controllerConnected)
        {
            return;
        }
    }
    }