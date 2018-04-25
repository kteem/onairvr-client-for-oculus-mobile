/***********************************************************

  Copyright (c) 2017-2018 Clicked, Inc.

  Licensed under the MIT license found in the LICENSE file 
  in the root folder of the project.

 ***********************************************************/

using UnityEngine;
using UnityEngine.UI;

public class AirVRClientNetworkIndicator : MonoBehaviour
{
    [SerializeField] private Image[] signalBars;
    [SerializeField] private float frequencyTime = 1;

    private Text _networkName;
    private float _elapsedTime;

    private void Awake() 
    {
        _networkName = GetComponentInChildren<Text>();
    }

#if UNITY_ANDROID && !UNITY_EDITOR
    private void Start()
    {
        _elapsedTime = frequencyTime;
    }

    private void Update()
    {
        if (_elapsedTime >= frequencyTime)
        {
            _elapsedTime = 0f;

            _networkName.text = GetNetworkName().Trim('\"');

            int signalLevel = GetSignalLevel(4);

            for (int i = 0; i < signalBars.Length; i++)
            {
                signalBars[i].enabled = i < signalLevel;
            }
        }

        _elapsedTime += Time.deltaTime;
    }
#endif


    private string GetNetworkName()
    {
        if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
        {
            return "Cellular";
        }

        if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
        {         

            using (var activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity"))
            {
                var wifiManager = activity.Call<AndroidJavaObject>("getSystemService", "wifi");
                return wifiManager.Call<AndroidJavaObject>("getConnectionInfo").Call<string>("getSSID");
            }
        }

        return "N/A";
    }

    private int GetSignalLevel(int numberOfLevels)
    {
        using (var activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity"))
        {
            var wifiManager = activity.Call<AndroidJavaObject>("getSystemService", "wifi");
            var wifiInfo = wifiManager.Call<AndroidJavaObject>("getConnectionInfo");
            int rssi = wifiInfo.Call<int>("getRssi");
            return wifiManager.CallStatic<int>("calculateSignalLevel", rssi, numberOfLevels);
        }
    }  
}
