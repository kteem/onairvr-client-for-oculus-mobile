/***********************************************************

  Copyright (c) 2017-2018 Clicked, Inc.

  Licensed under the MIT license found in the LICENSE file 
  in the Docs folder of the distributed package.

 ***********************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Assertions;

public class AirVRClientInputStream : AirVRInputStream {
    [DllImport(AirVRClient.LibPluginName)]
    private static extern byte onairvr_RegisterInputSender(string name);

    [DllImport(AirVRClient.LibPluginName)]
    private static extern void onairvr_UnregisterInputSender(byte id);

    [DllImport(AirVRClient.LibPluginName)]
    private static extern void onairvr_PendInputTouch(byte deviceID, byte controlID, float posX, float posY, float touch, byte policy);

    [DllImport(AirVRClient.LibPluginName)]
    private static extern void onairvr_PendInputTransform(byte deviceID, byte controlID, float posX, float posY, float posZ, 
                                                          float rotX, float rotY, float rotZ, float rotW, byte policy);

    [DllImport(AirVRClient.LibPluginName)]
    private static extern void onairvr_PendInputFloat4(byte deviceID, byte controlID, float x, float y, float z, float w, byte policy);

    [DllImport(AirVRClient.LibPluginName)]
    private static extern void onairvr_PendInputFloat3(byte deviceID, byte controlID, float x, float y, float z, byte policy);

    [DllImport(AirVRClient.LibPluginName)]
    private static extern void onairvr_PendInputFloat2(byte deviceID, byte controlID, float x, float y, byte policy);

    [DllImport(AirVRClient.LibPluginName)]
    private static extern void onairvr_PendInputFloat(byte deviceID, byte controlID, float value, byte policy);

    [DllImport(AirVRClient.LibPluginName)]
    private static extern bool onairvr_GetTrackedDeviceFeedback(byte deviceID, byte controlID, 
                                                                ref float worldRayOriginX, ref float worldRayOriginY, ref float worldRayOriginZ,
                                                                ref float worldHitPositionX, ref float worldHitPositionY, ref float worldHitPositionZ,
                                                                ref float worldHitNormalX, ref float worldHitNormalY, ref float worldHitNormalZ);

    [DllImport(AirVRClient.LibPluginName)]
    private static extern void onairvr_SendPendingInputs();

    [DllImport(AirVRClient.LibPluginName)]
    private static extern void onairvr_ResetInput();

    public override void Init() {
        foreach (var key in senders.Keys) {
            senders[key].OnRegistered(onairvr_RegisterInputSender(senders[key].name));
        }

        base.Init();
    }

    public bool CheckIfInputReceiverAvailable(AirVRInputReceiver receiver) {
        return receivers.ContainsValue(receiver) && receiver.isRegistered;
    }

    public void RegisterInputReceiver(AirVRInputReceiver receiver) {
        Assert.IsFalse(initialized);
        if (receivers.ContainsValue(receiver) == false) {
            receivers.Add(receiver.name, receiver);
        }
    }

    public void UnregisterInputReceiver(AirVRInputReceiver receiver) {
        receivers.Remove(receiver.name);
    }

    public bool CheckIfInputSenderAvailable(AirVRInputSender sender) {
        return senders.ContainsValue(sender) && sender.isRegistered;
    }

    public void EnableInputSender(AirVRInputSender sender) {
        if (senders.ContainsKey(sender.name) == false) {
            senders.Add(sender.name, sender);
            if (initialized) {
                sender.OnRegistered(onairvr_RegisterInputSender(sender.name));
            }
        }
    }

    public void DisableInputSender(AirVRInputSender sender) {
        if (senders.ContainsKey(sender.name)) {
            if (sender.isRegistered) {
                onairvr_UnregisterInputSender((byte)sender.deviceID);
                sender.OnUnregistered();
            }
            senders.Remove(sender.name);
        }
    }

    // implements AirVRInputStreaming
    protected override float sendingRatePerSec {
        get {
            return 60.0f;
        }
    }

    protected override void UnregisterInputSenderImpl(byte id) {
        onairvr_UnregisterInputSender(id);
    }

    protected override void PendInputTouchImpl(byte deviceID, byte controlID, Vector2 position, float touch, byte policy) {
        onairvr_PendInputTouch(deviceID, controlID, position.x, position.y, touch, policy);
    }

    protected override void PendInputTransformImpl(byte deviceID, byte controlID, Vector3 position, Quaternion orientation, byte policy) {
        onairvr_PendInputTransform(deviceID, controlID, position.x, position.y, position.z,
                           orientation.x, orientation.y, orientation.z, orientation.w, policy);
    }

    protected override void PendInputFloat4Impl(byte deviceID, byte controlID, Vector4 value, byte policy) {
        onairvr_PendInputFloat4(deviceID, controlID, value.x, value.y, value.z, value.w, policy);
    }

    protected override void PendInputFloat3Impl(byte deviceID, byte controlID, Vector3 value, byte policy) {
        onairvr_PendInputFloat3(deviceID, controlID, value.x, value.y, value.z, policy);
    }

    protected override void PendInputFloat2Impl(byte deviceID, byte controlID, Vector2 value, byte policy) {
        onairvr_PendInputFloat2(deviceID, controlID, value.x, value.y, policy);
    }

    protected override void PendInputFloatImpl(byte deviceID, byte controlID, float value, byte policy) {
        onairvr_PendInputFloat(deviceID, controlID, value, policy);
    }

    protected override void PendTrackedDeviceFeedbackImpl(byte deviceID, byte controlID, Vector3 worldRayOrigin, Vector3 worldHitPosition, Vector3 worldHitNormal, byte policy) {
        Assert.IsTrue(false);
    }

    protected override bool GetInputTouchImpl(byte deviceID, byte controlID, ref Vector2 position, ref float touch) {
        Assert.IsTrue(false);
        return false;
    }

    protected override bool GetInputTransformImpl(byte deviceID, byte controlID, ref double timeStamp, ref Vector3 position, ref Quaternion orientation) {
        Assert.IsTrue(false);
        return false;
    }

    protected override bool GetInputFloat4Impl(byte deviceID, byte controlID, ref Vector4 value) {
        Assert.IsTrue(false);
        return false;
    }

    protected override bool GetInputFloat3Impl(byte deviceID, byte controlID, ref Vector3 value) {
        Assert.IsTrue(false);
        return false;
    }

    protected override bool GetInputFloat2Impl(byte deviceID, byte controlID, ref Vector2 value) {
        Assert.IsTrue(false);
        return false;
    }

    protected override bool GetInputFloatImpl(byte deviceID, byte controlID, ref float value) {
        Assert.IsTrue(false);
        return false;
    }

    protected override bool GetTrackedDeviceFeedbackImpl(byte deviceID, byte controlID, ref Vector3 worldRayOrigin, ref Vector3 worldHitPosition, ref Vector3 worldHitNormal) {
        return onairvr_GetTrackedDeviceFeedback(deviceID, controlID, 
                                                ref worldRayOrigin.x, ref worldRayOrigin.y, ref worldRayOrigin.z,
                                                ref worldHitPosition.x, ref worldHitPosition.y, ref worldHitPosition.z,
                                                ref worldHitNormal.x, ref worldHitNormal.y, ref worldHitNormal.z);
    }

    protected override void SendPendingInputEventsImpl() {
        onairvr_SendPendingInputs();
    }

    protected override void ResetInputImpl() {
        onairvr_ResetInput();
    }
}
