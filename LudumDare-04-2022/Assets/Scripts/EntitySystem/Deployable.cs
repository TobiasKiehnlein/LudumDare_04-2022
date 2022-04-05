using UnityEngine;
using Utils;

namespace EntitySystem
{
    public abstract class Deployable: Entity
    {
        private float _startTime;
        private bool lerpDeployed = false;
        
        public Deployable(Type t): base(t)
        {
        }

        protected abstract float GetDuration();
        
        protected override void Start()
        {
            base.Start();
            handleNearby = false;
            _startTime = Time.time;
        }
        
        protected override void Update()
        {
            base.Update();
            if (_startTime < Time.time - GetDuration() && !lerpDeployed)
            {
                var lerp = gameObject.AddComponent<LerpHeightManual>();
                lerp.startHeight = 0;
                lerp.endHeight = 3;
                lerp.destroyGameObject = true;
                lerp.ManualTrigger();
                lerpDeployed = true;
            }
        }
        
        
    }
}