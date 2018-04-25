/***********************************************************

  Copyright (c) 2017-2018 Clicked, Inc.

  Licensed under the MIT license found in the LICENSE file 
  in the Docs folder of the distributed package.

 ***********************************************************/

using UnityEngine;
using System;

[Serializable]
public class AirVRMessage {
    public const string TypeEvent = "Event";
    public const string TypeUserData = "userdata";

    public IntPtr source { get; set; }

    public string Type;

    [SerializeField]
    protected string Data;
    public byte[] Data_Decoded { get; private set; }

    protected virtual void postParse() {
        if (string.IsNullOrEmpty(Data) == false) {
            Data_Decoded = System.Convert.FromBase64String(Data);
        }
    }
}
