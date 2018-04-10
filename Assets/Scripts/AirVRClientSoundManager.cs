/***********************************************************

  Copyright (c) 2017-2018 Clicked, Inc.

  Licensed under the MIT license found in the LICENSE file 
  in the root folder of the project.

 ***********************************************************/

using UnityEngine;

public class AirVRClientSoundManager : Singleton<AirVRClientSoundManager>
{
    [Header("Audio Clips")]
    public AudioClip Error;

    public static void Play(AudioClip audioClip, AudioSource audioSource)
    {
        audioSource.clip = audioClip;
        audioSource.Play();
    }

    public static void TryPlay(AudioClip audioClip, AudioSource audioSource, bool interrupt = false)
    {
        if (audioSource.isPlaying)
        {
            if (interrupt)
                Play(audioClip, audioSource);

            return;
        }

        Play(audioClip, audioSource);
    }

    public static void Stop(AudioSource audioSource)
    {
        audioSource.Stop();        
    }    
}
