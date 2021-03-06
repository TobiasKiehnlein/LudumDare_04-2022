using System;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RayWallSystem
{
    public class GlobalItemHandler : MonoBehaviour
    {
        [SerializeField] private float timeInSeconds = 1f;

        public static GlobalItemHandler Instance { get; private set; }
        [HideInInspector] [NonSerialized] public GameObject prefab = null;
        public GameObject initialPrefab;
        private IHandleInstantiation _handleInstantiation;

        public IHandleInstantiation HandleInstantiation
        {
            get => _handleInstantiation;
            set
            {
                _handleInstantiation?.Deactivate();
                _handleInstantiation = value;
                _handleInstantiation.Activate();
            }
        }

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
            if (Input.GetMouseButtonDown(0) && HandleInstantiation != null && HandleInstantiation.ClickAllowed() && !EventSystem.current.IsPointerOverGameObject())
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