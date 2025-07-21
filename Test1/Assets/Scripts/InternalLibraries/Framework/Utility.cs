using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility
{
    public static T Get<T>(Transform parent, string path) where T : UnityEngine.Object
    {
        if (parent)
        {
            Transform tempTran = parent.Find(path);
            if (tempTran)
            {
                if (typeof(T) == typeof(Transform)) return tempTran as T;
                if (typeof(T) == typeof(GameObject)) return tempTran.gameObject as T;
                T tempComp = tempTran.gameObject.GetComponent<T>() as T;
                return tempComp;
            }
        }
        return null;
    }
}
