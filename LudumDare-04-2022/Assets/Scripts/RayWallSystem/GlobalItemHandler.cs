using System;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

namespace RayWallSystem
{
    public class GlobalItemHandler : MonoBehaviour
    {
        [SerializeField] private float timeInSeconds = 1f;

        public static GlobalItemHandler Instance { get; private set; }
        [HideInInspector] [NonSerialized] public GameObject prefab = null;
        public GameObject initialPrefab;
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
            if (Input.GetMouseButtonDown(0) && HandleInstantiation != null && HandleInstantiation.ClickAllowed())
            {
                var activePrefab = prefab != null ? prefab : initialPrefab;
                if (activePrefab == null) return;
                var instance = Instantiate(activePrefab);
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