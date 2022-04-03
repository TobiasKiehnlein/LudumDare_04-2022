﻿using System;
using System.Collections.Generic;
using System.Linq;
using Enums;
using UnityEngine;
using UnityEngine.Audio;

namespace Utils
{
    public class SfxHandler : MonoBehaviour
    {
        public static SfxHandler Instance;
    
        [Serializable]
        class ClipInfo
        {
            public string Name;
            public Sfx sfx;
            public AudioClip Clip;
            public bool shouldLoop;
            [NonSerialized] public AudioSource AudioSource;
        }

        [SerializeField] private List<ClipInfo> sfxClips;
        [SerializeField] private AudioMixerGroup mixerGroup;

        // Start is called before the first frame update
        void Start()
        {
            if (Instance != null)
            {
                DestroyImmediate(gameObject);
            }

            Instance = this;
        
            foreach (var clipInfo in sfxClips)
            {
                var audioSource = gameObject.AddComponent<AudioSource>();
                clipInfo.AudioSource = audioSource;
                audioSource.clip = clipInfo.Clip;
                audioSource.outputAudioMixerGroup = mixerGroup;
            }
        }

        public void TriggerSfx(Sfx sfx)
        {
            var audioSrc = sfxClips.FirstOrDefault(x => x.sfx == sfx)?.AudioSource;
            if (!audioSrc) return;
            audioSrc.Play();
            audioSrc.time = 0;
        }

        public void StopSfx(Sfx sfx)
        {
            sfxClips.FirstOrDefault(x => x.sfx == sfx)?.AudioSource.Stop();
        }
    }
}