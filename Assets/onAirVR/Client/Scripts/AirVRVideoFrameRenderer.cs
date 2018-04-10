/***********************************************************

  Copyright (c) 2017-2018 Clicked, Inc.

  Licensed under the MIT license found in the LICENSE file 
  in the Docs folder of the distributed package.

 ***********************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirVRVideoFrameRenderer {
    private const float Depth = 0.9f;

    private Transform _cameraTransform;
    private Camera _camera;
    private MeshFilter _meshFilter;
    private MeshRenderer _renderer;

    public AirVRVideoFrameRenderer(GameObject owner, AirVRProfileBase profile, AirVRCameraBase camera) {
        _camera = camera.GetComponent<Camera>();
        _cameraTransform = camera.transform;

        float[] videoScale = profile.videoScale;

        Mesh mesh = new Mesh();
        mesh.vertices = new Vector3[] {
            new Vector3(-videoScale[0],  videoScale[1], Depth),
            new Vector3( videoScale[0],  videoScale[1], Depth),
            new Vector3(-videoScale[0], -videoScale[1], Depth),
            new Vector3( videoScale[0], -videoScale[1], Depth)
        };
        mesh.uv = new Vector2[] {
            new Vector2(0.0f, 0.0f),
            new Vector2(1.0f, 0.0f),
            new Vector2(0.0f, 1.0f),
            new Vector2(1.0f, 1.0f)
        };
        mesh.triangles = new int[] {
            0, 1, 2, 2, 1, 3
        };

        _meshFilter = owner.GetComponent<MeshFilter>();
        if (_meshFilter == null) {
            _meshFilter = owner.AddComponent<MeshFilter>();
        }
        _meshFilter.mesh = mesh;
        _meshFilter.mesh.UploadMeshData(true);

        _renderer = owner.GetComponent<MeshRenderer>();
        if (_renderer == null) {
            _renderer = owner.AddComponent<MeshRenderer>();
        }
        _renderer.material = new Material(Shader.Find("onAirVR/Video frame on far clip plane"));
        _renderer.enabled = false;
    }

    public bool enabled {
        get {
            return _renderer.enabled;
        }
        set {
            _renderer.enabled = value;
        }
    }

    public void SetVideoFrameTexture(Texture2D texture) {
        _renderer.material.mainTexture = texture;
    }

    public void Update() {
        _meshFilter.mesh.bounds = new Bounds(_cameraTransform.position + _cameraTransform.forward * (_camera.nearClipPlane + _camera.farClipPlane) / 2, Vector2.one);
    }
}
