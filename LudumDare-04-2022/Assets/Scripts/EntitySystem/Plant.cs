using UnityEngine;

namespace EntitySystem
{
    public class Plant: Entity
    {
        [SerializeField] private Sprite healthy;
        [SerializeField] private Sprite mediHealthy;
        [SerializeField] private Sprite unhealthy;

        [SerializeField] private float health = 1f;

        private SpriteRenderer sprite;
        
        public Plant() : base(Type.Obstacle)
        {
        }

        protected override void Start()
        {
            base.Start();
            sprite = spriteContainer.GetComponentInChildren<SpriteRenderer>();
            if (sprite == null) Debug.LogError($"Plant {this.name} has no SpriteRenderer");
            sprite.sprite = healthy;
            handleNearby = true;
        }

        protected override void OnDeath()
        {
            throw new System.NotImplementedException();
        }

        protected override void HandleNearbyEntity(Entity e, DistanceInformation distInfo)
        {
            if (e.type != Type.Human || !e.Dead) return;
            health -= Time.deltaTime / settings.human_deathTimeout * distInfo.HighDistanceFraction;

            if (health < 0.5f)
            {
                sprite.sprite = mediHealthy;
            } else if (sprite.sprite != unhealthy && health < 0)
            {
                sprite.sprite = unhealthy;
            }
        }
    }
}