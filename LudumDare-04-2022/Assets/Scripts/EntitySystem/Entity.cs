using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Utils;
using WallSystem;

namespace EntitySystem
{
    public abstract class Entity : MonoBehaviour
    {
        [SerializeField] private GameObject spriteContainer;
        [SerializeField] private float nearbyRadius = 10f;

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
            DistanceHandler.Instance.Register(this);
            
            if (!this.gameObject.IsInLayerMask(EntityMask))
            {
                Debug.LogWarning($"The Entity {this.name} does not have the Entity layer set.");
            }
            
            if (spriteContainer.GetComponent<SpriteRenderer>() != null)
            {
                Debug.LogWarning($"The SpriteContainer of Entity {this.name} directly has a SpriteRenderer as Component. This may lead to visual bugs. Put the Renderer in a child Gameobject");
            }
            if (spriteContainer.transform.localPosition != Vector3.zero || spriteContainer.transform.localScale != Vector3.one || spriteContainer.transform.eulerAngles != Vector3.zero) {
                Debug.LogWarning($"The SpriteContainer if Entity {this.name} has the wrong Transform. This may lead to visual bugs.");
            }

            CameraObject = GameObject.FindGameObjectWithTag("MainCamera");
            if (CameraObject == null)
            {
                Debug.LogError("Camera not found. Needs to have 'MainCamera' tag.");
            }
        }

        private void OnDestroy()
        {
            DistanceHandler.Instance.Register(this);
        }

        /*
        private void SetupTriggers()
        {
            bool hasTrigger = false;
            bool hasCollider = false;
            bool hasRigidbody = false; // Rigidbody is needed for Triggers to work. Should be used only in kinematic mode.

            foreach ( var colliderComponent in GetComponents<Collider2D>())
            {
                if (colliderComponent.isTrigger) hasTrigger = true;
                else hasCollider = true;
            }

            if (GetComponent<Rigidbody2D>() != null)
            {
                hasRigidbody = true;
                Debug.LogWarning($"The Entity {this.name} already has a Rigidbody. This is not recommended.");
            }

            if (!hasTrigger)
            {
                if (defaultNearbyRadius == 0)
                {
                    Debug.Log($"The Entity {this.name} has no trigger for nearby Entities and defaultNearbyRadius == 0. It will not be able to detect other Entities.");
                }
                else
                {
                    var newTrigger = this.AddComponent<CircleCollider2D>();
                    newTrigger.radius = defaultNearbyRadius;
                    newTrigger.isTrigger = true;
                }
            }
            else
            {
                Debug.Log($"The Entity {this.name} already has a trigger for nearby Entities. Ignoring defaultNearbyRadius.");
            }

            if (!hasRigidbody && (hasTrigger || (defaultNearbyRadius > 0)))
            {
                var newRigidbody = this.AddComponent<Rigidbody2D>();
                newRigidbody.isKinematic = true;
            }

            if (!hasCollider)
            {
                if (defaultColliderRadius == 0)
                {
                    Debug.Log($"The Entity {this.name} has no (non trigger) Collider and defaultColliderRadius == 0. It will not be detected by other Entities but can detect Entities itself.");
                }
                else
                {
                    var newCollider = this.AddComponent<CircleCollider2D>();
                    newCollider.radius = defaultColliderRadius;
                    newCollider.isTrigger = false;
                }
            }
            else
            {
                Debug.Log($"The Entity {this.name} already has a (non trigger) Collider. Ignoring defaultColliderRadius.");
            }
        }*/

        protected virtual void Update()
        {
            spriteContainer.transform.up = CameraObject.transform.forward;
            if (!Dead)
            {
                var distances = DistanceHandler.Instance.GetDistancesFor(this).Where(value => value.Value <= nearbyRadius && value.Key != this);
                foreach (var (entity, distance) in distances)
                {
                    this.HandleNearbyEntity(entity, distance);
                }
            }
        }

        protected abstract void OnDeath();

        public void Die()
        {
            Dead = true;
            OnDeath();
        }

        protected abstract void HandleNearbyEntity(Entity e, float distance);
    }
}