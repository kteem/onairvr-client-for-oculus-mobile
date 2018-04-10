/***********************************************************

  Copyright (c) 2017-2018 Clicked, Inc.

  Licensed under the MIT license found in the LICENSE file 
  in the Docs folder of the distributed package.

 ***********************************************************/

using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

[RequireComponent(typeof(AudioSource))]

public class AirVRClientAudioSource : MonoBehaviour {
    [DllImport(AirVRClient.LibPluginName)]
    private static extern bool onairvr_GetAudioData([MarshalAs(UnmanagedType.LPArray)]float[] buffer, int length, int channels);
    
    private bool _isEditor;

    void Awake() {
        _isEditor = Application.isEditor;
    }

    void OnAudioFilterRead(float[] data, int channels) {
        if (_isEditor == false) {
            if (onairvr_GetAudioData(data, data.Length / channels, channels) == false) {
                System.Array.Clear(data, 0, data.Length);
            }
        }
    }
}
