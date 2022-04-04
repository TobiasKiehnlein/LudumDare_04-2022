namespace EntitySystem
{
    public class GenericObstacle: Entity
    {
        public GenericObstacle() : base(Type.Obstacle)
        {
        }

        protected override void Start()
        {
            handleNearby = false;
            base.Start();
        }

        protected override void OnDeath()
        {
            throw new System.NotImplementedException();
        }

        protected override void HandleNearbyEntity(Entity e, DistanceInformation distInfo)
        {
            throw new System.NotImplementedException();
        }
    }
}