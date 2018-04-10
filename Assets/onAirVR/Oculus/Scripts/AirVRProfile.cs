/***********************************************************

  Copyright (c) 2017-2018 Clicked, Inc.

  Licensed under the MIT license found in the LICENSE file 
  in the Docs folder of the distributed package.

 ***********************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Comparers;

public class AirVRProfile : AirVRProfileBase {
	private bool _userPresent;

	public override int videoWidth { 
        get {
            return 2560;
        }
    }

    public override int videoHeight { 
        get {
            return 1280;
        }
    }

    public override float videoFrameRate {
        get {
#if !UNITY_EDITOR && UNITY_ANDROID
            AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject activity = jc.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject display = activity.Call<AndroidJavaObject>("getSystemService", "window").Call<AndroidJavaObject>("getDefaultDisplay");
            return display.Call<float>("getRefreshRate");
#else
            return 59.2f;
#endif
        }
    }

	public override bool stereoscopy { 
		get {
			return true;
		}
	}

    public override bool isEyeCameraFrustumSymmetric { 
        get {
            return true;
        }
    }

    public override float eyeCameraVerticalFieldOfView { 
        get {
            OVRDisplay.EyeRenderDesc desc = OVRManager.display.GetEyeRenderDesc(UnityEngine.XR.XRNode.LeftEye);
            return Mathf.Atan(Mathf.Tan(desc.fov.y / 2 * Mathf.Deg2Rad) * videoScale[1]) * 2 * Mathf.Rad2Deg;
        }
    }

    public override float eyeCameraAspectRatio { 
        get {
            OVRDisplay.EyeRenderDesc desc = OVRManager.display.GetEyeRenderDesc(UnityEngine.XR.XRNode.LeftEye);
            return (float)desc.resolution.x / desc.resolution.y;
        }
    }

    public override float[] leftEyeCameraNearPlane { 
        get {
            OVRDisplay.EyeRenderDesc desc = OVRManager.display.GetEyeRenderDesc(UnityEngine.XR.XRNode.LeftEye);
            float x = Mathf.Tan(desc.fov.x / 180.0f * Mathf.PI * 0.5f);
            float y = Mathf.Tan(desc.fov.y / 180.0f * Mathf.PI * 0.5f);

            // returns (l, t, r, b) of the near plane of the camera frustum when n = 1.
            return new float[] { -x, y, x, -y };
        }
    }

    public override Vector3 eyeCenterPosition { 
        get {
            return new Vector3(0.0f, OVRManager.profile.eyeHeight - OVRManager.profile.neckHeight, OVRManager.profile.eyeDepth);
        }
    }

    public override float ipd { 
        get {
            return OVRManager.profile.ipd;
        }
    }

	public override bool hasInput {
		get {
			return true;
		}
	}

	public override RenderType renderType {
		get {
			return RenderType.DirectOnTwoEyeTextures;
		}
	}

	public override int[] leftEyeViewport { 
		get {
			OVRDisplay.EyeRenderDesc desc = OVRManager.display.GetEyeRenderDesc(UnityEngine.XR.XRNode.LeftEye);
			return new int[] { 0, 0, (int)desc.resolution.x, (int)desc.resolution.y };
		}
	}

	public override int[] rightEyeViewport { 
		get {
			return leftEyeViewport;
		}
	}

	public override float[] videoRenderMeshVertices { 
		get { 
			return new float[] { 
				-0.5f,  0.5f, 0.0f,
				 0.5f,  0.5f, 0.0f,
				-0.5f, -0.5f, 0.0f,
				 0.5f, -0.5f, 0.0f 
			};
		}
	}

	public override float[] videoRenderMeshTexCoords { 
		get { 
			return new float[] {
				0.0f, 1.0f,
				1.0f, 1.0f,
				0.0f, 0.0f,
				1.0f, 0.0f
			};
		}
	}

	public override int[] videoRenderMeshIndices { 
		get { 
			return new int[] {
				0, 1, 2, 2, 1, 3
			};
		}
	}

	public override float[] videoScale {
		get {
			OVRDisplay.EyeRenderDesc desc = OVRManager.display.GetEyeRenderDesc(UnityEngine.XR.XRNode.LeftEye);
			return new float[] { (float)videoWidth / 2 / desc.resolution.x, (float)videoHeight / desc.resolution.y };
		}
	}

	public override bool isUserPresent {
		get {
			return OVRManager.instance.isUserPresent;
		}
	}

	public override float delayToResumePlayback {
		get {
			return 4.0f;
		}
	}
}
