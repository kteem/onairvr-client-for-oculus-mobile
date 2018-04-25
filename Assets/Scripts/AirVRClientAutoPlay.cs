/***********************************************************

  Copyright (c) 2017-2018 Clicked, Inc.

  Licensed under the MIT license found in the LICENSE file 
  in the root folder of the project.

 ***********************************************************/

using UnityEngine;
using UnityEngine.UI;

public class AirVRClientAutoPlay : MonoBehaviour
{
    [SerializeField] private Toggle _toggle;
    [SerializeField] private Image _guage;
    [SerializeField] private float _delay = 0f;
    [SerializeField] private float _duration = 3f;

    public Toggle Toggle { get { return _toggle; } }

    private float _elapsedTime = 0f;
    private bool _canAutoPlay;

    private void Awake()
    {
        if (_toggle == null)
            _toggle = GetComponent<Toggle>();
    }

    private void Update()
    {
        _canAutoPlay = _toggle.isOn && !AirVRClientAppManager.Instance.IsConnecting && AirVRClientUIManager.Instance.CanvasGroup.interactable && AirVRClientUIManager.Instance.SettingPanel.CanvasGroup.interactable;

#if UNITY_ANDROID && !UNITY_EDITOR
        _canAutoPlay = _canAutoPlay && OVRManager.instance.isUserPresent && !AirVRClient.connected;
#endif

        if (_canAutoPlay)
        {
            if (_elapsedTime >= _delay)
            {
                if ((_elapsedTime - _delay) >= _duration)
                {
                    AirVRClientUIManager.Instance.SettingPanel.OnPlayClicked();
                    _elapsedTime = 0f;
                    _guage.fillAmount = 0f;
                }
                _guage.fillAmount = (_elapsedTime - _delay) / _duration;
            }
            
            _elapsedTime += Time.deltaTime;            
        }

        else
        {
            _elapsedTime = 0f;
            _guage.fillAmount = 0f;
        }
    }

    public void OnToggle(bool value)
    {
        if (value)
            _elapsedTime = _delay;
    }
}
