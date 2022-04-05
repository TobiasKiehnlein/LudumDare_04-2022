using UnityEngine;
using Utils;

namespace EntitySystem
{
    public class Cross: Deployable
    {
        public Cross(Type t) : base(Type.Cross)
        {
        }

        protected override void OnDeath()
        {
            throw new System.NotImplementedException();
        }

        protected override void HandleNearbyEntity(Entity e, DistanceInformation distInfo)
        {
            throw new System.NotImplementedException();
        }

        protected override float GetDuration()
        {
            return settings.crossDuration;
        }
    }
}