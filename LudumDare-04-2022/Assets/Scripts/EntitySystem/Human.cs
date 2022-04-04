using System;
using UnityEngine;
using Utils;
using WallSystem;
using Random = UnityEngine.Random;

namespace EntitySystem
{
    public class Human : MovingEntity
    {
        [SerializeField] public Mood state = Mood.Chilling;

        private TransitionMatrix<Mood> _transitionMatrix = new TransitionMatrix<Mood>();

        private void SetupTransitionMatrix()
        {
            _transitionMatrix.SetWeight(Mood.Chilling, Mood.Chilling, 20);
            _transitionMatrix.SetWeight(Mood.Chilling, Mood.Crowdy, 2);
            _transitionMatrix.SetWeight(Mood.Chilling, Mood.AntiCrowdy, 3);
            _transitionMatrix.SetWeight(Mood.Chilling, Mood.Jogging, 1);
            _transitionMatrix.SetWeight(Mood.Chilling, Mood.Afraid, 0.5f);

            _transitionMatrix.SetWeight(Mood.Crowdy, Mood.Chilling, 5);
            _transitionMatrix.SetWeight(Mood.Crowdy, Mood.Crowdy, 20);
            _transitionMatrix.SetWeight(Mood.Crowdy, Mood.AntiCrowdy, 5);
            _transitionMatrix.SetWeight(Mood.Crowdy, Mood.Jogging, 2);
            _transitionMatrix.SetWeight(Mood.Crowdy, Mood.Afraid, 0.1f);

            _transitionMatrix.SetWeight(Mood.AntiCrowdy, Mood.Chilling, 4);
            _transitionMatrix.SetWeight(Mood.AntiCrowdy, Mood.Crowdy, 1);
            _transitionMatrix.SetWeight(Mood.AntiCrowdy, Mood.AntiCrowdy, 10);
            _transitionMatrix.SetWeight(Mood.AntiCrowdy, Mood.Jogging, 6);
            _transitionMatrix.SetWeight(Mood.AntiCrowdy, Mood.Afraid, 1);

            _transitionMatrix.SetWeight(Mood.Jogging, Mood.Chilling, 4);
            _transitionMatrix.SetWeight(Mood.Jogging, Mood.Crowdy, 4);
            _transitionMatrix.SetWeight(Mood.Jogging, Mood.AntiCrowdy, 1);
            _transitionMatrix.SetWeight(Mood.Jogging, Mood.Jogging, 20);
            _transitionMatrix.SetWeight(Mood.Jogging, Mood.Afraid, 0.5f);

            _transitionMatrix.SetWeight(Mood.Afraid, Mood.Chilling, 1);
            _transitionMatrix.SetWeight(Mood.Afraid, Mood.Crowdy, 5);
            _transitionMatrix.SetWeight(Mood.Afraid, Mood.AntiCrowdy, 2);
            _transitionMatrix.SetWeight(Mood.Afraid, Mood.Jogging, 6);
            _transitionMatrix.SetWeight(Mood.Afraid, Mood.Afraid, 15);
        }

        [Serializable]
        public enum Mood
        {
            Chilling,
            Crowdy,
            AntiCrowdy,
            Jogging,
            Afraid,
        }

        public Human() : base(Type.Human)
        {
        }

        protected override void OnDeath()
        {
        }

        protected override void HandleNearbyEntity(Entity e, DistanceInformation distInfo)
        {
            var delta = Time.deltaTime;
            var steeringStrength = 0f;
            Vector2 steeringDirection = ((Vector2)(e.gameObject.transform.position - this.transform.position)).normalized; // Direction towards

            switch (e.type) // Set steering base values, direction and Mood changing depending on type
            {
                case Type.Death:
                    _transitionMatrix.BoostState(Mood.Afraid, +distInfo.MedDistanceFraction * delta);
                    if (distInfo.IsCollision)
                    {
                        _transitionMatrix.BoostState(Mood.Afraid, +distInfo.HighDistanceFraction * delta * 5);
                    }

                    steeringDirection *= -1;
                    steeringStrength += settings.human_death_steeringStrengthBase * distInfo.MedDistanceFraction;
                    break;
                case Type.Human:
                    if (e.Dead)
                    {
                        _transitionMatrix.BoostState(Mood.Afraid, +distInfo.HighDistanceFraction * 3 * delta);
                        _transitionMatrix.BoostState(Mood.AntiCrowdy, +distInfo.LowDistanceFraction * delta * 3);
                        _transitionMatrix.BoostState(Mood.Crowdy, +distInfo.HighDistanceFraction * delta * 0.5f);
                        
                        steeringDirection *= -1;
                        steeringStrength += settings.human_deadHuman_steeringStrengthBase * distInfo.HighDistanceFraction;
                    }
                    else
                    {
                        _transitionMatrix.BoostState(Mood.Crowdy, +distInfo.HighDistanceFraction * delta * 0.05f);
                        _transitionMatrix.BoostState(Mood.Afraid, -distInfo.LowDistanceFraction * delta * 0.5f);
                        _transitionMatrix.BoostState(Mood.Chilling, +distInfo.LowDistanceFraction * delta * 0.5f);
                        _transitionMatrix.BoostState(Mood.AntiCrowdy, +distInfo.LowDistanceFraction * delta * 0.25f);
                        steeringStrength += settings.human_human_steeringStrengthBase;
                        if (distInfo.IsCollision)
                        {
                            steeringDirection *= -1;
                            steeringStrength += settings.human_human_steeringStrengthCollision;
                        }
                    }

                    break;
                case Type.Cross:
                    _transitionMatrix.BoostState(Mood.Afraid, -distInfo.MedDistanceFraction * delta * 2);
                    _transitionMatrix.BoostState(Mood.Crowdy, +distInfo.HighDistanceFraction * delta);
                    steeringStrength += settings.human_cross_steeringStrengthBase;
                    break;
                case Type.Totem:
                    _transitionMatrix.BoostState(Mood.Afraid, +distInfo.MedDistanceFraction * delta);
                    _transitionMatrix.BoostState(Mood.Jogging, +distInfo.LowDistanceFraction * delta * 3);
                    steeringDirection *= -1;
                    steeringStrength += settings.human_totem_steeringStrengthBase * distInfo.MedDistanceFraction;
                    if (distInfo.IsCollision) steeringStrength += settings.human_totem_steeringStrengthCollision;
                    break;
                case Type.Obstacle:
                    _transitionMatrix.BoostState(Mood.Chilling, +distInfo.LowDistanceFraction * delta * 0.5f);
                    _transitionMatrix.BoostState(Mood.Crowdy, +distInfo.MedDistanceFraction * delta * 0.1f);
                    steeringDirection *= -1;
                    steeringStrength += settings.human_obstacle_steeringStrengthBase * distInfo.LowDistanceFraction;
                    break;
                case Type.VisualOnly:
                    _transitionMatrix.BoostState(Mood.Chilling, +distInfo.LowDistanceFraction * delta * 0.5f);
                    _transitionMatrix.BoostState(Mood.Crowdy, +distInfo.MedDistanceFraction * delta);
                    steeringDirection *= -1;
                    steeringStrength += settings.human_visualOnly_steeringStrengthBase * distInfo.LowDistanceFraction;
                    if (distInfo.IsCollision) steeringStrength += settings.human_visualOnly_steeringStrengthCollision;
                    break;
            }
            
            switch (state) // Apply steering factors depending on Mood
            {
                case Mood.Chilling:
                    steeringStrength *= settings.human_chilling_steeringStrengthMult;
                    break;
                case Mood.Crowdy:
                    steeringStrength *= settings.human_crowdy_steeringStrengthMult;
                    break;
                case Mood.AntiCrowdy:
                    steeringStrength *= settings.human_antiCrowdy_steeringStrengthMult;
                    break;
                case Mood.Jogging:
                    steeringStrength *= settings.human_jogging_steeringStrengthMult;
                    break;
                case Mood.Afraid:
                    steeringStrength *= settings.human_afraid_steeringStrengthMult;
                    break;

            }
            
            // Steer
            steeringStrength = Mathf.Max(0, steeringStrength);
            InfluenceDirection(steeringDirection, steeringStrength * delta);
        }

        protected override void UpdateState()
        {
            _transitionMatrix.ReduceMultipliers(settings.stateUpdateCooldown * settings.stateMultiplierReductionSpeed);
            state = _transitionMatrix.GetNextState(state);
            switch (state)
            {
                case Mood.Chilling:
                    TargetSpeed = settings.human_chilling_targetSpeed;
                    break;
                case Mood.Crowdy:
                    TargetSpeed = settings.human_crowdy_targetSpeed;
                    break;
                case Mood.AntiCrowdy:
                    TargetSpeed = settings.human_antiCrowdy_targetSpeed;
                    break;
                case Mood.Jogging:
                    TargetSpeed = settings.human_jogging_targetSpeed;
                    break;
                case Mood.Afraid:
                    TargetSpeed = settings.human_afraid_targetSpeed;
                    break;
            }
        }

        protected override void Start()
        {
            base.Start();
            SetupTransitionMatrix();
        }

        protected override void OnUpdateDirection(Vector2 oldDirection, Vector2 newDirection)
        {
            //throw new System.NotImplementedException();
        }

        protected override void OnWallHit(Wall w)
        {
            switch (state)
            {
                case Mood.Chilling:
                    InfluenceSpeed(settings.human_chilling_wallSpeedInfluence);
                    break;
                case Mood.Crowdy:
                    InfluenceSpeed(settings.human_crowdy_wallSpeedInfluence);
                    break;
                case Mood.AntiCrowdy:
                    InfluenceSpeed(settings.human_antiCrowdy_wallSpeedInfluence);
                    break;
                case Mood.Jogging:
                    InfluenceSpeed(settings.human_jogging_wallSpeedInfluence);
                    break;
                case Mood.Afraid:
                    InfluenceSpeed(settings.human_afraid_wallSpeedInfluence);
                    break;
            }
        }
    }
}