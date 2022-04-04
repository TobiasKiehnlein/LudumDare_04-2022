using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[Serializable]
public struct AudioEntry
{
    public AudioClip AudioClip;
    public AudioMixerGroup AudioMixerGroup;
}

public class CreateAudioSources : MonoBehaviour
{
    [SerializeField] private List<AudioEntry> audioEntries;

    private void Awake()
    {
        foreach (var audioEntry in audioEntries)
        {
            var audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = audioEntry.AudioClip;
            audioSource.loop = true;
            audioSource.outputAudioMixerGroup = audioEntry.AudioMixerGroup;
            audioSource.dopplerLevel = 0;
            audioSource.Play();
        }
    }
}