using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class RayWall : MonoBehaviour
{
    public float timeInSeconds = 1f;
    private bool _previousState;
    private ParticleSystem _particleSystem;
    private EdgeCollider2D _edgeCollider;
    private float _timeEnabled = float.MaxValue;

    // Start is called before the first frame update
    private void Start()
    {
        // go.SetActive(false);
        _edgeCollider = GetComponentInChildren<EdgeCollider2D>();
        _particleSystem = GetComponentInChildren<ParticleSystem>();

        Debug.Assert(Camera.main != null, "Camera.main != null");
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out var hit, 500.0f)) return;

        if (hit.transform == null) return;
        var target = hit.point + Vector3.back * .1f;
        _edgeCollider.points = new Vector2[] {target, target};

        HandleEmissionChange(false);
    }

    private void FixedUpdate()
    {
#if UNITY_EDITOR
        if (_edgeCollider.enabled)
            for (var i = 0; i < _edgeCollider.points.Length - 1; i++)
            {
                var points = _edgeCollider.points;
                Debug.DrawRay(points[i], points[i + 1] - points[i], Color.yellow, Time.fixedDeltaTime);
            }
#endif
    }

    // Update is called once per frame
    private void Update()
    {
        if (!Input.GetMouseButton(0))
        {
            HandleEmissionChange(false);
            return;
        }

        HandleEmissionChange(!Input.GetMouseButtonDown(0));
    }

    void HandleEmissionChange(bool active)
    {
        if (!active || timeInSeconds + _timeEnabled < Time.time)
        {
            _edgeCollider.enabled = false;
            _particleSystem.enableEmission = false;
        }
        else
        {
            if (!_previousState)
            {
                _timeEnabled = Time.time;
                StartCoroutine(StopIt());
            }
            else
            {
                _particleSystem.enableEmission = true;
                _edgeCollider.enabled = true;
            }

            Debug.Assert(Camera.main != null, "Camera.main != null");
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out var hit, 500.0f)) return;

            if (hit.transform == null) return;
            var target = hit.point + Vector3.back * .1f;
            _particleSystem.gameObject.transform.position = target;
            _edgeCollider.points = _edgeCollider.points.Append(target).ToArray();
        }

        _previousState = active;
    }

    private IEnumerator StopIt()
    {
        yield return new WaitForSeconds(timeInSeconds);
        HandleEmissionChange(false);
        yield return new WaitForSeconds(timeInSeconds);
        Destroy(gameObject);
    }
}