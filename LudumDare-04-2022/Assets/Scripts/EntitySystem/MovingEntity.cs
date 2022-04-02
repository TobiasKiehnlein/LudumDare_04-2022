using UnityEngine;
using WallSystem;

namespace EntitySystem
{
    public abstract class MovingEntity : Entity
    {
        public Vector2 Direction
        {
            get => _direction;
            protected set
            {
                _direction = value;
                OnUpdateDirection();
            }
        }

        private Vector2 _direction;

        public float Speed { get; protected set; }

        protected abstract void OnUpdateDirection();
        
        // When this is called the Direction has already been updated
        // Note: Wall.Hit will automatically be called after this
        protected abstract void OnWallHit(Wall w);

        // Uses direction and speed to update the position of the gameobject, including Wall detection
        private void UpdatePosition()
        {
            float distance; // The distance to move in this frame
            float hitDeltaTime = 0; // The time at which the Wall is hit
            Vector2 translation = Vector2.zero;
            RaycastHit2D hit;
            do
            {
                distance = Speed * (Time.deltaTime - hitDeltaTime);
                hit = Physics2D.Raycast(gameObject.transform.position, Direction, distance, EntityMask);
                if (hit.collider != null)
                {
                    translation = translation + Direction * hit.distance;
                    hitDeltaTime = hitDeltaTime + hit.distance / Speed;
                    Direction = Vector2.Reflect(Direction, hit.normal);
                    var wall = hit.collider.gameObject.GetComponentInChildren<Wall>();
                    if (wall != null)
                    {
                        OnWallHit(wall); // Might update the Speed
                        wall.Hit(this);
                    }
                    else
                    {
                        Debug.LogWarning($"Collider {hit.collider.gameObject.name} on Wall layer but without Wall component detected.");
                    }
                }
                else
                {
                    translation = translation + Direction * distance;
                }
            } while (hit.collider != null);

            this.gameObject.transform.Translate(translation);
        }

        protected override void Update()
        {
            base.Update();
            if (!Dead)
            {
                UpdatePosition();
            }
        }
    }
}