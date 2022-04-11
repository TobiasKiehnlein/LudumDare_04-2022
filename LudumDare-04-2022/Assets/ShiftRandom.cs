using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ShiftRandom : MonoBehaviour
{
	[SerializeField] private float strength;
	[SerializeField] private Vector2 direction = Vector2.right;
    [SerializeField] private float speed;
    
    private float _offset;
    private Vector2 _initialPosition;
    private RectTransform _transform;

    private void Start()
    {
	    _offset = Random.value * 2 * (float) Math.PI - (float) Math.PI;
	    _transform = GetComponent<RectTransform>();
	    _initialPosition = _transform.anchoredPosition;
    }

    // Update is called once per frame
    void Update()
    {
        var newPos = _initialPosition + direction * (float) Math.Sin(Time.time * speed + _offset) * strength;
        _transform.anchoredPosition = newPos;
    }
}
