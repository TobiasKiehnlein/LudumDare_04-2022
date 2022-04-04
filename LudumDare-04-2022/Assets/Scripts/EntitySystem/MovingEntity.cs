using System;
using ScriptableObjects;
using UnityEngine;
using Utils;
using WallSystem;
using Random = UnityEngine.Random;

namespace EntitySystem
{
    public abstract class MovingEntity : Entity
    {
        [SerializeField] protected MovingEntitySettings settings;
        [SerializeField] private float startingSpeed = 0;
        [SerializeField] public float speedAdjustSpeed = 1;
        public float unityReportSpeed = 0; // only reports to editor
        public float unityReportTargetSpeed = 0; // only reports to editor

        private float _stateUpdateCooldownRemaining;
        private float _wallHitDirectionCooldownRemaining = 0;

        public Vector2 Direction
        {
            get => _direction;
            private set
            {
                var oldDirection = _direction;
                _direction = value.normalized; // fffffffffffff
                OnUpdateDirection(oldDirection, _direction);
            }
        }

        private Vector2 _direction;

        // Important: Reports the actual speed but only sets the target speed (speedAdjustSpeed set how fast the actual speed reaches the target)
        public float TargetSpeed
        {
            get { return _targetSpeed; }
            set { _targetSpeed = Mathf.Abs(value); }
        }

        private float _targetSpeed;

        public float Speed
        {
            get { return _speed; }
            private set { _speed = Mathf.Abs(value); }
        }

        private float _speed;

        public MovingEntity(Type t) : base(t)
        {
        }

        protected abstract void OnUpdateDirection(Vector2 oldDirection, Vector2 newDirection);

        // Amount should contain time.deltatime
        public void InfluenceDirection(Vector2 direction, float amount)
        {
            if (_wallHitDirectionCooldownRemaining > 0) return;
            Direction += direction.normalized * amount * settings.steeringMultiplier;
        }

        // Amount should contain time.deltatime
        // Sets the speed directly, but does not modify the target speed
        public void InfluenceSpeed(float amount)
        {
            Speed += amount;
        }

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
            while (hitcount <= settings.maxHitcount)
            {
                distance = Speed * (Time.deltaTime - hitDeltaTime);
                distance = Math.Clamp(distance, settings.minMoveDistance, settings.maxMoveDistance);
                var position = gameObject.transform.position;
                Debug.DrawRay(position, Direction * distance, Color.green, settings.debugDrawDuration);
                // ReSharper disable once Unity.PreferNonAllocApi
                var hits = Physics2D.RaycastAll(position, Direction, distance + settings.wallCollisionDistance,
                    Wall.WallMask);
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
                    Debug.DrawRay(hit.point, hit.normal * 10, Color.red, settings.debugDrawDuration);
                    var actualHitDistance = hit.distance - settings.wallCollisionDistance;
                    this.gameObject.transform.Translate(Direction *
                                                        actualHitDistance); // Important: actualHitDistance, not distance -> otherwise entity will move too far
                    hitDeltaTime += actualHitDistance / Speed;
                    Direction = Vector2.Reflect(Direction, hit.normal);
                    var wall = hit.collider.gameObject.GetComponentInChildren<Wall>();
                    if (wall != null)
                    {
                        _wallHitDirectionCooldownRemaining = settings.wallHitDirectionCooldown;
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

            if (hitcount > settings.maxHitcount)
            {
                Debug.LogWarning($"Entity {this.gameObject.name}: hitcount in one update > {settings.maxHitcount}");
            }
        }

        protected override void Start()
        {
            if (settings == null)
            {
                Debug.LogError($"Entity {this.name} has not settings set.");
                Destroy(this);
                return;
            }

            base.Start();
            Direction = new Vector2(Random.Range(-10, 10), Random.Range(-10, 10));
            TargetSpeed = startingSpeed;
            _stateUpdateCooldownRemaining = settings.stateUpdateCooldown;
        }

        protected abstract void UpdateState();

        protected override void Update()
        {
            _wallHitDirectionCooldownRemaining -= Time.deltaTime;
            _stateUpdateCooldownRemaining -= Time.deltaTime;
            if (_stateUpdateCooldownRemaining <= 0)
            {
                _stateUpdateCooldownRemaining = settings.stateUpdateCooldown;
                UpdateState();
            }

            base.Update();
        }

        protected override void FixedUpdate()
        {
            base.Update();
            if (!Dead)
            {
                Speed = Mathf.Lerp(Speed, TargetSpeed, speedAdjustSpeed * Time.deltaTime);
                unityReportSpeed = Speed;
                unityReportTargetSpeed = TargetSpeed;
                UpdatePosition();
            }
        }
    }
}