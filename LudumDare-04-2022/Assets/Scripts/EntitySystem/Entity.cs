using System;
using System.Linq;
using UnityEngine;
using Utils;

namespace EntitySystem
{
    public abstract class Entity : MonoBehaviour
    {
        [SerializeField] private GameObject spriteContainer;
        [SerializeField] private bool handleNearby = false;
        private float _lastNearbyUpdate = 0;
        private DistanceHandler.DistanceInformation[] _distanceInformations = null;

        protected const float NearbyRadius = 30f;
        protected const float HighDistance = NearbyRadius - 5f;
        protected const float MedDistance = HighDistance / 2f;
        protected const float LowDistance = 5f;
        protected const float CollisionDistance = 1f;
        public readonly Type type;

        public Entity(Type t)
        {
            type = t;
        }

        [Serializable]
        public enum Type
        {
            Death,
            Human,
            Cross,
            Totem,
            Obstacle,
            VisualOnly,
        }

        public struct DistanceInformation
        {
            public readonly float Distance;

            // Fractions are 1 if distance is 0, get smaller if further away
            public readonly float MaxDistanceFraction;
            public readonly float HighDistanceFraction;
            public readonly float MedDistanceFraction;
            public readonly float LowDistanceFraction;

            public readonly bool IsHighDistance;
            public readonly bool IsMedDistance;
            public readonly bool IsLowDistance;
            public readonly bool IsCollision;

            public DistanceInformation(float distance)
            {
                this.Distance = distance;
                MaxDistanceFraction = 1.0f - Mathf.Min(distance, NearbyRadius) / NearbyRadius;
                HighDistanceFraction = 1.0f - Mathf.Min(distance, HighDistance) / HighDistance;
                IsHighDistance = (distance <= HighDistance);
                MedDistanceFraction = 1.0f - Mathf.Min(distance, MedDistance) / MedDistance;
                IsMedDistance = (distance <= MedDistance);
                LowDistanceFraction = 1.0f - Mathf.Min(distance, LowDistance) / LowDistance;
                IsLowDistance = (distance <= LowDistance);
                IsCollision = (distance <= CollisionDistance);
            }
        }

        protected GameObject CameraObject;

        public static int EntityMask
        {
            get
            {
                if (_entityMask < 0)
                {
                    string[] entityLayers = {"Entity"};
                    _entityMask = LayerMask.GetMask(entityLayers);
                }

                return _entityMask;
            }
        }

        private static int _entityMask = -1;
        public bool Dead { get; private set; } = false;

        protected virtual void Start()
        {
            if (DistanceHandler.Instance == null)
            {
                Debug.LogError("DistanceHandler missing");
                Destroy(this);
                return;
            }

            DistanceHandler.Instance.Register(this);

            if (!this.gameObject.IsInLayerMask(EntityMask))
            {
                Debug.LogWarning($"The Entity {this.name} does not have the Entity layer set.");
            }

            if (spriteContainer.GetComponent<SpriteRenderer>() != null)
            {
                Debug.LogWarning(
                    $"The SpriteContainer of Entity {this.name} directly has a SpriteRenderer as Component. This may lead to visual bugs. Put the Renderer in a child Gameobject");
            }

            if (spriteContainer.transform.localPosition != Vector3.zero ||
                spriteContainer.transform.localScale != Vector3.one ||
                spriteContainer.transform.eulerAngles != Vector3.zero)
            {
                Debug.LogWarning(
                    $"The SpriteContainer if Entity {this.name} has the wrong Transform. This may lead to visual bugs.");
            }

            CameraObject = GameObject.FindGameObjectWithTag("MainCamera");
            if (CameraObject == null)
            {
                Debug.LogError("Camera not found. Needs to have 'MainCamera' tag.");
            }
            
            _distanceInformations = new DistanceHandler.DistanceInformation[] {};
        }

        private void OnDestroy()
        {
            DistanceHandler.Instance.UnRegister(this);
        }

        protected virtual void Update()
        {
            spriteContainer.transform.up = CameraObject.transform.forward;
            var lastUpdate = DistanceHandler.Instance.LastUpdate;
            if (!Dead && handleNearby)
            {
                if (lastUpdate > _lastNearbyUpdate)
                {
                    _lastNearbyUpdate = lastUpdate;
                    _distanceInformations = DistanceHandler.Instance.GetDistancesFor(this);
                }

                foreach (var info in _distanceInformations)
                {
                    if (info.Distance <= NearbyRadius && info.Entity != this)
                    {
                        this.HandleNearbyEntity(info.Entity, new DistanceInformation(info.Distance));
                    }
                }

                DebugDrawNearby();
            }
        }

        private void DebugDrawNearby()
        {
            var pos = gameObject.transform.position;
            Debug.DrawRay(pos, Vector3.left * NearbyRadius, Color.blue);
            Debug.DrawRay(pos, Vector3.right * NearbyRadius, Color.blue);
            Debug.DrawRay(pos, Vector3.up * NearbyRadius, Color.blue);
            Debug.DrawRay(pos, Vector3.down * NearbyRadius, Color.blue);
        }

        protected virtual void FixedUpdate()
        {
        }

        protected abstract void OnDeath();

        public void Kill()
        {
            Dead = true;
            OnDeath();
        }

        protected abstract void HandleNearbyEntity(Entity e, DistanceInformation distInfo);
    }
}