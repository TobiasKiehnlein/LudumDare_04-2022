using UnityEngine;
using WallSystem;

namespace EntitySystem
{
    public class Human: MovingEntity
    {
        protected override void OnDeath()
        {
            //throw new System.NotImplementedException();
        }

        protected override void HandleNearbyEntity(Entity e, float distance)
        {
            //throw new System.NotImplementedException();
        }

        protected override void OnUpdateDirection(Vector2 oldDirection, Vector2 newDirection)
        {
            //throw new System.NotImplementedException();
        }

        protected override void OnWallHit(Wall w)
        {
            //throw new System.NotImplementedException();
        }
    }
}