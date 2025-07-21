using System.Collections.Generic;
using UnityEngine;

public class UITools
{
    /// <summary>
    /// 获取UI的除Canvas的所有父节点
    /// </summary>
    /// <param name="child">当前物体</param>
    /// <returns></returns>
    public static List<GameObject> GetUIAllParentNodes(GameObject child)
    {
        List<GameObject> allParentList = new List<GameObject>();
        GameObject kObj = child;
        while (kObj.transform.parent != null)
        {
            GameObject newParent = kObj.transform.parent.gameObject;
            if (newParent.GetComponent<Canvas>() == null)
            {
                allParentList.Add(newParent);
            }
            kObj = newParent;
        }

        return allParentList;
    }
        
}