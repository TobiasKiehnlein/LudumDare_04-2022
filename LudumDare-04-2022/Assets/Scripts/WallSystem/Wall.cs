using EntitySystem;
using UnityEngine;

namespace WallSystem
{
    public abstract class Wall: MonoBehaviour
    {
        public static int WallMask { get; private set; }

        static Wall()
        {
            string[] wallLayers = {"Wall"};
            WallMask = LayerMask.GetMask(wallLayers);
        }
        
        public abstract void Hit(Entity e);
    }
}