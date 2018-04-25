/***********************************************************

  Copyright (c) 2017-2018 Clicked, Inc.

  Licensed under the MIT license found in the LICENSE file 
  in the root folder of the project.

 ***********************************************************/

using UnityEngine;
using UnityEngine.EventSystems;

public class PointerOver : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private bool _isPointerOver;

    private void Awake()
    {
        _isPointerOver = false;
    }

    private void OnDisable()
    {
        _isPointerOver = false;
    }

    public bool IsPointerOver
    {
        get { return _isPointerOver; }        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _isPointerOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _isPointerOver = false;
    }
}
