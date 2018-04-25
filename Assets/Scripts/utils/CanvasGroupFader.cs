/***********************************************************

  Copyright (c) 2017-2018 Clicked, Inc.

  Licensed under the MIT license found in the LICENSE file 
  in the root folder of the project.

 ***********************************************************/

using System;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class CanvasGroupFader : MonoBehaviour
{ 
    public event Action OnFadeInStart;
    public event Action OnFadeOutStart;
    public event Action OnFadeInComplete;                  
    public event Action OnFadeOutComplete;                 

    [SerializeField] private CanvasGroup[] groupsToFade;

    public bool Fading { get; private set; }
    public bool Visible { get; private set; }

    public void FadeIn(float duration = 1f, float delay = 0f, bool waitForFading = false)
    {
        StartCoroutine(FadeInRoutine(duration, delay, waitForFading));
    }

    public void InteruptAndFadeIn(float duration = 1f, float delay = 0f)
    {
        StopAllCoroutines();
        StartCoroutine(FadeInRoutine(duration, delay, false));
    }

    private IEnumerator FadeInRoutine(float duration, float delay, bool waitForFading)
    {
        if (waitForFading)
        {
            while (Fading)
            {
                yield return null;
            }
        }

        if (delay > 0)
            yield return new WaitForSeconds(delay);

        Fading = true;

        float lowestAlpha;

        if (OnFadeInStart != null)
            OnFadeInStart();

        do
        {
            lowestAlpha = 1f;

            for (int i = 0; i < groupsToFade.Length; i++)
            {
                groupsToFade[i].alpha += Time.deltaTime / duration;

                if (groupsToFade[i].alpha < lowestAlpha)
                    lowestAlpha = groupsToFade[i].alpha;
            }

            yield return null;
        }
        while (lowestAlpha < 1f);

        if (OnFadeInComplete != null)
            OnFadeInComplete();

        Fading = false;

        Visible = true;
    }

    public void FadeOut(float duration = 1f, float delay = 0f, bool waitForFading = false)
    {
        StartCoroutine(FadeOutRoutine(duration, delay, waitForFading));
    }

    public void InteruptAndFadeOut(float duration = 1f, float delay = 0f)
    {
        StopAllCoroutines();
        StartCoroutine(FadeOutRoutine(duration, delay, false));
    }

    private IEnumerator FadeOutRoutine(float duration, float delay, bool waitForFading)
    {
        if (waitForFading)
        {
            while (Fading)
            {
                yield return null;
            }
        }

        if (delay > 0)
            yield return new WaitForSeconds(delay);

        Fading = true;

        float highestAlpha;

        if (OnFadeOutStart != null)
            OnFadeOutStart();

        do
        {
            highestAlpha = 0f;

            for (int i = 0; i < groupsToFade.Length; i++)
            {
                groupsToFade[i].alpha -= Time.deltaTime / duration;

                if (groupsToFade[i].alpha > highestAlpha)
                    highestAlpha = groupsToFade[i].alpha;
            }

            yield return null;
        }
        while (highestAlpha > 0f);

        if (OnFadeOutComplete != null)
            OnFadeOutComplete();

        Fading = false;

        Visible = false;
    }


    public void SetVisible()
    {
        for (int i = 0; i < groupsToFade.Length; i++)
        {
            groupsToFade[i].alpha = 1f;
        }

        Visible = true;
    }


    public void SetInvisible()
    {
        for (int i = 0; i < groupsToFade.Length; i++)
        {
            groupsToFade[i].alpha = 0f;
        }

        Visible = false;
    }
}