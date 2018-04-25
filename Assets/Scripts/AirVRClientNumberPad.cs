/***********************************************************

  Copyright (c) 2017-2018 Clicked, Inc.

  Licensed under the MIT license found in the LICENSE file 
  in the root folder of the project.

 ***********************************************************/

using System.Collections;
using UnityEngine;

public class AirVRClientNumberPad : MonoBehaviour
{
    [SerializeField] private AnimationCurve _showAnimationCurve;
    [SerializeField] private AnimationCurve _hideAnimationCurve;
    [SerializeField] private Vector3 _startPos;
    [SerializeField] private Vector3 _endPos;

    private CanvasGroup _canvasGroup;
    private float _showDuration;
    private float _hideDuration;

    public enum Key
    {
        Num0 = 0,
        Num1,
        Num2,
        Num3,
        Num4,
        Num5,
        Num6,
        Num7,
        Num8,
        Num9,
        Dot,
        Del
    }

    public delegate void KeyClickHandler(AirVRClientNumberPad numberPad, Key key);
    public event KeyClickHandler KeyClicked;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();

        AirVRClientNumberPadKey[] keys = GetComponentsInChildren<AirVRClientNumberPadKey>();
        foreach (AirVRClientNumberPadKey key in keys)
        {
            key.owner = this;
        }        

        _showDuration = _showAnimationCurve.keys[_showAnimationCurve.length - 1].time;
        _hideDuration = _hideAnimationCurve.keys[_hideAnimationCurve.length - 1].time;

        if (gameObject.activeSelf)
            gameObject.SetActive(false);
    }

    // handle AirVRClientNumberPadKey events
    public void OnKeyClicked(AirVRClientNumberPadKey key)
    {
        if (KeyClicked != null)
        {
            KeyClicked(this, key.key);
        }
    }

    public void Show()
    {
        if (!gameObject.activeSelf)
            gameObject.SetActive(true);

        StopAllCoroutines();
        StartCoroutine(ShowRoutine());
    }

    public void Hide()
    {
        StopAllCoroutines();
        StartCoroutine(HideRoutine());
    }

    private IEnumerator ShowRoutine()
    {
        float elapsedTime = 0f;        
        while (elapsedTime < _showDuration)
        {            
            _canvasGroup.alpha = _showAnimationCurve.Evaluate(elapsedTime);
            transform.position = _startPos + (_endPos - _startPos) * _showAnimationCurve.Evaluate(elapsedTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.interactable = true;
        yield return null;
    }

    private IEnumerator HideRoutine()
    {
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.interactable = false;
        yield return null;

        float elapsedTime = 0f;
        while (elapsedTime < _hideDuration)
        {
            _canvasGroup.alpha = _hideAnimationCurve.Evaluate(elapsedTime);
            transform.position = _startPos + (_endPos - _startPos) * _hideAnimationCurve.Evaluate(elapsedTime);
            transform.localScale = Vector3.one * (0.8f + 0.2f * _hideAnimationCurve.Evaluate(elapsedTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (gameObject.activeSelf)
            gameObject.SetActive(false);

        transform.localScale = Vector3.one;
    }
}
