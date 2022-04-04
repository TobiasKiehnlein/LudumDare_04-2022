using System;
using Unity.VisualScripting;
using UnityEngine;

namespace RayWallSystem
{
    public class GlobalItemHandler : MonoBehaviour
    {
        [SerializeField] private float timeInSeconds = 1f;
        [SerializeField] private GameObject defaultPrefab;

        public static GlobalItemHandler Instance { get; private set; }
        [HideInInspector] public GameObject prefab;
        public IHandleInstantiation HandleInstantiation;

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


        // Update is called once per frame
        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                var instance = Instantiate(prefab != null ? prefab : defaultPrefab);
                var rayWall = instance.GetComponent<RayWall>();
                if (rayWall != null)
                    rayWall.timeInSeconds = timeInSeconds;

                Debug.Assert(Camera.main != null, "Camera.main != null");
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (!Physics.Raycast(ray, out var hit, 500.0f)) return;

                if (hit.transform == null)
                {
                    Debug.LogWarning("No target for start");
                    return;
                }

                var target = hit.point;
                instance.transform.position = hit.point;

                HandleInstantiation.HandleInstantiation();
            }
        }
    }
}