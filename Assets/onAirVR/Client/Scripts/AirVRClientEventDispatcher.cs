/***********************************************************

  Copyright (c) 2017-2018 Clicked, Inc.

  Licensed under the MIT license found in the LICENSE file 
  in the Docs folder of the distributed package.

 ***********************************************************/

using UnityEngine;
using System;
using System.IO;
using System.Runtime.InteropServices;

public class AirVRClientEventDispatcher : AirVREventDispatcher {
    [DllImport(AirVRClient.LibPluginName)]
    private static extern void onairvr_SendUserData(IntPtr data, int length);

    [DllImport(AirVRClient.LibPluginName)]
	private static extern bool onairvr_CheckMessageQueue(out IntPtr source, out IntPtr data, out int length);

	[DllImport(AirVRClient.LibPluginName)]
	private static extern void onairvr_RemoveFirstMessageFromQueue();

    protected override AirVRMessage ParseMessageImpl(IntPtr source, string message) {
        return AirVRClientMessage.Parse(source, message);
    }

    protected override bool CheckMessageQueueImpl(out IntPtr source, out IntPtr data, out int length) {
		return onairvr_CheckMessageQueue(out source, out data, out length);
    }

    protected override void RemoveFirstMessageFromQueueImpl() {
		onairvr_RemoveFirstMessageFromQueue();
    }

    internal void SendUserData(byte[] data) {
        IntPtr ptr = Marshal.AllocHGlobal(data.Length);

        try {
            Marshal.Copy(data, 0, ptr, data.Length);
            onairvr_SendUserData(ptr, data.Length);
        }
        finally {
            Marshal.FreeHGlobal(ptr);
        }
    }
}
