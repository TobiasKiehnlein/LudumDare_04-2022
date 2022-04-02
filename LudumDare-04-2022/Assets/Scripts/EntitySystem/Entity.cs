using System.Collections.Generic;
using UnityEngine;
using WallSystem;

namespace EntitySystem
{
    public abstract class Entity : MonoBehaviour
    {
        private SortedSet<Entity> _nearbyEntities;
        private GameObject _sprite;
        public static int EntityMask { get; private set; }
        public bool Dead { get; private set; } = false;
        
        void Start()
        {
            _sprite = this.GetComponentInChildren<SpriteRenderer>().gameObject;
            if (_sprite == null)
            {
                Debug.LogWarning($"Entity {this.name} has no child containing a SpriteRenderer");
            }
        }

        static Entity()
        {
            string[] entityLayers = {"Entity"};
            EntityMask = LayerMask.GetMask(entityLayers);
        }
        
        protected virtual void Update()
        {
            if (_sprite != null)
            {
                _sprite.transform.forward = this.transform.forward;
            }
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