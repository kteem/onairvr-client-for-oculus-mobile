/***********************************************************

  Copyright (c) 2017-2018 Clicked, Inc.

  Licensed under the MIT license found in the LICENSE file 
  in the Docs folder of the distributed package.

 ***********************************************************/

using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Assertions;

public class AirVRInputManager : MonoBehaviour {
    private static AirVRInputManager _instance;

    public static void LoadOnce() {
        if (_instance == null) {
            GameObject go = new GameObject("AirVRInputManager");
            go.AddComponent<AirVRInputManager>();
        }
    }
    
    public static void RegisterInputDevice(AirVRInputDevice device) {
        Assert.IsNotNull(_instance);
        if (_instance._inputDevices.Contains(device) == false) {
            _instance._inputDevices.Add(device);
        }
    }

    public static void UnregisterInputDevice(AirVRInputDevice device) {
        if (_instance != null) {
            _instance._inputDevices.Remove(device);
        }
    }
    
    public static void RegisterTrackedDeviceFeedback(AirVRPointerBase pointer) {
        Assert.IsNotNull(_instance);
        _instance._inputStream.RegisterInputReceiver(pointer.feedbackReceiver);
    }

    public static void UnregisterTrackedDeviceFeedback(AirVRPointerBase pointer) {
        if (_instance != null) {
            _instance._inputStream.UnregisterInputReceiver(pointer.feedbackReceiver);
        }
    }

    public static bool CheckIfInputDeviceEnabled(AirVRInputDevice device) {
        Assert.IsNotNull(_instance);
        return _instance._inputStream.CheckIfInputSenderAvailable(device);
    }
    
    public static void EnableInputDevice(AirVRInputDevice device) {
        Assert.IsNotNull(_instance);
        _instance._inputStream.EnableInputSender(device);
    }

    public static void DisableInputDevice(AirVRInputDevice device) {
        if (_instance != null) {
            _instance._inputStream.DisableInputSender(device);
        }
    }

    private List<AirVRInputDevice> _inputDevices;
    private AirVRClientInputStream _inputStream;

    void Awake() {
        Assert.IsNull(_instance);
        _instance = this;
        DontDestroyOnLoad(gameObject);

        _inputStream = new AirVRClientInputStream();
        _inputDevices = new List<AirVRInputDevice>();

		AirVRClient.MessageReceived += onAirVRMessageReceived;
    }

    void Update() {
        foreach (var device in _inputDevices) {
            device.Update();
        }
        _inputStream.UpdateReceivers();
    }

    void LateUpdate() {
        _inputStream.UpdateSenders();
    }

    void OnDestroy() {
		AirVRClient.MessageReceived -= onAirVRMessageReceived;
    }

	// handle AirVRMessages
	private void onAirVRMessageReceived(AirVRClientMessage message) {
        if (message.IsSessionEvent()) {
            if (message.Name.Equals(AirVRClientMessage.NameSetupResponded)) {
                _inputStream.Init();
            }
            else if (message.Name.Equals(AirVRClientMessage.NamePlayResponded)) {
                _inputStream.Start();
            }
            else if (message.Name.Equals(AirVRClientMessage.NameStopResponded)) {
                _inputStream.Stop();
            }
            else if (message.Name.Equals(AirVRClientMessage.NameDisconnected)) {
                _inputStream.Cleanup();
            }
        }
        else if (message.IsInputStreamEvent()) {
            if (message.Name.Equals(AirVRClientMessage.NameRemoteInputDeviceRegistered)) {
                _inputStream.HandleRemoteInputDeviceRegistered(message.DeviceName, (byte)message.DeviceID);
            }
            else if (message.Name.Equals(AirVRClientMessage.NameRemoteInputDeviceUnregistered)) {
                _inputStream.HandleRemoteInputDeviceUnregistered((byte)message.DeviceID);
            }
        }
	}
}
