using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class VolumeHandler : MonoBehaviour
{
    private Volume _volume;
    private float _influence = 0;

    // Start is called before the first frame update
    void Start()
    {
        _volume = GetComponent<Volume>();
    }

    private void Update()
    {
        _volume.weight = Mathf.Lerp(_volume.weight, _influence, Time.deltaTime * 2f);
    }

    public void SetInfluence(float influence)
    {
        _influence = influence;
    }
}