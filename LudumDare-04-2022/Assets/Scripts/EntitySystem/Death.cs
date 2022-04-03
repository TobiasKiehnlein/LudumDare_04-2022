using UnityEngine;
using WallSystem;

namespace EntitySystem
{
    public class Death: MovingEntity
    {
        protected override void OnDeath()
        {
            //throw new System.NotImplementedException();
        }

        protected override void HandleNearbyEntity(Entity e, float distance)
        {
            Debug.Log($"{e.name}, {distance}");
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