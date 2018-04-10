/***********************************************************

  Copyright (c) 2017-2018 Clicked, Inc.

  Licensed under the MIT license found in the LICENSE file 
  in the Docs folder of the distributed package.

 ***********************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;

public abstract class AirVRPointer : AirVRPointerBase {
    private Transform _trackingSpace;
    private Matrix4x4 _trackingLocalToWorldMatrix;

    protected Transform cameraRoot { get; private set; }

    protected override void recalculatePointerRoot() {
        OVRCameraRig cameraRig = FindObjectOfType<OVRCameraRig>();
        _trackingSpace = cameraRig != null ? cameraRig.trackingSpace : Camera.main.transform;
        cameraRoot = cameraRig != null ? cameraRig.transform : _trackingSpace;
        _trackingLocalToWorldMatrix = Matrix4x4.identity;
    }
        
    protected override Matrix4x4 trackingOriginLocalToWorldMatrix {
        get {
            if (cameraRoot != null && _trackingSpace != null) {
                _trackingLocalToWorldMatrix.SetTRS(_trackingSpace.position, cameraRoot.rotation, cameraRoot.localScale);
                return _trackingLocalToWorldMatrix;
            }
            return Matrix4x4.identity;
        }
    }
}
