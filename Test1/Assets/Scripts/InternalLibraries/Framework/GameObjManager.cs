using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjManager : MonoSingleton<GameObjManager>
{
    //路径，<code,资源实例>
    private Dictionary<string, Dictionary<int, GameObject>> gameObjDic;

    public override void Init()
    {
        gameObjDic = new Dictionary<string, Dictionary<int, GameObject>>(10);
    }

    public GameObject CreatGameObject(string assetPath, Transform parentTra = null)
    {
        if (parentTra == null)
        {
            parentTra = gameObject.transform;
        }
        GameObject creatObj = null;
        if (gameObjDic.TryGetValue(assetPath, out var objDic))
        {
            GameObject assetObj = AsstesManager.Instance.LoadAsset<GameObject>(assetPath);
            creatObj = Instantiate(assetObj, parentTra);
            objDic.Add(creatObj.GetHashCode(), creatObj);
        }
        else
        {
            var insObjDic = new Dictionary<int, GameObject>(10);
            Debug.Log("assetPath ===" + assetPath);
            var assetObj = AsstesManager.Instance.LoadAsset<GameObject>(assetPath);
            creatObj = Instantiate(assetObj, parentTra);
            insObjDic.Add(creatObj.GetHashCode(), creatObj);
            gameObjDic.Add(assetPath, insObjDic);
        }

        return creatObj;
    }

    public GameObject CreatGameObject(GameObject assetObj, Transform parentTra = null)
    {
        GameObject insObj = null;
        int code = assetObj.GetHashCode();
        foreach (var pathPair in gameObjDic)
        {
            if (pathPair.Value.ContainsKey(code))
            {
                string path = pathPair.Key;
                insObj = CreatGameObject(path, parentTra);
                return insObj;
            }
        }
        Debug.LogError("目标资源不存在");
        return insObj;
    }

    public void DestroyGameObj(GameObject insObj)
    {
        string removeKey = "";
        int insCode = insObj.GetHashCode();
        foreach (var pathPair in gameObjDic)
        {
            if (pathPair.Value.TryGetValue(insCode, out var targetObj))
            {
                DestroyImmediate(targetObj);
                if (pathPair.Value.Count <= 1)
                {
                    removeKey = pathPair.Key;
                }
                else
                {
                    pathPair.Value.Remove(insCode);
                }
                break;
            }
        }

        if (removeKey != string.Empty)
        {
            Debug.Log("removeKey === " + removeKey);
            AsstesManager.Instance.UnloadOneAsset(removeKey);
            gameObjDic.Remove(removeKey);
        }
    }
}