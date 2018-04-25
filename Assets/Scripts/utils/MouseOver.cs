/***********************************************************

  Copyright (c) 2017-2018 Clicked, Inc.

  Licensed under the MIT license found in the LICENSE file 
  in the root folder of the project.

 ***********************************************************/

using UnityEngine;
using UnityEngine.EventSystems;

public class MouseOver : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool showLog = false;
    GameObject currentHover;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject != null)
        {
            Debug.Log("Mouse Over: " + eventData.pointerCurrentRaycast.gameObject.name);
            currentHover = eventData.pointerCurrentRaycast.gameObject;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        currentHover = null;
    }

    private void Update()
    {
        if (currentHover && showLog)
            Debug.Log(currentHover.name + " @ " + Input.mousePosition);
    }
}