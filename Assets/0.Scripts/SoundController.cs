using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController: MonoBehaviour
{
    public static SoundController instance;
    public AudioSource audioSource;
    public AudioClip hitAudio, behitAudio, jumpAudio;

    private void Awake()
    {
        instance = this;
    }
    public void HitAudio()
    {
        audioSource.clip = hitAudio;
        audioSource.Play();
    }
    public void JumpAudio()
    {
        audioSource.clip = jumpAudio;
        audioSource.Play();
    }
    public void BehitAudio()
    {
        audioSource.clip = behitAudio;
        audioSource.Play();
    }

}