/***********************************************************

  Copyright (c) 2017-2018 Clicked, Inc.

  Licensed under the MIT license found in the LICENSE file 
  in the root folder of the project.

 ***********************************************************/

using UnityEngine;

public class MouseCamera : MonoBehaviour
{
    [SerializeField] Transform cameraRigTransform;
    [SerializeField] public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
    [SerializeField] public RotationAxes axes = RotationAxes.MouseXAndY;
    [SerializeField] private float lerpSpeed = 10f;
    [SerializeField] private float sensitivityX = 10f;
    [SerializeField] private float sensitivityY = 10f;
    [SerializeField] private float minimumX = -360f;
    [SerializeField] private float maximumX = 360f;
    [SerializeField] private float minimumY = -60f;
    [SerializeField] private float maximumY = 60f;
    [SerializeField] private KeyCode togleKey = KeyCode.Slash;

    private float rotationY = 0f;
    private float rotationX = 0f;
    private Vector3 targetRot;

    private bool isActive = true;

#if UNITY_EDITOR
    void Update()
    {
        if (Input.GetKeyDown(togleKey))
        {
            isActive = !isActive;
            cameraRigTransform.rotation = Quaternion.identity;
        }

        if (!isActive)
            return;

        switch(axes)
        {
            case RotationAxes.MouseXAndY:
                {
                    SetRotationX();
                    SetRotationY();
                }
                break;
            case RotationAxes.MouseX:
                {
                    SetRotationX();
                }
                break;
            case RotationAxes.MouseY:
                {
                    SetRotationY();
                }
                break;
        }

        Rotate();
    }
#endif

    void SetRotationX()
    {
        rotationX += Input.GetAxis("Mouse X") * sensitivityX;
        rotationX = Mathf.Clamp(rotationX, minimumX, maximumX);
    }

    void SetRotationY()
    {
        rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
        rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);
    }

    void Rotate()
    {
        targetRot = new Vector3(-rotationY, rotationX, 0f);
        cameraRigTransform.rotation = Quaternion.Slerp(cameraRigTransform.rotation, Quaternion.Euler(targetRot), Time.deltaTime * lerpSpeed);
    }
}

