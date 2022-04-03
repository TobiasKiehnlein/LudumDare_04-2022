using System;
using UnityEngine;
using Utils;
using WallSystem;

namespace EntitySystem
{
    public abstract class MovingEntity : Entity
    {
        [SerializeField] private Vector2 startingDirection = Vector2.zero;
        [SerializeField] private float startingSpeed = 0;
        [SerializeField] private float wallCollisionDistance = 0.2f;

        private const int MaxHitcount = 5;
        private const float MinMoveDistance = 0.0f; // Per update and iteration
        private const float MaxMoveDistance = 10.000f; // Per update and iteration

        public Vector2 Direction
        {
            get => _direction;
            protected set
            {
                var oldDirection = _direction;
                _direction = value.normalized; // fffffffffffff
                OnUpdateDirection(oldDirection, _direction);
            }
        }

        private Vector2 _direction;

        public float Speed { get; protected set; }

        protected abstract void OnUpdateDirection(Vector2 oldDirection, Vector2 newDirection);

        // When this is called the Direction has already been updated
        // Note: Wall.Hit will automatically be called after this
        protected abstract void OnWallHit(Wall w);

        // Uses direction and speed to update the position of the gameobject, including Wall detection
        private void UpdatePosition()
        {
            float distance; // The distance to move in this frame
            float hitDeltaTime = 0; // The time at which the Wall is hit
            RaycastHit2D? lastHit = null;
            int hitcount = 0;
            while (hitcount <= MaxHitcount)
            {
                distance = Speed * (Time.deltaTime - hitDeltaTime);
                distance = Math.Clamp(distance, MinMoveDistance, MaxMoveDistance);
                var position = gameObject.transform.position;
                Debug.DrawRay(position, Direction * distance, Color.green, 5);
                // ReSharper disable once Unity.PreferNonAllocApi
                var hits = Physics2D.CircleCastAll(position, wallCollisionDistance, Direction, distance, Wall.WallMask);
                RaycastHit2D? confirmedHit = null;
                if (hits.Length > 0) // We have hit a Wall, but it might be the same from the last iteration
                {
                    if (hits[0].IsSameRaycastHit2D(
                            lastHit)) // Our first hit is the same from the last iteration => ignore the first hit
                    {
                        if (hits.Length > 1)
                        {
                            confirmedHit = hits[1];
                        }
                    }
                    else
                    {
                        confirmedHit = hits[0];
                    }
                }

                if (confirmedHit is { } hit)
                {
                    Debug.DrawRay(hit.point, hit.normal * 10, Color.red, 5);
                    var actualHitDistance = hit.distance - wallCollisionDistance;
                    this.gameObject.transform.Translate(Direction * actualHitDistance); // Important: actualHitDistance, not distance -> otherwise entity will move too far
                    hitDeltaTime += actualHitDistance / Speed;
                    Direction = Vector2.Reflect(Direction, hit.normal);
                    var wall = hit.collider.gameObject.GetComponentInChildren<Wall>();
                    if (wall != null)
                    {
                        OnWallHit(wall); // Might update the Speed
                        wall.Hit(this);
                    }
                    else
                    {
                        Debug.LogWarning(
                            $"Collider {hit.collider.gameObject.name} on Wall layer but without Wall component detected.");
                    }

                    lastHit = hit;
                    ++hitcount;
                }
                else
                {
                    this.gameObject.transform.Translate(Direction * distance);
                    break;
                }
            }

            if (hitcount > MaxHitcount)
            {
                Debug.LogWarning($"Entity {this.gameObject.name}: hitcount in one update > {MaxHitcount}");
            }
        }

        protected override void Start()
        {
            base.Start();
            Direction = startingDirection;
            Speed = startingSpeed;
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