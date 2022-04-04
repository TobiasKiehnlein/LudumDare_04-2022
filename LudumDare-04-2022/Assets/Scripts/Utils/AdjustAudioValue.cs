using System;
using System.Collections;
using System.Collections.Generic;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

public class AdjustAudioValue : MonoBehaviour
{
    [SerializeField] private GameSettings settings;
    [SerializeField] private bool isMusic;
    private Slider _slider;

    private void Start()
    {
        _slider = GetComponent<Slider>();
    }

    private void Update()
    {
        _slider.value = isMusic ? settings.MusicVolume : settings.SfxVolume;
    }
}