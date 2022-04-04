using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ShiftRandom : MonoBehaviour
{
    [SerializeField] private Vector3 direction;
    [SerializeField] private float strength;
    [SerializeField] private float speed;

    private Vector3 _initialPosition;
    private float _offset;

    private void Start()
    {
        _initialPosition = transform.position;
        _offset = Random.value * 2 * (float) Math.PI - (float) Math.PI;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = _initialPosition + direction * (float) Math.Sin(Time.time * speed + _offset) * strength;
    }
}