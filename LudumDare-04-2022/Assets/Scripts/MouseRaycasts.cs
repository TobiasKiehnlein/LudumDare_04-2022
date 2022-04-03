using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

public class MouseRaycasts : MonoBehaviour
{
    [SerializeField] private GameObject go;
    private ParticleSystem _particleSystem;

    // Start is called before the first frame update
    private void Start()
    {
        // go.SetActive(false);
        _particleSystem = go.GetComponent<ParticleSystem>();
        _particleSystem.Pause();

    }

    // Update is called once per frame
    private void Update()
    {
        if (!Input.GetMouseButton(0))
        {
            _particleSystem.Pause();
            return;
        }

        Debug.Assert(Camera.main != null, "Camera.main != null");
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out var hit, 100.0f)) return;

        if (hit.transform == null) return;
        go.transform.position = hit.point + Vector3.back * .1f;
        _particleSystem.Play();
    }
}