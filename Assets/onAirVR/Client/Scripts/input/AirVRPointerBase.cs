/***********************************************************

  Copyright (c) 2017-2018 Clicked, Inc.

  Licensed under the MIT license found in the LICENSE file 
  in the Docs folder of the distributed package.

 ***********************************************************/

using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.VR;
using UnityEngine.Assertions;

public abstract class AirVRPointerBase : MonoBehaviour {
	private const float DefaultRayLength = 0.15f;
	private const float MaxRayLength = 1.5f;
	private const float RatioOfRayLengthToHit = 0.3f;
	private static float[] RayPositions = { 0.0f, 0.1f, 1.0f };
	private static GradientAlphaKey[] RayAlphaKeys = {
		new GradientAlphaKey(0.0f, 0.0f),
		new GradientAlphaKey(0.8f, 0.1f),
		new GradientAlphaKey(0.0f, 1.0f)
	};

	private class FilteredPose {
		private const float FilterCoef = 0.6f;

		private bool _bypassFilterOnNextUpdate = true;

		public Vector3 position { get; private set; }
		public Quaternion rotation { get; private set; }

		public void Update(Vector3 pos, Quaternion rot) {
			if (_bypassFilterOnNextUpdate) {
				position = pos;
				rotation = rot;

				_bypassFilterOnNextUpdate = false;
			}
			else {
				position = Vector3.Lerp(position, pos, FilterCoef);
				rotation = Quaternion.Slerp(rotation, rot, FilterCoef);
			}
		}

		public void Reset() {
			_bypassFilterOnNextUpdate = true;
		}
	}

	public class FeedbackReceiver : AirVRInputReceiver {
		public FeedbackReceiver(AirVRPointerBase owner) {
			_owner = owner;
		}

		private AirVRPointerBase _owner;

		// implements AirVRInputReceiver
		public override string name {
			get { return _owner.inputDeviceName; }
		}

		public override void OnRegistered(byte inDeviceID) {
			base.OnRegistered(inDeviceID);
			_owner.pointerEnabled = true;
		}

		public override void OnUnregistered() {
			base.OnUnregistered();
			_owner.pointerEnabled = false;
		}

		public override void PollInputsPerFrame(AirVRInputStream inputStream) {
			if (_owner.pointerEnabled) {
				inputStream.GetTrackedDeviceFeedback(this, _owner.raycastHitResultKey,
					out _owner._rayOrigin,
					out _owner._raycastHitPosition,
					out _owner._raycastHitNormal);
			}
			_owner.updateVisuals();
		}
	}

	private Transform _cookie;
	private MeshRenderer _cookieRenderer;
	private float _depthScaleMultiplier;
	private LineRenderer _ray;
	private Transform _body;
	private bool _pointerEnabled;
	private Vector3 _rayOrigin;
	private Vector3 _raycastHitPosition;
	private Vector3 _raycastHitNormal;

	private FilteredPose _worldOriginPose = new FilteredPose();

	private bool pointerEnabled {
		get { return _pointerEnabled; }
		set {
			_pointerEnabled = value;
			updateVisuals();
		}
	}

	private void createCookie(Shader cookieShader) {
		GameObject goCookie = new GameObject("Cookie");
		goCookie.transform.localPosition = Vector3.zero;
		goCookie.transform.localRotation = Quaternion.identity;
		goCookie.transform.localScale = Vector3.one;

		MeshFilter meshFilter = goCookie.AddComponent<MeshFilter>();
		meshFilter.mesh = new Mesh();
		meshFilter.mesh.vertices = new Vector3[] {
			new Vector3(-0.5f, 0.5f, 0.0f),
			new Vector3(0.5f, 0.5f, 0.0f),
			new Vector3(-0.5f, -0.5f, 0.0f),
			new Vector3(0.5f, -0.5f, 0.0f)
		};
		meshFilter.mesh.uv = new Vector2[] {
			new Vector2(1.0f, 1.0f),
			new Vector2(0.0f, 1.0f),
			new Vector2(1.0f, 0.0f),
			new Vector2(0.0f, 0.0f)
		};
		meshFilter.mesh.triangles = new int[] {
			1, 0, 3, 3, 0, 2
		};
		meshFilter.mesh.UploadMeshData(true);

		_cookie = goCookie.transform;
		_cookieRenderer = goCookie.AddComponent<MeshRenderer>();
		_cookieRenderer.material = new Material(cookieShader);

		pointerEnabled = false;
	}

	private bool isRaycastHitMissed() {
		return _raycastHitNormal == Vector3.zero;
	}

	private void updateCookie(bool pointerVisible) {
		_cookieRenderer.enabled = pointerVisible && (isRaycastHitMissed() == false);
		if (_cookieRenderer.enabled && _cookieRenderer.material.mainTexture != null) {
			float distance = trackingOriginLocalToWorldMatrix.MultiplyVector(_rayOrigin - _raycastHitPosition).magnitude;
			Quaternion rotation = _cookie.rotation;
			rotation.SetLookRotation(trackingOriginLocalToWorldMatrix.MultiplyVector(_raycastHitNormal),
				_worldOriginPose.rotation * Vector3.up);

			_cookie.position = _worldOriginPose.position + worldOriginOrientation * (distance * Vector3.forward);
			_cookie.rotation = rotation;

			_cookie.localScale = Vector3.one * _raycastHitPosition.magnitude * _depthScaleMultiplier;
		}
	}

	private void setRayPositions(Vector3 start, Vector3 end, float[] positions) {
		Debug.Assert(_ray != null);
		
		_ray.SetPosition(0, start);
		for (int i = 0; i < positions.Length; i++) {
			_ray.SetPosition(i, Vector3.Lerp(start, end, positions[i]));
		}
	}

	private void updateRay(bool pointerVisible) {
		if (_ray != null) {
			_ray.enabled = pointerVisible;
			if (_ray.enabled) {
				if (_cookieRenderer.enabled) {
					Ray ray = new Ray(_worldOriginPose.position, _cookie.position - _worldOriginPose.position);
					setRayPositions(_worldOriginPose.position,
									ray.GetPoint(RatioOfRayLengthToHit * Mathf.Min((_cookie.position - _worldOriginPose.position).magnitude, MaxRayLength)),
									RayPositions);
				}
				else {
					setRayPositions(_worldOriginPose.position,
									_worldOriginPose.position + _worldOriginPose.rotation * (Vector3.forward * DefaultRayLength),
									RayPositions);
				}
			}
		}
	}

	private void updateBody(bool pointerVisible) {
		if (_body != null) {
			_body.gameObject.SetActive(pointerVisible);

			if (pointerVisible) {
				Vector3 posOffset = worldBodyPosition - worldOriginPosition;
				Quaternion rotOffset = worldBodyOrientation * Quaternion.Inverse(worldOriginOrientation);

				_body.position = _worldOriginPose.position + posOffset;
				_body.rotation = _worldOriginPose.rotation * rotOffset;
			}
		}
	}

	private void updateVisuals() {
		bool shouldShowPointer = _pointerEnabled && AirVRClient.playing;

		if (shouldShowPointer) {
			_worldOriginPose.Update(worldOriginPosition, worldOriginOrientation);
		}
		else {
			_worldOriginPose.Reset();
		}

		updateCookie(shouldShowPointer);
		updateRay(shouldShowPointer);
		updateBody(shouldShowPointer);
	}

	void Awake() {
		feedbackReceiver = new FeedbackReceiver(this);
	}

	protected virtual void Start() {
        createCookie(Shader.Find("onAirVR/Unlit alpha blended"));
        recalculatePointerRoot();

		AirVRClient.MessageReceived += onAirVRMessageReceived;
		AirVRInputManager.RegisterTrackedDeviceFeedback(this);
    }

    void OnDestroy() {
		AirVRClient.MessageReceived -= onAirVRMessageReceived;
		AirVRInputManager.UnregisterTrackedDeviceFeedback(this);
    }

    protected abstract string inputDeviceName { get; }
    protected abstract byte raycastHitResultKey { get; }
    protected abstract void recalculatePointerRoot();
    protected abstract Matrix4x4 trackingOriginLocalToWorldMatrix { get; }
    protected abstract Vector3 worldOriginPosition { get; }
    protected abstract Quaternion worldOriginOrientation { get; }

	protected virtual Vector3 worldBodyPosition {
		get { 
			return worldOriginPosition;
		}
	}

	protected virtual Quaternion worldBodyOrientation {
		get { 
			return worldOriginOrientation;
		}
	}
	
	public FeedbackReceiver feedbackReceiver { get; private set; }

	public void Configure(GameObject bodyModelPrefab, bool createRay) {
		if (bodyModelPrefab != null) {
			_body = Instantiate(bodyModelPrefab, Vector3.zero, Quaternion.identity).transform;
		}
		if (createRay) {
			Material mat = new Material(Shader.Find("onAirVR/Unlit vertex color"));

			_ray = gameObject.AddComponent<LineRenderer>();
			_ray.positionCount = RayPositions.Length;
			_ray.receiveShadows = false;
			_ray.material = mat;
			_ray.useWorldSpace = true;
			_ray.loop = false;
			_ray.widthCurve = new AnimationCurve(new Keyframe[] { 
				new Keyframe(0.0f, 0.005f),
				new Keyframe(1.0f, 0.005f),
			});
			Gradient colorCurve = new Gradient();
			colorCurve.SetKeys(
				new GradientColorKey[] {
					new GradientColorKey(Color.white, 0.0f),
					new GradientColorKey(Color.white, 1.0f) 
				},
				RayAlphaKeys
			);
			_ray.colorGradient = colorCurve;
			_ray.enabled = false;
		}
	}

    // handle AirVRMessages
    private void onAirVRMessageReceived(AirVRClientMessage message) {
        if (message.IsSessionEvent() && message.Name.Equals(AirVRClientMessage.NameDisconnected)) {
            _cookieRenderer.material.mainTexture = null;
            _cookieRenderer.enabled = false;
            pointerEnabled = false;
        }
        else if (message.IsInputStreamEvent() && message.Name.Equals(AirVRClientMessage.NameRemoteInputDeviceRegistered)) {
            if (message.PointerCookieTexture_Decoded != null) {
                Texture2D cookieTexture = new Texture2D(2, 2);
                cookieTexture.LoadImage(message.PointerCookieTexture_Decoded);
                _cookieRenderer.material.mainTexture = cookieTexture;
            }
            if (message.PointerCookieDepthScaleMultiplier > 0.0f) {
                _depthScaleMultiplier = message.PointerCookieDepthScaleMultiplier;
            }
        }
	}
}
