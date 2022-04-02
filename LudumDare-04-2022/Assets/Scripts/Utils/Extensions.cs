using UnityEngine;

public static class Extensions
{
    public static bool IsInLayerMask(this GameObject gameObject, int layerMask)
    {
        return layerMask == (layerMask | (1 << gameObject.layer));
    }
}