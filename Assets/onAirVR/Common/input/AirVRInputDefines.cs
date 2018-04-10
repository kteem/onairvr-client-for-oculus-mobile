/***********************************************************

  Copyright (c) 2017-2018 Clicked, Inc.

  Licensed under the MIT license found in the LICENSE file 
  in the Docs folder of the distributed package.

 ***********************************************************/

public static class AirVRInputDeviceName {
    public const string HeadTracker = "HeadTracker";
    public const string Touchpad = "Touchpad";
    public const string Gamepad = "Gamepad";
    public const string TrackedController = "TrackedController";
}

public enum AirVRHeadTrackerKey {
    Transform = 0,
    RaycastHitResult,

    // ADD ADDITIONAL KEYS HERE

    Max
}

public enum AirVRTouchpadKey {
    Touchpad = 0,

    ButtonBack,
    ButtonUp,
    ButtonDown,
    ButtonLeft,
    ButtonRight,

    // ADD ADDITIONAL KEYS HERE

    ExtAxis2DPosition,
    ExtButtonTouch,

    Max
}

public enum AirVRGamepadKey {
    Axis2DLThumbstick = 0,
    Axis2DRThumbstick,
    AxisLIndexTrigger,
    AxisRIndexTrigger,

    ButtonA,
    ButtonB,
    ButtonX,
    ButtonY,
    ButtonStart,
    ButtonBack,
    ButtonLShoulder,
    ButtonRShoulder,
    ButtonLThumbstick,
    ButtonRThumbstick,
    ButtonDpadUp,
    ButtonDpadDown,
    ButtonDpadLeft,
    ButtonDpadRight,

    // ADD ADDITIONAL KEYS HERE

    ExtButtonLIndexTrigger,
    ExtButtonRIndexTrigger,
    ExtButtonLThumbstickUp,
    ExtButtonLThumbstickDown,
    ExtButtonLThumbstickLeft,
    ExtButtonLThumbstickRight,
    ExtButtonRThumbstickUp,
    ExtButtonRThumbstickDown,
    ExtButtonRThumbstickLeft,
    ExtButtonRThumbstickRight,

    Max
}

public enum AirVRTrackedControllerKey {
    Touchpad = 0,
    Transform,
    RaycastHitResult,

    ButtonTouchpad,
    ButtonBack,
    ButtonIndexTrigger,
    ButtonUp,
    ButtonDown,
    ButtonLeft,
    ButtonRight,

    // ADD ADDITIONAL KEYS HERE

    ExtAxis2DTouchPosition,
    ExtButtonTouch,

    Max
}

