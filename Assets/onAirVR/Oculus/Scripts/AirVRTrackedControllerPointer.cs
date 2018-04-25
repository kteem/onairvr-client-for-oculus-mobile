/***********************************************************

  Copyright (c) 2017-2018 Clicked, Inc.

  Licensed under the MIT license found in the LICENSE file 
  in the Docs folder of the distributed package.

 ***********************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirVRTrackedControllerPointer : AirVRPointer {
	// implements AirVRPointerBase
    protected override string inputDeviceName {
        get {
            return AirVRInputDeviceName.TrackedController;
        }
    }

    protected override byte raycastHitResultKey {
        get {
            return (byte)AirVRTrackedControllerKey.RaycastHitResult;
        }
    }

    protected override Vector3 worldOriginPosition {
        get {
            bool leftHanded = (OVRInput.GetConnectedControllers() & OVRInput.Controller.LTrackedRemote) != 0;
            return trackingOriginLocalToWorldMatrix.MultiplyPoint(OVRInput.GetLocalControllerPosition(leftHanded ? OVRInput.Controller.LTrackedRemote : OVRInput.Controller.RTrackedRemote));
        }
    }

    protected override Quaternion worldOriginOrientation {
        get {
            bool leftHanded = (OVRInput.GetConnectedControllers() & OVRInput.Controller.LTrackedRemote) != 0;
            return cameraRoot.rotation * OVRInput.GetLocalControllerRotation(leftHanded ? OVRInput.Controller.LTrackedRemote : OVRInput.Controller.RTrackedRemote);
        }
    }
}
