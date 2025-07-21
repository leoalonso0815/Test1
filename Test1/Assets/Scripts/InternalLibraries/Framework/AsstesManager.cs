using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class AsstesManager : MonoSingleton<AsstesManager>
{
    //路径，资源
    private Dictionary<string, Object> loadObjDic;

    public override void Init()
    {
        loadObjDic = new Dictionary<string, Object>();
    }

    public T LoadAsset<T>(string path) where T : Object
    {
        if (loadObjDic.TryGetValue(path, out Object oldAssetObj))
        {
            return oldAssetObj as T;
        }

        var result = Resources.Load<T>(path);
        if (result == null)
        {
            Debug.LogError($"资源加载失败:{path}");
            return null;
        }
        loadObjDic.Add(path, result);
        return result;
    }
    
    public void AsyncLoadAsset<T>(string path,Action<object> completedAction) where T : Object
    {
        StartCoroutine(StartAsyncLoadAsset(path,completedAction));
    }

    private IEnumerator StartAsyncLoadAsset(string path,Action<object> completedAction)
    {
        var loadAsset = Resources.LoadAsync(path);
        while (!loadAsset.isDone)
        {
            yield return new WaitForEndOfFrame();
        }
        
        completedAction?.Invoke(loadAsset.asset);
        loadObjDic.Add(path, loadAsset.asset);
    }

    public void UnloadOneAsset(string path)
    {
        if (loadObjDic.TryGetValue(path,out var loadObj))
        {
            Resources.UnloadAsset(loadObj);
        }
    }

    public void UnloadAllAsset()
    {
        foreach (var pair in loadObjDic)
        {
            Resources.UnloadAsset(pair.Value);
        }

        loadObjDic = new Dictionary<string, Object>();
    }
}