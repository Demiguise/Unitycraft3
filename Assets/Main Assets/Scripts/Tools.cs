using UnityEngine;
using System.Collections;

public static class Tools {

    public static int CreateLayerMask(int[] layers)
    {
        int layerMask = 1;
        foreach (int layer in layers)
        {
            int i = 1 << layer;
            layerMask = layerMask | i;
        }
        return layerMask;
    }

    public static RaycastHit CastRay(Vector3 pos, Vector3 dir, float dist, int[] layer)
    {
        int layerMask = CreateLayerMask(layer);
        Ray rV = new Ray(pos, dir);
        RaycastHit objectHit;
        Physics.Raycast(rV, out objectHit, dist, layerMask);
        return objectHit;
    }
}
