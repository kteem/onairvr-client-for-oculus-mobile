/***********************************************************

  Copyright (c) 2017-2018 Clicked, Inc.

  Licensed under the MIT license found in the LICENSE file 
  in the root folder of the project.

 ***********************************************************/

using UnityEngine;
using UnityEngine.EventSystems;

public class AirVRClientInputModule : OVRInputModule
{
    protected override PointerEventData.FramePressState GetGazeButtonState()
    {
        var pressed = OVRInput.GetDown(OVRInput.Button.One) || Input.GetMouseButtonDown(0) || Input.GetKeyDown(gazeClickKey);
        var released = OVRInput.GetUp(OVRInput.Button.One) || Input.GetMouseButtonUp(0) || Input.GetKeyUp(gazeClickKey);

        if (pressed && released)
            return PointerEventData.FramePressState.PressedAndReleased;
        if (pressed)
            return PointerEventData.FramePressState.Pressed;
        if (released)
            return PointerEventData.FramePressState.Released;
        return PointerEventData.FramePressState.NotChanged;
    }
}
