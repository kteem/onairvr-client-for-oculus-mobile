/***********************************************************

  Copyright (c) 2017-2018 Clicked, Inc.

  Licensed under the MIT license found in the LICENSE file 
  in the Docs folder of the distributed package.

 ***********************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal class AirVRTouchpadInputDevice : AirVRInputDevice {
	// implements AirVRInputDevice
    protected override string deviceName { 
        get {
            return AirVRInputDeviceName.Touchpad;
        }
    }

    protected override bool connected { 
        get {
			return OVRInput.IsControllerConnected(OVRInput.Controller.Touchpad);
        }
    }

    protected override void PendInputs(AirVRInputStream inputStream) {
        inputStream.PendTouch(this, (byte)AirVRTouchpadKey.Touchpad, OVRInput.Get(OVRInput.RawAxis2D.LTouchpad, OVRInput.Controller.Touchpad),
                                                                     OVRInput.Get(OVRInput.RawTouch.LTouchpad, OVRInput.Controller.Touchpad));
        
        // workaround : avoid bugs in OVRInput for important inputs
        inputStream.PendButton(this, (byte)AirVRTouchpadKey.ButtonBack, Input.GetKey(KeyCode.Escape));
        
        inputStream.PendButton(this, (byte)AirVRTouchpadKey.ButtonUp, OVRInput.Get(OVRInput.RawButton.DpadUp, OVRInput.Controller.Touchpad));
        inputStream.PendButton(this, (byte)AirVRTouchpadKey.ButtonDown, OVRInput.Get(OVRInput.RawButton.DpadDown, OVRInput.Controller.Touchpad));
        inputStream.PendButton(this, (byte)AirVRTouchpadKey.ButtonLeft, OVRInput.Get(OVRInput.RawButton.DpadLeft, OVRInput.Controller.Touchpad));
        inputStream.PendButton(this, (byte)AirVRTouchpadKey.ButtonRight, OVRInput.Get(OVRInput.RawButton.DpadRight, OVRInput.Controller.Touchpad));
    }
}
