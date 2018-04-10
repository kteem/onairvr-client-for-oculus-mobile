/***********************************************************

  Copyright (c) 2017-2018 Clicked, Inc.

  Licensed under the MIT license found in the LICENSE file 
  in the Docs folder of the distributed package.

 ***********************************************************/

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

[RequireComponent(typeof(Camera))]

public class AirVRCamera : AirVRCameraBase {
    [DllImport(AirVRClient.LibPluginName)]
    private static extern void onairvr_InitJNI();

    private AirVRProfile _profile;

    protected override void Awake () {
        // Work around : Only the game module can access to java classes in onAirVR client plugin.
        if (Application.isEditor == false && Application.platform == RuntimePlatform.Android) {
            onairvr_InitJNI();
        }
        
        base.Awake();
        _profile = new AirVRProfile();
    }

    protected override void Start() {
        base.Start();

        AirVRInputManager.RegisterInputDevice(new AirVRTouchpadInputDevice());
        AirVRInputManager.RegisterInputDevice(new AirVRGamepadInputDevice());
        AirVRInputManager.RegisterInputDevice(new AirVRTrackedControllerInputDevice());

        gameObject.AddComponent<AirVRGazePointer>();
		gameObject.AddComponent<AirVRTrackedControllerPointer>().Configure(defaultTrackedControllerModel, true);
    }

    // implements AirVRCameraBase
    protected override AirVRProfileBase profile {
        get {
            return _profile;
        }
    }

    protected override void RecenterPose() {
        OVRManager.display.RecenterPose();
    }
}
