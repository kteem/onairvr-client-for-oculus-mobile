/***********************************************************

  Copyright (c) 2017-2018 Clicked, Inc.

  Licensed under the MIT license found in the LICENSE file 
  in the Docs folder of the distributed package.

 ***********************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal class AirVRGamepadInputDevice : AirVRInputDevice {

	// implements AirVRInputDevice
    protected override string deviceName {
        get {
            return AirVRInputDeviceName.Gamepad;
        }
    }

    protected override bool connected {
        get {
            return OVRInput.IsControllerConnected(OVRInput.Controller.Gamepad);
        }
    }

    protected override void PendInputs(AirVRInputStream inputStream) {
        inputStream.PendAxis2D(this, (byte)AirVRGamepadKey.Axis2DLThumbstick, OVRInput.Get(OVRInput.RawAxis2D.LThumbstick, OVRInput.Controller.Gamepad));
        inputStream.PendAxis2D(this, (byte)AirVRGamepadKey.Axis2DRThumbstick, OVRInput.Get(OVRInput.RawAxis2D.RThumbstick, OVRInput.Controller.Gamepad));
        inputStream.PendAxis(this, (byte)AirVRGamepadKey.AxisLIndexTrigger, OVRInput.Get(OVRInput.RawAxis1D.LIndexTrigger, OVRInput.Controller.Gamepad));
        inputStream.PendAxis(this, (byte)AirVRGamepadKey.AxisRIndexTrigger, OVRInput.Get(OVRInput.RawAxis1D.RIndexTrigger, OVRInput.Controller.Gamepad));
        inputStream.PendButton(this, (byte)AirVRGamepadKey.ButtonA, OVRInput.Get(OVRInput.RawButton.A, OVRInput.Controller.Gamepad));
        inputStream.PendButton(this, (byte)AirVRGamepadKey.ButtonB, OVRInput.Get(OVRInput.RawButton.B, OVRInput.Controller.Gamepad));
        inputStream.PendButton(this, (byte)AirVRGamepadKey.ButtonX, OVRInput.Get(OVRInput.RawButton.X, OVRInput.Controller.Gamepad));
        inputStream.PendButton(this, (byte)AirVRGamepadKey.ButtonY, OVRInput.Get(OVRInput.RawButton.Y, OVRInput.Controller.Gamepad));
        inputStream.PendButton(this, (byte)AirVRGamepadKey.ButtonStart, OVRInput.Get(OVRInput.RawButton.Start, OVRInput.Controller.Gamepad));
        
        // workaround : avoid bugs in OVRInput for important inputs
        inputStream.PendButton(this, (byte)AirVRGamepadKey.ButtonBack, OVRInput.Get(OVRInput.Button.Back));
        
        inputStream.PendButton(this, (byte)AirVRGamepadKey.ButtonLShoulder, OVRInput.Get(OVRInput.RawButton.LShoulder, OVRInput.Controller.Gamepad));
        inputStream.PendButton(this, (byte)AirVRGamepadKey.ButtonRShoulder, OVRInput.Get(OVRInput.RawButton.RShoulder, OVRInput.Controller.Gamepad));
        inputStream.PendButton(this, (byte)AirVRGamepadKey.ButtonLThumbstick, OVRInput.Get(OVRInput.RawButton.LThumbstick, OVRInput.Controller.Gamepad));
        inputStream.PendButton(this, (byte)AirVRGamepadKey.ButtonRThumbstick, OVRInput.Get(OVRInput.RawButton.RThumbstick, OVRInput.Controller.Gamepad));
        inputStream.PendButton(this, (byte)AirVRGamepadKey.ButtonDpadUp, OVRInput.Get(OVRInput.RawButton.DpadUp, OVRInput.Controller.Gamepad));
        inputStream.PendButton(this, (byte)AirVRGamepadKey.ButtonDpadDown, OVRInput.Get(OVRInput.RawButton.DpadDown, OVRInput.Controller.Gamepad));
        inputStream.PendButton(this, (byte)AirVRGamepadKey.ButtonDpadLeft, OVRInput.Get(OVRInput.RawButton.DpadLeft, OVRInput.Controller.Gamepad));
        inputStream.PendButton(this, (byte)AirVRGamepadKey.ButtonDpadRight, OVRInput.Get(OVRInput.RawButton.DpadRight, OVRInput.Controller.Gamepad));
    }
}
