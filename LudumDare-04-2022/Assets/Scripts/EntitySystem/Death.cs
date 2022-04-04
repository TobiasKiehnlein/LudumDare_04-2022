using System;
using UnityEngine;
using WallSystem;

namespace EntitySystem
{
    public class Death : MovingEntity
    {
        [SerializeField] public Mood state = Mood.Normal;

        private TransitionMatrix<Mood> _transitionMatrix = new TransitionMatrix<Mood>();

        [Serializable]
        public enum Mood
        {
            Normal,
            Aggressive,
            KillingSpree,
            Starving,
        }

        public Death() : base(Type.Death)
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
            Vector2 steeringDirection = ((Vector2)(e.gameObject.transform.position - this.transform.position)).normalized; // Direction towards

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
                        
                        steeringStrength += settings.death_deadHuman_steeringStrengthBase * distInfo.LowDistanceFraction;
                    }
                    else
                    {
                        _transitionMatrix.BoostState(Mood.Normal, +distInfo.MedDistanceFraction * delta * 0.1f);
                        steeringStrength += settings.death_deadHuman_steeringStrengthBase * distInfo.MedDistanceFraction;
                        if (distInfo.IsCollision)
                        {
                            //if (noKillCooldown)
                            _transitionMatrix.BoostState(Mood.Starving, -1); // no delta
                            _transitionMatrix.BoostState(Mood.Aggressive, -1); // no delta
                            _transitionMatrix.BoostState(Mood.KillingSpree, +1); // no delta
                            // TODO: Kill Human
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

        protected override void OnUpdateDirection(Vector2 oldDirection, Vector2 newDirection)
        {
            //throw new System.NotImplementedException();
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

        protected override void UpdateState()
        {
            _transitionMatrix.ReduceMultipliers(settings.stateUpdateCooldown * settings.stateMultiplierReductionSpeed);
            state = _transitionMatrix.GetNextState(state);
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
            }
        }
    }
}