using System;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public static UIManager CreateInstance()
    {
        GameObject go = new GameObject("UIManager");
        DontDestroyOnLoad(go);
        Instance = go.AddComponent<UIManager>();
        Instance.InitData();
        return Instance;
    }

    class UIInfo
    {
        public string UIName;
        public Type UITpye;
        public System.Func<GameObject, UIBase> AddComponentAction;
        public System.Action<UIBase> callback;
        public object param;
        public UIBase mPanel;
    }

    //private List<UIInfo> mLoadingUIList;
    //private Dictionary<string, UIInfo> mLoadingUIDic;

    //private List<UIInfo> mLoadedUIList;
    //private List<string> mLoadedUINameList;
    private Dictionary<string, UIInfo> mLoadedUIDic;

    private List<UIInfo> mLoadedUnlimitedUIList;

    private List<string> mQuickTipsList;

    protected void InitData()
    {
        //===============InitEvent====================

        //===============End InitEvent===================

        //mLoadingUIList = new List<UIInfo>();
        //mLoadingUIDic = new Dictionary<string, UIInfo>();
        //mLoadedUIList = new List<UIInfo>();
        //mLoadedUINameList = new List<string>();
        mLoadedUIDic = new Dictionary<string, UIInfo>();
        mLoadedUnlimitedUIList = new List<UIInfo>();
        mQuickTipsList = new List<string>() { "UIEquipTipsMPanel", "UIAutoUsePanel" };
    }

    private Transform mRoot;

    public Transform Root
    {
        get
        {
            if (mRoot == null)
            {
                GameObject _canvas = GameObject.Find("Canvas");

                if (_canvas != null)
                {
                    mRoot = _canvas.transform;
                    Canvas canvas = _canvas.GetComponent<Canvas>();
                    if (canvas != null)
                    {
                        planeDistance = canvas.planeDistance;
                    }
                }
                else
                    UnityEngine.Debug.LogError("UI没有找到根节点！！！");
            }

            return mRoot;
        }
    }

    public Canvas CurCanvas
    {
        get
        {
            if (mRoot == null)
            {
                GameObject _canvas = GameObject.Find("Canvas");

                if (_canvas != null)
                {
                    mRoot = _canvas.transform;
                    Canvas canvas = _canvas.GetComponent<Canvas>();
                    return canvas;
                }
                else
                    UnityEngine.Debug.LogError("UI没有找到Canvas！！！");
            }

            return null;
        }
    }

    private float planeDistance = 100;

    public float PlaneDistance => planeDistance;

    public void CreatePanel<T>(System.Action<UIBase> action = null, object param = null, int audioID = 0)
        where T : UIBase
    {
        Type type = typeof(T);
        UIInfo loadUI = null;

        loadUI = new UIInfo();
        loadUI.UIName = type.Name;
        loadUI.UITpye = type;
        loadUI.callback = action;
        loadUI.AddComponentAction = (go) => go.AddComponent<T>();
        loadUI.param = param;
        string path = AssetsPathConfig.uIPanelPath + type.Name;
        var uiPanelObj = GameObjManager.Instance.CreatGameObject(path);
        InstantiateCompleted(uiPanelObj, loadUI);
        mLoadedUIDic.Add(loadUI.UIName, loadUI);
    }

    private void InstantiateCompleted(GameObject uiObj, UIInfo uiInfo)
    {
        UIBase uiComponent = uiInfo.AddComponentAction(uiObj);
        if (uiComponent != null)
        {
            uiInfo.mPanel = uiComponent;

            UILayerManager.Instance.SetLayer(uiObj, uiComponent.PanelLayer);

            uiComponent.Show();

            if (uiInfo != null)
            {
                uiComponent.TransParam = uiInfo.param;
                uiInfo.callback?.Invoke(uiComponent);
            }
        }
    }

    public void ClosePanel(Type type)
    {
        UIInfo loadUI = null;

        if (mLoadedUIDic.TryGetValue(type.Name, out var uiInfo))
        {
            GameObjManager.Instance.DestroyGameObj(uiInfo.mPanel.gameObject);
            mLoadedUIDic.Remove(type.Name);
        }
    }

    public T GetPanel<T>() where T : UIBase
    {
        Type type = typeof(T);
        if (mLoadedUIDic.TryGetValue(type.Name, out UIInfo uiinfo))
        {
            return uiinfo.mPanel as T;
        }

        return null;
    }

    public UIBase GetPanelByType(Type type)
    {
        if (mLoadedUIDic.ContainsKey(type.Name) && mLoadedUIDic[type.Name].mPanel != null)
        {
            return mLoadedUIDic[type.Name].mPanel;
        }

        return null;
    }

    public UIBase FindPanel(string name)
    {
        if (mLoadedUIDic.TryGetValue(name, out UIInfo uiinfo))
        {
            return uiinfo.mPanel;
        }

        return null;
    }

    public void CloseAllPanel() //string withoutPanelName
    {
        foreach (var uiPair in mLoadedUIDic)
        {
            GameObjManager.Instance.DestroyGameObj(uiPair.Value.mPanel.gameObject);
        }
        mLoadedUIDic.Clear();
    }

    public bool GetPanelOpenByTypes(List<Type> checkUITypes)
    {
        if (checkUITypes == null) return false;

        for (int i = 0; i < checkUITypes.Count; i++)
        {
            var uiType = checkUITypes[i];
            if (mLoadedUIDic.TryGetValue(uiType.Name, out UIInfo uiinfo)
                && uiinfo != null
                && uiinfo.mPanel != null
                && uiinfo.mPanel.gameObject.gameObject.activeSelf
                && uiinfo.mPanel.gameObject.gameObject.activeInHierarchy)
            {
                return true;
            }
        }

        return false;
    }
}