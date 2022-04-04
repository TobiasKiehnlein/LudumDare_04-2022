using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace RayWallSystem
{
    internal class Point
    {
        public Guid ID { get; } = Guid.NewGuid();
        public float X { get; set; }
        public float Y { get; set; }
    }

    public class RayWall : MonoBehaviour
    {
        public float timeInSeconds = 3f;
        private bool _previousState;
        private ParticleSystem _particleSystem;
        private EdgeCollider2D _edgeCollider;
        private float _timeEnabled = float.MaxValue;
        private bool done;
        private IEnumerable<Point> _points = new List<Point>();


        // Start is called before the first frame update
        private void Start()
        {
            // go.SetActive(false);
            _edgeCollider = GetComponentInChildren<EdgeCollider2D>();
            _particleSystem = GetComponentInChildren<ParticleSystem>();

            Debug.Assert(Camera.main != null, "Camera.main != null");
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out var hit, 500.0f)) return;

            if (hit.transform == null)
            {
                Debug.LogWarning("No target for start");
                return;
            }

            var target = hit.point - gameObject.transform.position;
            _edgeCollider.points = new Vector2[] {target, target};

            _timeEnabled = Time.time;
            StartCoroutine(StopIt());
        }

        private void FixedUpdate()
        {
#if UNITY_EDITOR
            if (_edgeCollider.enabled)
            {
                var points = _edgeCollider.points.Select(x => x + (Vector2) transform.position).ToArray();
                for (var i = 0; i < _edgeCollider.points.Length - 1; i++)
                {
                    Debug.DrawRay(points[i], points[i + 1] - points[i], Color.yellow, Time.fixedDeltaTime);
                }
            }
#endif
        }

        // Update is called once per frame
        private void Update()
        {
            _edgeCollider.points = _points.Select(p => new Vector2(p.X, p.Y)).ToArray();
            if (Input.GetMouseButtonUp(0)) done = true;
            if (done) return;
            HandleEmissionChange(true);
        }

        void HandleEmissionChange(bool active)
        {
            if (!active || timeInSeconds + _timeEnabled < Time.time)
            {
                // _edgeCollider.enabled = false;
                // _particleSystem.enableEmission = false;
            }
            else
            {
                _particleSystem.enableEmission = true;
                _edgeCollider.enabled = true;

                Debug.Assert(Camera.main != null, "Camera.main != null");
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (!Physics.Raycast(ray, out var hit, 500.0f)) return;

                if (hit.transform == null) return;
                var target = hit.point + Vector3.back * .1f - gameObject.transform.position;
                _particleSystem.gameObject.transform.position = target + transform.position;
                var point = new Point() {X = target.x, Y = target.y};
                _points = _points.Append(point);
                StartCoroutine(RemovePoint(point));
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

        private IEnumerator RemovePoint(Point p)
        {
            yield return new WaitForSeconds(timeInSeconds);
            _points = _points.Where(p1 => p1.ID != p.ID);
        }
    }
}