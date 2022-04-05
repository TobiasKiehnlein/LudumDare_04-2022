using System;
using UnityEngine;

namespace Utils
{
    public class LerpHeight: MonoBehaviour
    {
        [SerializeField] public float startHeight = 5;
        [SerializeField] public float endHeight = 0;
        [SerializeField] public float speed = 1;
        [SerializeField] public bool destroyGameObject = false;
        public bool manualTrigger = false;
        
        private void Start()
        {
            if (manualTrigger) return;
            var pos = this.gameObject.transform.position;
            this.gameObject.transform.position = new Vector3(pos.x, pos.y, startHeight);
        }

        public void ManualTrigger()
        {
            var pos = this.gameObject.transform.position;
            this.gameObject.transform.position = new Vector3(pos.x, pos.y, startHeight);
            manualTrigger = false;
        }

        private void Update()
        {
            if (manualTrigger) return;
            
            var pos = this.gameObject.transform.position;
            var newZ = Mathf.Lerp(pos.z, endHeight, Time.deltaTime * speed);
            this.gameObject.transform.position = new Vector3(pos.x, pos.y, newZ);
            if (Mathf.Abs(newZ - endHeight) < 0.01)
            {
                if (destroyGameObject) Destroy(this.gameObject);
                else Destroy(this);
            }
        }
    }
}