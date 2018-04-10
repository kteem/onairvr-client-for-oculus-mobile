/***********************************************************

  Copyright (c) 2017-2018 Clicked, Inc.

  Licensed under the MIT license found in the LICENSE file 
  in the root folder of the project.

 ***********************************************************/

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class AirVRClientNumberPadKey : MonoBehaviour, IPointerDownHandler
{
    public AirVRClientNumberPad.Key key;

    private AirVRClientNumberPad _owner;

    public AirVRClientNumberPad owner
    {
        set
        {
            _owner = value;
        }
    }

    // implements IPointerDownHandler
    public void OnPointerDown(PointerEventData eventData)
    {
        if (_owner != null)
        {
            _owner.OnKeyClicked(this);
        }
    }
}
