using System.Collections.Generic;
using UnityEngine;
using Utils;
using WallSystem;

namespace EntitySystem
{
    public abstract class Entity : MonoBehaviour
    {
        private SortedSet<Entity> _nearbyEntities;
        [SerializeField] private GameObject spriteContainer;

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
            if (spriteContainer.GetComponent<SpriteRenderer>() != null)
            {
                Debug.LogWarning($"The SpriteContainer of Entity {this.name} directly has a SpriteRenderer as Component. This may lead to visual bugs. Put the Renderer in a child Gameobject");
            }
            if (spriteContainer.transform.localPosition != Vector3.zero || spriteContainer.transform.localScale != Vector3.one || spriteContainer.transform.eulerAngles != Vector3.zero) {
                Debug.LogWarning($"The SpriteContainer if Entity {this.name} has the wrong Transform. This may lead to visual bugs.");
            }

            _nearbyEntities = new();
            CameraObject = GameObject.FindGameObjectWithTag("MainCamera");
            if (CameraObject == null)
            {
                Debug.LogError("Camera not found. Needs to have 'MainCamera' tag.");
            }
        }

        protected virtual void Update()
        {
            spriteContainer.transform.up = CameraObject.transform.forward;
            if (!Dead)
            {
                foreach (var nearbyEntity in _nearbyEntities)
                {
                    this.HandleNearbyEntity(nearbyEntity);
                }
            }
        }

        protected abstract void OnDeath();

        public void Die()
        {
            Dead = true;
            OnDeath();
        }

        protected abstract void HandleNearbyEntity(Entity e);

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.IsInLayerMask(EntityMask))
            {
                var entity = other.gameObject.GetComponentInChildren<Entity>();

                if (entity != null)
                {
                    if (entity.Dead)
                    {
                        Debug.Log("Ignoring dead Entity");
                    }
                    else
                    {
                        _nearbyEntities.Add(entity);
                    }
                }
                else
                {
                    Debug.LogWarning($"Trigger {other.gameObject.name} on Entity layer but without Entity component detected.");
                }
            }
            else if (!other.gameObject.IsInLayerMask(Wall.WallMask))
            {
                Debug.LogWarning($"Trigger {other.gameObject.name} without proper layer detected.");
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.IsInLayerMask(EntityMask))
            {
                var entity = other.gameObject.GetComponentInChildren<Entity>();

                if (entity != null)
                {
                    _nearbyEntities.Remove(entity);
                }
            }
        }
    }
}