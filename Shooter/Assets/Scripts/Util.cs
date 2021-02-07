using UnityEngine;

public class Util
{
    public static void SetlayerRecursively(GameObject obj, int newLayer)
    {
        if(obj == null)
        {
            return;
        }
        obj.layer = newLayer;

        foreach(Transform t in obj.transform)
        {
            if(t == null)
            {
                continue;
            }
            SetlayerRecursively(t.gameObject, newLayer);
        }
    }
}
