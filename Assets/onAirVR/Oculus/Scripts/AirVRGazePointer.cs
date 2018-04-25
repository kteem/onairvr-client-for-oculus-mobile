/***********************************************************

  Copyright (c) 2017-2018 Clicked, Inc.

  Licensed under the MIT license found in the LICENSE file 
  in the Docs folder of the distributed package.

 ***********************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;

public class AirVRGazePointer : AirVRPointer {
    // implements AirVRPointer
    protected override string inputDeviceName {
        get {
            return AirVRInputDeviceName.HeadTracker;
        }
    }

    protected override byte raycastHitResultKey {
        get {
            return (byte)AirVRHeadTrackerKey.RaycastHitResult;
        }
    }

    protected override Vector3 worldOriginPosition {
        get {
            return trackingOriginLocalToWorldMatrix.MultiplyPoint(UnityEngine.XR.InputTracking.GetLocalPosition(UnityEngine.XR.XRNode.CenterEye));
        }
    }

    protected override Quaternion worldOriginOrientation {
        get {
            return cameraRoot.rotation * UnityEngine.XR.InputTracking.GetLocalRotation(UnityEngine.XR.XRNode.CenterEye);
        }
    }
}
