using System;
using UnityEngine;

namespace Utils
{
    public class MoveUpSpawn: MonoBehaviour
    {
        [SerializeField] private float startHeight = 5;
        [SerializeField] private float endHeight = 0;
        [SerializeField] private float speed = 1;
        
        private void Start()
        {
            var pos = this.gameObject.transform.position;
            this.gameObject.transform.position = new Vector3(pos.x, pos.y, startHeight);
        }

        private void Update()
        {
            var pos = this.gameObject.transform.position;
            var newZ = Mathf.Lerp(pos.z, endHeight, Time.deltaTime * speed);
            this.gameObject.transform.position = new Vector3(pos.x, pos.y, newZ);
            if (Mathf.Abs(newZ - endHeight) < 0.001) Destroy(this);
        }
    }
}