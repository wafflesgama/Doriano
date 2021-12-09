using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class StartLoopRandomTime : MonoBehaviour
{
    AudioSource audioSource;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.time = Random.Range(0, audioSource.clip.length);
        
        if (!audioSource.playOnAwake)
            audioSource.Play();
    }
}
