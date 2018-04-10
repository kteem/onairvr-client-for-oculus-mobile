/***********************************************************

  Copyright (c) 2017-2018 Clicked, Inc.

  Licensed under the MIT license found in the LICENSE file 
  in the Docs folder of the distributed package.

 ***********************************************************/

using UnityEngine;
using System.Collections;

internal class AirVRClientLicenseFile {
    internal AirVRClientLicenseFile(string filename) {
        _path = Application.persistentDataPath + "/" + filename;

        if (Application.isEditor == false) {
            AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject context = jc.GetStatic<AndroidJavaObject>("currentActivity").Call<AndroidJavaObject>("getApplicationContext");
            AndroidJavaObject assetManager = context.Call<AndroidJavaObject>("getAssets");
            AndroidJavaObject inputStream = assetManager.Call<AndroidJavaObject>("open", filename);

            AndroidJavaObject outputStream = new AndroidJavaObject("java.io.FileOutputStream", _path);
            int read = inputStream.Call<int>("read");
            while (read != -1) {
                outputStream.Call("write", read);
                read = inputStream.Call<int>("read");
            }
            outputStream.Call("close");
        }
    }

    private string _path;

    internal string path {
        get {
            return _path;
        }
    }

    internal void Release() {
        if (Application.isEditor == false) {
            System.IO.File.Delete(_path);
        }
    }
}
