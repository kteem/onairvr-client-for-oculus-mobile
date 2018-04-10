/***********************************************************

  Copyright (c) 2017-2018 Clicked, Inc.

  Licensed under the MIT license found in the LICENSE file 
  in the Docs folder of the distributed package.

 ***********************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[System.Serializable]
public abstract class AirVRProfileBase {
    public enum RenderType {
        DirectOnTwoEyeTextures,
        UseSeperateVideoRenderTarget
    }
		
	#pragma warning disable CS0414
	[SerializeField] private string UserID;
	[SerializeField] private string[] SupportedVideoCodecs;
	[SerializeField] private string[] SupportedAudioCodecs;
	[SerializeField] private int VideoWidth;
	[SerializeField] private int VideoHeight;
	[SerializeField] private float VideoFrameRate;
    [SerializeField] private float IPD;
	[SerializeField] private bool Stereoscopy;
	[SerializeField] private bool IsEyeCameraFrustumSymmetric;
	[SerializeField] private float EyeCameraVerticalFOV;
	[SerializeField] private float EyeCameraAspectRatio;
	[SerializeField] private float[] LeftEyeCameraNearPlane;
	[SerializeField] private Vector3 EyeCenterPosition;

	[SerializeField] private int[] LeftEyeViewport;
	[SerializeField] private int[] RightEyeViewport;
	[SerializeField] private float[] VideoRenderMeshVertices;
	[SerializeField] private float[] VideoRenderMeshTexCoords;
	[SerializeField] private int[] VideoRenderMeshIndices;
	[SerializeField] private float[] VideoScale;
	#pragma warning restore CS0414

	private string[] supportedVideoCodecs {
		get {
			bool supportAVC = false;
			bool supportHEVC = false;

			if (Application.isEditor == false && Application.platform == RuntimePlatform.Android) {
				AndroidJavaObject mediaCodecList = new AndroidJavaObject("android.media.MediaCodecList", 0);
				AndroidJavaObject[] mediaCodecInfos = mediaCodecList.Call<AndroidJavaObject[]>("getCodecInfos");
				foreach (AndroidJavaObject codecInfo in mediaCodecInfos) {
					string[] types = codecInfo.Call<string[]>("getSupportedTypes");
					foreach (string type in types) {
						if (type.Equals("video/avc")) {
							supportAVC = true;
						} else if (type.Equals("video/hevc")) {
							supportHEVC = this.supportHEVC;
						}
					}
				}
			} else {
				supportAVC = supportHEVC = true;
			}
			Assert.IsTrue(supportHEVC || supportAVC);
			string[] result = new string[(supportHEVC && supportAVC) ? 2 : 1];
			if (supportHEVC) {
				result[0] = "H265";
			}
			if (supportAVC) {
				result[result.Length - 1] = "H264";
			}

			return result;
		}
    }

	private string[] supportedAudioCodecs {
		get {
			return new string[] { "opus" };
		}
	}

    private float[] leftEyeCameraNearPlaneScaled { 
        get {
            float[] result = leftEyeCameraNearPlane;
            float[] scale = videoScale;
            result[0] *= scale[0];
            result[1] *= scale[1];
            result[2] *= scale[0];
            result[3] *= scale[1];

            return result;
        }
    }
        
    public abstract int videoWidth { get; }
    public abstract int videoHeight { get; }
    public abstract float videoFrameRate { get; }
	public abstract bool stereoscopy { get; }
    public abstract bool isEyeCameraFrustumSymmetric { get; }
    public abstract float eyeCameraVerticalFieldOfView { get; }     // valid for symmetric eye camera only
    public abstract float eyeCameraAspectRatio { get; }             // valid for symmetric eye camera only
    public abstract float[] leftEyeCameraNearPlane { get; }
    public abstract Vector3 eyeCenterPosition { get; }
    public abstract float ipd { get; }
	public abstract bool hasInput { get; }

	public abstract RenderType renderType { get; }
	public abstract int[] leftEyeViewport { get; }
	public abstract int[] rightEyeViewport { get; }
	public abstract float[] videoScale { get; }   // ratio of the size of the whole video rendered to the size of the area visible to an eye camera
	
	public abstract bool isUserPresent { get; }
	public abstract float delayToResumePlayback { get; }

	public virtual float[] videoRenderMeshVertices { 
		get { 
			return new float[] { 
				-0.5f,  0.5f, 0.0f,
				0.5f,  0.5f, 0.0f,
				-0.5f, -0.5f, 0.0f,
				0.5f, -0.5f, 0.0f 
			};
		}
	}

	public virtual float[] videoRenderMeshTexCoords { 
		get { 
			return new float[] {
				0.0f, 1.0f,
				1.0f, 1.0f,
				0.0f, 0.0f,
				1.0f, 0.0f
			};
		}
	}

	public virtual int[] videoRenderMeshIndices { 
		get { 
			return new int[] {
				0, 1, 2, 2, 1, 3
			};
		}
	}

    public virtual bool supportHEVC {
        get {
            return true;
        }
    }

    public bool useSeperateVideoRenderTarget { 
        get {
            return renderType == RenderType.UseSeperateVideoRenderTarget;
        }
    }

    public bool useSingleTextureForEyes {
        get {
            return renderType == RenderType.UseSeperateVideoRenderTarget;
        }
    }

	public string userID {
		get { 
			return UserID;
		}
		set { 
			UserID = value;
		}
	}

	public AirVRProfileBase GetSerializable() {
		SupportedVideoCodecs = supportedVideoCodecs;
		SupportedAudioCodecs = supportedAudioCodecs;
		VideoWidth = videoWidth;
		VideoHeight = videoHeight;
		VideoFrameRate = videoFrameRate;
        IPD = ipd;
		Stereoscopy = stereoscopy;
		IsEyeCameraFrustumSymmetric = isEyeCameraFrustumSymmetric;
		EyeCameraVerticalFOV = eyeCameraVerticalFieldOfView;
		EyeCameraAspectRatio = eyeCameraAspectRatio;
		LeftEyeCameraNearPlane = leftEyeCameraNearPlane;
		EyeCenterPosition = eyeCenterPosition;

		LeftEyeViewport = leftEyeViewport;
		RightEyeViewport = rightEyeViewport;
		VideoRenderMeshVertices = videoRenderMeshVertices;
		VideoRenderMeshTexCoords = videoRenderMeshTexCoords;
		VideoRenderMeshIndices = videoRenderMeshIndices;
		VideoScale = videoScale;

		return this;
	}

    public override string ToString () {
        return string.Format("[AirVRProfile]\n" +
                             "    videoWidth={0}\n" +
                             "    videoHeight={1}\n" +
                             "    videoFrameRate={2}\n" + 
                             "    videoScale=({3}, {4})\n" + 
                             "    render type={5}\n" +
                             "    leftEyeViewport=({6}, {7}, {8}, {9})\n" + 
                             "    rightEyeViewport=({10}, {11}, {12}, {13})\n" + 
                             "    isEyeCameraFrustumSymmetric={14}\n" + 
                             "    eyeCameraVerticalFieldOfView={15}\n" + 
                             "    eyeCameraAspectRatio={16}\n" + 
                             "    leftEyeCameraNearPlane=({17}, {18}, {19}, {20})\n" +
                             "    eyeCenterPosition={21}\n" + 
                             "    ipd={22}\n" + 
                             "    stereoscopy={23}\n", 
                             videoWidth, 
                             videoHeight, 
                             videoFrameRate, 
                             videoScale[0], videoScale[1], 
                             renderType, 
                             leftEyeViewport[0], leftEyeViewport[1], leftEyeViewport[2], leftEyeViewport[3], 
                             rightEyeViewport[0], rightEyeViewport[1], rightEyeViewport[2], rightEyeViewport[3], 
                             isEyeCameraFrustumSymmetric, 
                             eyeCameraVerticalFieldOfView, 
                             eyeCameraAspectRatio, 
                             leftEyeCameraNearPlane[0], leftEyeCameraNearPlane[1], leftEyeCameraNearPlane[2], leftEyeCameraNearPlane[3], 
                             eyeCenterPosition, 
                             ipd, 
                             stereoscopy);
    }
}
