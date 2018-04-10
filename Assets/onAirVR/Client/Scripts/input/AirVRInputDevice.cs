/***********************************************************

  Copyright (c) 2017-2018 Clicked, Inc.

  Licensed under the MIT license found in the LICENSE file 
  in the Docs folder of the distributed package.

 ***********************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AirVRInputDevice : AirVRInputSender {
    public void Update() {
        if (Application.isEditor) {
            return;
        }

        if (AirVRInputManager.CheckIfInputDeviceEnabled(this) != connected) {
            if (connected) {
                AirVRInputManager.EnableInputDevice(this);
            }
            else {
                AirVRInputManager.DisableInputDevice(this);
            }
        }
    }

    // abstract properties and methods
    protected abstract string deviceName { get; }
    protected abstract bool connected { get; }
    protected abstract void PendInputs(AirVRInputStream inputStream);

    // implements AirVRInputSender
    public override string name {
        get {
            return deviceName;
        }
    }

    public override void PendInputsPerFrame(AirVRInputStream inputStream) {
        PendInputs(inputStream);
    }
}
