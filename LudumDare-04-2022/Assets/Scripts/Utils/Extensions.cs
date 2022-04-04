using System;
using EntitySystem;
using UnityEngine;

namespace Utils
{
    public static class Extensions
    {
        private const float RaycastDistanceMargin = 0.2f;
        private const float RaycastNormalMargin = 0.001f;

        public static bool IsInLayerMask(this GameObject gameObject, int layerMask)
        {
            return layerMask == (layerMask | (1 << gameObject.layer));
        }

        public static bool IsSameRaycastHit2D(this RaycastHit2D hit, RaycastHit2D? otherHit)
        {
            if (otherHit is { } unpackedOtherHit)
            {
                if (hit.collider != unpackedOtherHit.collider) return false;
                if (Vector2.Distance(hit.point, unpackedOtherHit.point) > RaycastDistanceMargin) return false;
                var dot = Vector2.Dot(hit.normal, unpackedOtherHit.normal);
                if (1 - dot < RaycastNormalMargin) return false;
                return true;
            }

            return false;
        }

        public static int ToInt<T>(this T e) where T : Enum
        {
            return (int) Convert.ChangeType(e, typeof(int));
        }

        public static T ToEnum<T>(this int i) where T : Enum
        {
            return (T) Enum.ToObject(typeof(T), i);
        }
    }
}