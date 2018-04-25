/***********************************************************

  Copyright (c) 2017-2018 Clicked, Inc.

  Licensed under the MIT license found in the LICENSE file 
  in the root folder of the project.

 ***********************************************************/

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class AirVRClientNotification : MonoBehaviour
{        
    [SerializeField] private Text _text;
    [SerializeField] private Color _normalColor = Color.black;
    [SerializeField] private Color _errorColor = Color.red;
    private AudioSource _audioSource;
    private bool _isDisplaying;
    private Coroutine _coroutine;

    private void Awake()
    {
        if (_text == null)
            _text = GetComponent<Text>();

        _audioSource = GetComponent<AudioSource>();

        _text.text = string.Empty;
        _isDisplaying = false;
    }

    private IEnumerator DisplayRoutine(string message, float duration, Color color)
    {
        if(_isDisplaying)
            yield break;
        
        _isDisplaying = true;

        _text.color = color;
        _text.text = message;

        yield return new WaitForSeconds(duration);

        _text.text = string.Empty;
        _isDisplaying = false;
        _coroutine = null;
    }

    public void DisplayError(string message)
    {        
        Display(message, _errorColor);
    }

    public void Display(string message)
    {
        Display(message, _normalColor);
    }

    public void Display(string message, Color color)
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _isDisplaying = false;
        }

        AirVRClientSoundManager.Play(AirVRClientSoundManager.Instance.Error, _audioSource);
        _coroutine = StartCoroutine(DisplayRoutine(message, 2f, color));
    }

    public void DisplayConnecting()
    {        
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _isDisplaying = false;
        }

        _coroutine = StartCoroutine(DisplayConnectingRoutine());
    }
    
    // 데스크탑에서는 Connected, Disconnected 이벤트가 발생하지 않아, 무한 반복 됨.
    private IEnumerator DisplayConnectingRoutine()
    {
        float elapsedTime = 0.5f;
        int cnt = 0;
        _isDisplaying = true;

        _text.color = _normalColor;

        while (AirVRClientAppManager.Instance.IsConnecting)
        {
            elapsedTime += Time.deltaTime;            

            if (elapsedTime >= 0.5f)
            {                
                string message = "Connecting";
                for (int i = 0; i < cnt; i++)
                {
                    message += ".";
                }
                
                _text.text = message;

                cnt = (cnt + 1) % 4;

                elapsedTime = 0f;
            }
            yield return new WaitForEndOfFrame();
        }

        _text.text = string.Empty;
        _isDisplaying = false;
        _coroutine = null;
    }
}
