using System;
using UnityEngine;
using WallSystem;

namespace EntitySystem
{
    public class Death : MovingEntity
    {
        [SerializeField] public Mood state = Mood.Normal;

        private TransitionMatrix<Mood> _transitionMatrix = new TransitionMatrix<Mood>();

        private void SetupTransitionMatrix()
        {
            _transitionMatrix.SetWeight(Mood.Normal, Mood.Normal, 2);
        }

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

        protected override void Start()
        {
            base.Start();
            SetupTransitionMatrix();
        }

        protected override void OnDeath()
        {
            //throw new System.NotImplementedException();
        }

        protected override void HandleNearbyEntity(Entity e, DistanceInformation distInfo)
        {
            //throw new System.NotImplementedException();
        }

        protected override void OnUpdateDirection(Vector2 oldDirection, Vector2 newDirection)
        {
            //throw new System.NotImplementedException();
        }

        protected override void OnWallHit(Wall w)
        {
            switch (state)
            {
                /*case Mood.Normal:
                    break;*/
                case Mood.Aggressive:
                    InfluenceSpeed(15);
                    break;
                case Mood.KillingSpree:
                    InfluenceSpeed(-10);
                    break;
                case Mood.Starving:
                    InfluenceSpeed(20);
                    break;
                default:
                    InfluenceSpeed(5);
                    break;
            }
        }

        protected override void UpdateState()
        {
            //throw new System.NotImplementedException();
        }
    }
}