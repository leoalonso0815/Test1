#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

public static class AssetDatabaseTools
{
    /// <summary>
    /// 从某个路径加载或新建一个配置。
    /// </summary>
    /// <param name="path"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    private static T GetOrCreate<T>(string path) where T : ScriptableObject
    {
        var settings = AssetDatabase.LoadAssetAtPath<T>(path);
        if (settings == null)
        {
            settings = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(settings, path);
            AssetDatabase.SaveAssets();
        }

        return settings;
    }
}

#endif