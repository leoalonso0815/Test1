using System;
using UnityEngine;
using Debug = UnityEngine.Debug;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// 可编程物体配置的单例。如果找不到会返回null。
/// todo: 通过特性来设置加载路径。
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class ScriptableObjectSingleton<T> : ScriptableObject, ISingleton where T : ScriptableObject
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (!instance)
            {
#if UNITY_EDITOR
                GetInstanceInEditor();
#else
                GetInstanceInRuntime();
#endif
            }

            return instance;
        }
    }

    public virtual void Init()
    {
    }

    public virtual void Clear()
    {
    }

    private static void GetInstanceInRuntime()
    {
        Debug.LogError($"暂时未提供运行时的资产单例");
    }

#if UNITY_EDITOR

    private static void GetInstanceInEditor()
    {
        var t = typeof(T);
        var searchType = AssetInstanceAttribute.ESearchType.ByDirectoryPath;
        var searchPath = string.Empty;
        var assetInstance = t.GetAttribute<AssetInstanceAttribute>();
        if (null == assetInstance)
        {
            Debug.LogWarning($"{t.Name}未配置资产路径");
        }
        else
        {
            searchType = assetInstance.SearchType;
            searchPath = assetInstance.Path;
        }

        switch (searchType)
        {
            case AssetInstanceAttribute.ESearchType.ByDirectoryPath:
                var searchInPath = string.IsNullOrEmpty(searchPath) ? null : new[] { searchPath };
                var guids = AssetDatabase.FindAssets($"t:{t.Name}", searchInPath);
                if (guids.Length != 1)
                {
                    Debug.LogError($"{t.Name}资产数量错误:{guids.Length}");
                    return;
                }

                var assetGuid = guids[0];
                instance = AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(assetGuid));                
                break;
            case AssetInstanceAttribute.ESearchType.ByFilePath:
                instance = AssetDatabase.LoadAssetAtPath<T>(searchPath);
                break;
        }
    }

#endif
}