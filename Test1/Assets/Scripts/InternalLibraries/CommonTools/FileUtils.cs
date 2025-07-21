#if UNITY_EDITOR
using System.IO;
using UnityEngine;

public static class FileUtils 
{
    public static void LockFile(string filePath)
    {
        if (File.Exists(filePath))
        {
            File.SetAttributes(filePath, File.GetAttributes(filePath) | FileAttributes.ReadOnly);
        }
    }

    public static void UnLockFile(string filePath)
    {
        if (File.Exists(filePath))
        {
            File.SetAttributes(filePath, File.GetAttributes(filePath) & (~FileAttributes.ReadOnly));
        }
    }

    public static string GetFullPath(string relativePathToCurrentDir)
    {
        var current = Directory.GetCurrentDirectory();
        var path = Path.GetFullPath(Path.Combine(current, relativePathToCurrentDir));
        return path;
    }
    
    public static void SetFileReadOnly()
    {
        var objs = UnityEditor.Selection.objects;
        if (objs.IsNullOrEmpty())
        {
            return;
        }

        var first = objs[0];
        var path = UnityEditor.AssetDatabase.GetAssetPath(first.GetInstanceID());
        if (path.Length > 0)
        {
            path = Path.Combine(Directory.GetCurrentDirectory(), path).Replace("/", "\\");
            if (Directory.Exists(path))
            {
                Debug.Log($"{first.name} : {first.GetType()} : {path}");
            }
            else
            {
                if (File.Exists(path))
                {
                    Debug.Log($"{first.name} : {first.GetType()} : {path}");
                    File.SetAttributes(path, File.GetAttributes(path) | FileAttributes.ReadOnly);
                }
            }
        }
    }
    
    public static void SetFileUnlock()
    {
        var objs = UnityEditor.Selection.objects;
        if (objs.IsNullOrEmpty())
        {
            return;
        }

        var first = objs[0];
        var path = UnityEditor.AssetDatabase.GetAssetPath(first.GetInstanceID());
        if (path.Length > 0)
        {
            path = Path.Combine(Directory.GetCurrentDirectory(), path).Replace("/", "\\");
            if (Directory.Exists(path))
            {
                Debug.Log($"{first.name} : {first.GetType()} : {path}");
            }
            else
            {
                if (File.Exists(path))
                {
                    Debug.Log($"{first.name} : {first.GetType()} : {path}");
                    File.SetAttributes(path, File.GetAttributes(path) & (~FileAttributes.ReadOnly));
                }
            }
        }
    }
}
#endif
