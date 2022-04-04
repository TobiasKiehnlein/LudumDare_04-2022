using System;
using System.Collections;
using System.Collections.Generic;
using Enums;
using UnityEngine;
using Utils;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        AudioManager.Instance.StartSound(Music.Major,3f);
    }
}
