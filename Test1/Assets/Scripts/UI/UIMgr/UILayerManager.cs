/*=================================================
*FileName:     UILayerManager.cs
*Author:       Z_Sprite
*UnityVersion：2018.2.12f1
*Date:         2018-11-03 17:48
*Description:  管理UI层级
*History:
=================================================*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILayerManager : MonoSingleton<UILayerManager>
{
    private Transform mUIRoot;

    public Transform UIRoot
    {
        get
        {
            if (mUIRoot == null)
            {
                GameObject _canvas = GameObject.Find("Canvas");

                if (_canvas != null)
                {
                    mUIRoot = _canvas.transform;
                }
                else
                    UnityEngine.Debug.LogError("UI没有找到根节点！！！");
            }

            return mUIRoot;
        }
    }

    private Dictionary<int, Transform> mLayerDic;

    public override void Init()
    {
        base.Init();
        if (mLayerDic == null)
            mLayerDic = new Dictionary<int, Transform>();
        InitUILayer();
    }

    private void InitUILayer()
    {
        GameObject.DontDestroyOnLoad(UIRoot);
        int _nums = Enum.GetNames(typeof(UILayerEnum)).Length;

        for (int i = 0; i < _nums; i++)
        {
            object _obj = Enum.GetValues(typeof(UILayerEnum)).GetValue(i);
            int _key = Convert.ToInt32(_obj);

            if (mLayerDic.ContainsKey(_key))
                mLayerDic[_key] = CreateLayerObj(_obj.ToString(), (UILayerEnum)_obj);
            else
                mLayerDic.Add(_key, CreateLayerObj(_obj.ToString(), (UILayerEnum)_obj));
        }
    }

    private Transform CreateLayerObj(string name, UILayerEnum enu)
    {
        GameObject _tempGo = new GameObject(name);
        RectTransform _tempTrans = _tempGo.AddComponent<RectTransform>();
        _tempTrans.SetParent(UIRoot.transform, false);
        _tempTrans.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, 0);
        _tempTrans.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 0, 0);
        _tempTrans.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, 0);
        _tempTrans.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 0, 0);
        _tempTrans.anchorMin = new Vector2(0, 0);
        _tempTrans.anchorMax = new Vector2(1, 1);
        _tempTrans.localPosition = Vector3.zero;
        _tempTrans.localScale = Vector3.one;

        return _tempTrans;
    }

    public void SetLayer(GameObject panel, UILayerEnum layer)
    {
        if (mLayerDic.Count < Enum.GetNames(typeof(UILayerEnum)).Length)
        {
            InitUILayer();
        }

        int _layerType = (int)layer;
        Transform _layerTran = mLayerDic[_layerType];

        panel.transform.SetParent(_layerTran, false);
    }

    public Transform GetLayer(UILayerEnum layer)
    {
        if (mLayerDic.Count < Enum.GetNames(typeof(UILayerEnum)).Length)
        {
            InitUILayer();
        }

        int _layerType = (int)layer;
        if (this.mLayerDic.ContainsKey(_layerType))
        {
            return this.mLayerDic[_layerType];
        }

        return null;
    }

    public int GetLayerChildNums(UILayerEnum layer)
    {
        if (mLayerDic.Count < Enum.GetNames(typeof(UILayerEnum)).Length)
        {
            InitUILayer();
        }

        int _layerType = (int)layer;
        int num = 0;
        if (this.mLayerDic.ContainsKey(_layerType))
        {
            foreach (RectTransform obj in this.mLayerDic[_layerType].GetComponentsInChildren<RectTransform>())
            {
                if (obj && obj.parent && obj.parent == this.mLayerDic[_layerType] && obj.gameObject.activeSelf) num++;
            }

            return num;
        }

        return 0;
    }
}