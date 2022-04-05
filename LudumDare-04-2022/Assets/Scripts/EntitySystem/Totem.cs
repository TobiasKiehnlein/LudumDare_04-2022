namespace EntitySystem
{
    public class Totem: Deployable
    {

        protected override void OnDeath()
        {
            throw new System.NotImplementedException();
        }

        protected override void HandleNearbyEntity(Entity e, DistanceInformation distInfo)
        {
            throw new System.NotImplementedException();
        }

        public Totem(Type t) : base(Type.Totem)
        {
        }

        protected override float GetDuration()
        {
            return settings.totemDuration;
        }
    }
}