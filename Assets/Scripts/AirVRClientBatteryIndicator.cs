/***********************************************************

  Copyright (c) 2017-2018 Clicked, Inc.

  Licensed under the MIT license found in the LICENSE file 
  in the root folder of the project.

 ***********************************************************/

using UnityEngine;
using UnityEngine.UI;

public class AirVRClientBatteryIndicator : MonoBehaviour
{
    [SerializeField] private Image guage;
    [SerializeField] private float frequencyTime = 1;

    private float _elapsedTime;

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

            float batteryLevel = OVRManager.batteryLevel;
            guage.fillAmount = batteryLevel;

            if (batteryLevel < 0.15f)
            {
                guage.color = Color.red;
            }
            else
            {
                guage.color = Color.white;
            }
        }
        _elapsedTime += Time.deltaTime;
    }
#endif
}
