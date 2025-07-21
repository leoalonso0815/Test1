using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public enum FilterType
{
    /// <summary>
    /// 
    /// </summary>
    GetCurrentAndContinue,
    /// <summary>
    /// 正常搜索
    /// </summary>
    Continue,
    /// <summary>
    /// 停止搜索，但是仍返回当前子物体
    /// </summary>
    GetCurrentAndStop,
    /// <summary>
    /// 停止搜索
    /// </summary>
    Stop,
}

public static class ComponentExtensions
{
    public static T AddChild<T>(this GameObject obj) where T : Component
    {
        var child = new GameObject().transform;
        child.SetParent(obj.transform);
        return child.GetOrAddComponent<T>();
    }

    public static T GetOrAddComponent<T>(this Component component) where T : Component
    {
        return component.gameObject.GetOrAddComponent<T>();
    }
    
    public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
    {
        var result = gameObject.GetComponent<T>();
        if (null == result)
        {
            result = gameObject.AddComponent<T>();
        }

        return result;
    }

    public static T GetComponentAt<T>(this Component component, string childPath) where T : Component
    {
        return null == component ? null : component.transform.GetComponentAt<T>(childPath);
    }
    
    public static T GetComponentAt<T>(this GameObject gameObject, string childPath) where T : Component
    {
        return null == gameObject ? null : gameObject.transform.GetComponentAt<T>(childPath);
    }

    public static T GetComponentAt<T>(this Transform transform, string childPath) where T : Component
    {
        if (null == transform)
        {
            return null;
        }

        var childTransform = transform.Find(childPath);
        return null != childTransform ? childTransform.GetComponent<T>() : null;
    }

    public static void SetScaleXAbs(this Component component, bool isPositive)
    {
        SetScaleXAbs(component.transform, isPositive);
    }

    public static void SetScaleXAbs(this Transform transform, bool isPositive)
    {
        var scale = transform.localScale;
        var scaleXAbs = Mathf.Abs(scale.x);
        scale.x = isPositive ? scaleXAbs : -scaleXAbs;
        transform.localScale = scale;
    }

    public static void RevertScaleX(this Component component)
    {
        RevertScaleX(component.transform);
    }

    public static void RevertScaleX(this Transform transform)
    {
        var scale = transform.localScale;
        scale.x = -scale.x;
        transform.localScale = scale;
    }

    public static bool IsHasComponent<T>(this Transform transform) where T : Component
    {
        return null != transform.GetComponent<T>();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="root">查找的根物体</param>
    /// <param name="childName">子物体名字（必须唯一），不唯一，返回找到的第一个。</param>
    /// <returns></returns>
    public static Transform FindChildByName(this Transform root, string childName)
    {
        if (null == root) return null;
        var result = root.FilterComponentsInChildren<Transform>(child =>
        {
            if (child.name == childName)
            {
                return FilterType.GetCurrentAndStop;
            }

            return FilterType.Continue;
        });

        if (0 < result.Count)
        {
            return result[0];
        }

        return null;
    }

    /// <summary>
    /// 从根物体开始，在根物体及所有子物体中筛选所需要的组件。
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="onStopSearchCondition">满足条件则停止</param>
    /// <param name="includeRoot">是否包含根物体</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static List<T> FilterComponentsInChildren<T>(this Transform parent, Func<Transform, FilterType> onStopSearchCondition, bool includeRoot = false) where T : Component
    {
        var result = new List<T>();
        
        if (null == onStopSearchCondition)
        {
            var components = parent.GetComponentsInChildren<T>(true).ToList();
            if (!includeRoot)
            {
                components.RemoveAt(0);
            }
            result.AddRange(components);
            return result;
        }
        
        if (includeRoot)
        {
            parent.FilterComponentsInChildrenInternal(result, onStopSearchCondition);
        }
        else
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                var child = parent.GetChild(i);
                child.FilterComponentsInChildrenInternal(result, onStopSearchCondition);
            }
        }
        
        return result;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="result"></param>
    /// <param name="onStopSearchCondition">必定不为空。</param>
    /// <typeparam name="T"></typeparam>
    private static void FilterComponentsInChildrenInternal<T>(this Transform parent, List<T> result, Func<Transform, FilterType> onStopSearchCondition) where T : Component
    {
        var searchType = onStopSearchCondition.Invoke(parent);
        if (searchType == FilterType.Stop)
        {
            return;
        }

        if (searchType == FilterType.GetCurrentAndContinue || searchType == FilterType.GetCurrentAndStop)
        {
            var resultComponent = parent.GetComponent<T>();
            if (null != resultComponent)
            {
                result.Add(resultComponent);
            }            
        }

        if (searchType == FilterType.Continue || searchType == FilterType.GetCurrentAndContinue)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                var child = parent.GetChild(i);
                child.FilterComponentsInChildrenInternal(result, onStopSearchCondition);
            }
        }
    }

    public static List<T> GetAllChildren<T>(this Transform transfrom, Func<Transform, T> onGet, Func<Transform, bool> onBreak = null)
    {
        if (null == onGet)
        {
            return null;
        }
        
        var result = new List<T>();
        var item = onGet.Invoke(transfrom);
        if (null != item)
        {
            result.Add(item);
        }

        if (null != onBreak && onBreak.Invoke(transfrom))
        {
            return result;
        }
        
        for (int i = 0; i < transfrom.childCount; i++)
        {
            var child = transfrom.GetChild(i);
            var list = child.GetAllChildren(onGet, onBreak);
            if (null != list)
            {
                result.AddRange(list);
            }
        }
        
        return result;
    }

    /// <summary>
    /// 获取物体的路径。往上找一直到父物体没有为止。
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static string GetPath(this GameObject obj)
    {
        return obj.transform.GetPath(null);
    }

    public static string GetPath(this Transform transform, Transform root)
    {
        if (null == root)
        {
            Debug.LogError("root cant be null");
            return string.Empty;
        }
        
        if (transform == root)
        {
            return string.Empty;
        }
        
        var result = transform.name;
        while (root != transform.parent)
        {
            var parent = transform.parent;
            if (null == parent)
            {
                Debug.LogError($"{transform} no path on {root}");
                return string.Empty;
            }

            result = $"{parent.name}/{result}";
            transform = parent;
        }

        return result;        
    }

#if UNITY_EDITOR

    public static long LocalIdentifierInFile(this Component component)
    {
        var inspectorModeInfo = typeof(UnityEditor.SerializedObject).GetProperty("inspectorMode",
            BindingFlags.NonPublic
            | BindingFlags.Instance);
        var so = new UnityEditor.SerializedObject(component);
        inspectorModeInfo.SetValue(so, UnityEditor.InspectorMode.Debug, null);
        var localIdProp = so.FindProperty("m_LocalIdentfierInFile");
        return localIdProp.longValue;
    }
    
#endif

}