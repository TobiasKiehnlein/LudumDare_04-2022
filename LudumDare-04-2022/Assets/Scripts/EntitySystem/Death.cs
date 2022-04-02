using WallSystem;

namespace EntitySystem
{
    public class Death: MovingEntity
    {
        protected override void OnDeath()
        {
            //throw new System.NotImplementedException();
        }

        protected override void HandleNearbyEntity(Entity e)
        {
            //throw new System.NotImplementedException();
        }

        protected override void OnUpdateDirection()
        {
            //throw new System.NotImplementedException();
        }

        protected override void OnWallHit(Wall w)
        {
            //throw new System.NotImplementedException();
        }
    }
}