using System;
using UnityEngine;
using Utils;
using WallSystem;

namespace EntitySystem
{
    public class Death : MovingEntity<Death.Mood>
    {
        private float _lastKillTime = 0;
        private float _spawnTime;
        
        [Serializable]
        public enum Mood
        {
            Normal,
            Aggressive,
            KillingSpree,
            Starving,
            Killing
        }

        private const string AnimationSliceTriggerName = "Slice";
        private const string AnimationStateName = "State";
        public enum AnimationState
        {
            Front,
            Back,
            Side,
            SideFront,
            SideBack,
        }

        public Death() : base(Type.Death, Mood.Normal)
        {
        }

        protected override void OnDeath()
        {
            throw new System.NotImplementedException();
        }

        protected override void HandleNearbyEntity(Entity e, DistanceInformation distInfo)
        {
            var delta = Time.deltaTime;
            var steeringStrength = 0f;
            Vector2 steeringDirection =
                ((Vector2) (e.gameObject.transform.position - this.transform.position)).normalized; // Direction towards

            switch (e.type) // Set steering base values, direction and Mood changing depending on type
            {
                case Type.Death:
                    _transitionMatrix.BoostState(Mood.KillingSpree, +distInfo.HighDistanceFraction * delta * 0.5f);
                    if (distInfo.IsLowDistance)
                    {
                        _transitionMatrix.BoostState(Mood.Aggressive, +distInfo.LowDistanceFraction * delta * 2);
                    }

                    steeringDirection *= -1;
                    steeringStrength += settings.death_death_steeringStrengthBase * distInfo.HighDistanceFraction;
                    if (distInfo.IsCollision) steeringStrength += settings.death_death_steeringStrengthCollision;
                    break;
                case Type.Human:
                    if (e.Dead)
                    {
                        _transitionMatrix.BoostState(Mood.KillingSpree, +distInfo.HighDistanceFraction * 3 * delta);
                        _transitionMatrix.BoostState(Mood.Starving, -distInfo.MedDistanceFraction * delta * 3);

                        steeringStrength +=
                            settings.death_deadHuman_steeringStrengthBase * distInfo.LowDistanceFraction;
                    }
                    else
                    {
                        _transitionMatrix.BoostState(Mood.Normal, +distInfo.MedDistanceFraction * delta * 0.1f);
                        steeringStrength +=
                            settings.death_deadHuman_steeringStrengthBase * distInfo.MedDistanceFraction;
                        if (distInfo.IsCollision)
                        {
                            Slice(e);
                        }
                    }

                    break;
                case Type.Cross:
                    _transitionMatrix.BoostState(Mood.Aggressive, +distInfo.MedDistanceFraction * delta * 2);
                    _transitionMatrix.BoostState(Mood.Starving, +distInfo.LowDistanceFraction * delta);
                    steeringDirection *= -1;
                    steeringStrength += settings.death_cross_steeringStrengthBase * distInfo.MedDistanceFraction;
                    if (distInfo.IsCollision)
                    {
                        _transitionMatrix.BoostState(Mood.Starving, +distInfo.LowDistanceFraction * delta * 2);
                        steeringStrength += settings.death_cross_steeringStrengthCollision;
                    }

                    break;
                case Type.Totem:
                    _transitionMatrix.BoostState(Mood.Normal, +distInfo.MedDistanceFraction * delta);
                    _transitionMatrix.BoostState(Mood.Aggressive, -distInfo.LowDistanceFraction * delta * 2);
                    _transitionMatrix.BoostState(Mood.KillingSpree, +distInfo.LowDistanceFraction * delta * 0.5f);
                    steeringStrength += settings.death_totem_steeringStrengthBase;
                    break;
                case Type.Obstacle:
                    _transitionMatrix.BoostState(Mood.Aggressive, +distInfo.LowDistanceFraction * delta * 0.05f);
                    steeringDirection *= -1;
                    steeringStrength += settings.death_obstacle_steeringStrengthBase * distInfo.LowDistanceFraction;
                    break;
                case Type.VisualOnly:
                    _transitionMatrix.BoostState(Mood.Normal, +distInfo.LowDistanceFraction * delta * 0.05f);
                    steeringDirection *= -1;
                    steeringStrength += settings.death_visualOnly_steeringStrengthBase * distInfo.LowDistanceFraction;
                    if (distInfo.IsCollision) steeringStrength += settings.death_visualOnly_steeringStrengthCollision;
                    break;
            }

            switch (state) // Apply steering factors depending on Mood
            {
                case Mood.Normal:
                    steeringStrength *= settings.death_normal_steeringStrengthMult;
                    break;
                case Mood.Aggressive:
                    steeringStrength *= settings.death_aggressive_steeringStrengthMult;
                    break;
                case Mood.KillingSpree:
                    steeringStrength *= settings.death_killingSpree_steeringStrengthMult;
                    break;
                case Mood.Starving:
                    steeringStrength *= settings.death_Starving_steeringStrengthMult;
                    break;
            }

            // Steer
            steeringStrength = Mathf.Max(0, steeringStrength);
            InfluenceDirection(steeringDirection, steeringStrength * delta);
        }

        private void Slice(Entity e)
        {
            if (_lastKillTime > Time.time - settings.death_killCooldown) return;
            
            handleNearby = false;
            UpdateState(Mood.Killing);
            var entityPos = e.gameObject.transform.position;
            var targetPos = new Vector2(entityPos.x, entityPos.y - 0.5f);
            var targetDirection = (targetPos - (Vector2)this.transform.position).normalized;
            InfluenceDirection(targetDirection, 10);
            MultiplySpeed(0.05f);
            _transitionMatrix.BoostState(Mood.Starving, -1); // no delta
            _transitionMatrix.BoostState(Mood.Aggressive, -1); // no delta
            _transitionMatrix.BoostState(Mood.KillingSpree, +3); // no delta
            Animator.SetTrigger(AnimationSliceTriggerName);
            e.Kill();
            _lastKillTime = Time.time;
        }

        protected override void OnUpdateDirection(Vector2 oldDirection, Vector2 newDirection)
        {
            UpdateMirror();
            var angle = Mathf.Asin(newDirection.y);
            const float pi8 = Mathf.PI / 8;
            var stateInt = 0;
            if (angle > pi8 * 3) // Upward
            {
                stateInt = AnimationState.Back.ToInt();
            }
            else if (angle > pi8) // Upward-Side
            {
                stateInt = AnimationState.SideBack.ToInt();
            }
            else if (angle > -pi8) // Side
            {
                stateInt = AnimationState.Side.ToInt();
            }
            else if (angle > -pi8 * 3) // Downward-Side
            {
                stateInt = AnimationState.SideFront.ToInt();
            }
            else // Downward
            {
                stateInt = AnimationState.Front.ToInt();
            }
            Animator.SetInteger(AnimationStateName, stateInt);
        }

        protected override void OnUpdateSpeed(float oldSpeed, float newSpeed)
        {
            // Not needed
        }

        protected override void OnWallHit(Wall w)
        {
            switch (state)
            {
                case Mood.Normal:
                    InfluenceSpeed(settings.death_normal_wallSpeedInfluence);
                    break;
                case Mood.Aggressive:
                    InfluenceSpeed(settings.death_aggressive_wallSpeedInfluence);
                    break;
                case Mood.KillingSpree:
                    InfluenceSpeed(settings.death_killingSpree_wallSpeedInfluence);
                    break;
                case Mood.Starving:
                    InfluenceSpeed(settings.death_Starving_wallSpeedInfluence);
                    break;
            }
        }

        protected override void Start()
        {
            base.Start();
            _spawnTime = Time.time;
            handleNearby = false;
            move = false;
        }

        protected override void Update()
        {
            base.Update();
            if (_spawnTime < Time.time - settings.death_spawnCooldown)
            {
                handleNearby = true;
                move = true;
            }
            if (_lastKillTime < Time.time - settings.death_killCooldown)
            {
                handleNearby = true;
                if (state == Mood.Killing) UpdateState(Mood.KillingSpree);
            }
        }

        protected override void UpdateSpeed()
        {
            switch (state)
            {
                case Mood.Normal:
                    TargetSpeed = settings.death_normal_targetSpeed;
                    break;
                case Mood.Aggressive:
                    TargetSpeed = settings.death_aggressive_targetSpeed;
                    break;
                case Mood.KillingSpree:
                    TargetSpeed = settings.death_killingSpree_targetSpeed;
                    break;
                case Mood.Starving:
                    TargetSpeed = settings.death_Starving_targetSpeed;
                    break;
                case Mood.Killing:
                    TargetSpeed = settings.death_Killing_targetSpeed;
                    break;
            }
        }
    }
}