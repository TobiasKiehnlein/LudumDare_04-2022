using EntitySystem;
using UnityEngine;

namespace WallSystem
{
    public abstract class Wall: MonoBehaviour
    {
        public static int WallMask
        {
            get
            {
                if (_wallMask < 0)
                {
                    string[] wallLayers = {"Wall"};
                    _wallMask = LayerMask.GetMask(wallLayers);
                }

                return _wallMask;
            }
        }

        private static int _wallMask = -1;

        public abstract void Hit(Entity e);
    }
}