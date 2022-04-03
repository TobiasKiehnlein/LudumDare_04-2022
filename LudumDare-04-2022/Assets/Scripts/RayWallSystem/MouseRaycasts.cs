using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class MouseRaycasts : MonoBehaviour
{
    [SerializeField] private float timeInSeconds = 1f;
    [SerializeField] private GameObject prefab;

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var instance = Instantiate(prefab);
            instance.GetComponent<RayWall>().timeInSeconds = timeInSeconds;
        }
    }
}