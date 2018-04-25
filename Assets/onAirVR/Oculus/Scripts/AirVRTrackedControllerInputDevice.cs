/***********************************************************

  Copyright (c) 2017-2018 Clicked, Inc.

  Licensed under the MIT license found in the LICENSE file 
  in the Docs folder of the distributed package.

 ***********************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal class AirVRTrackedControllerInputDevice : AirVRInputDevice {
	// implements AirVRInputDevice
    protected override string deviceName {
        get {
            return AirVRInputDeviceName.TrackedController;
        }
    }

    protected override bool connected {
        get {
            return (OVRInput.GetConnectedControllers() & (OVRInput.Controller.LTrackedRemote | OVRInput.Controller.RTrackedRemote)) != 0;
        }
    }

    protected override void PendInputs(AirVRInputStream inputStream) {
        bool leftHanded = (OVRInput.GetConnectedControllers() & OVRInput.Controller.LTrackedRemote) != 0;
        OVRInput.Controller controller = OVRInput.Controller.LTrackedRemote | OVRInput.Controller.RTrackedRemote;
        inputStream.PendTransform(this, (byte)AirVRTrackedControllerKey.Transform,
                                  OVRInput.GetLocalControllerPosition(leftHanded ? OVRInput.Controller.LTrackedRemote : OVRInput.Controller.RTrackedRemote),
                                  OVRInput.GetLocalControllerRotation(leftHanded ? OVRInput.Controller.LTrackedRemote : OVRInput.Controller.RTrackedRemote));
        inputStream.PendTouch(this, (byte)AirVRTrackedControllerKey.Touchpad, 
                              OVRInput.Get(leftHanded ? OVRInput.RawAxis2D.LTouchpad : OVRInput.RawAxis2D.RTouchpad, controller),
                              OVRInput.Get(leftHanded ? OVRInput.RawTouch.LTouchpad : OVRInput.RawTouch.RTouchpad, controller));
        inputStream.PendButton(this, (byte)AirVRTrackedControllerKey.ButtonTouchpad,
                               OVRInput.Get(leftHanded ? OVRInput.RawButton.LTouchpad : OVRInput.RawButton.RTouchpad, controller));
        
        // workaround : avoid bugs in OVRInput for important inputs
        inputStream.PendButton(this, (byte)AirVRTrackedControllerKey.ButtonBack, OVRInput.Get(OVRInput.Button.Back));
        
        inputStream.PendButton(this, (byte)AirVRTrackedControllerKey.ButtonIndexTrigger,
                               OVRInput.Get(leftHanded ? OVRInput.RawButton.LIndexTrigger : OVRInput.RawButton.RIndexTrigger, controller));
        inputStream.PendButton(this, (byte)AirVRTrackedControllerKey.ButtonUp, OVRInput.Get(OVRInput.RawButton.DpadUp, controller));
        inputStream.PendButton(this, (byte)AirVRTrackedControllerKey.ButtonDown, OVRInput.Get(OVRInput.RawButton.DpadDown, controller));
        inputStream.PendButton(this, (byte)AirVRTrackedControllerKey.ButtonLeft, OVRInput.Get(OVRInput.RawButton.DpadLeft, controller));
        inputStream.PendButton(this, (byte)AirVRTrackedControllerKey.ButtonRight, OVRInput.Get(OVRInput.RawButton.DpadRight, controller));
    }
}
