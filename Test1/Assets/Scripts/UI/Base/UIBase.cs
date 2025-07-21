/*=================================================
*FileName:     UIBase.cs 
*Author:       Z_Sprite 
*UnityVersion：2018.1.6f1 
*Date:         2018-08-17 22:11 
*Description: 所有UI一级界面基类
*History: 
=================================================*/

using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public abstract class UIBase : MonoBehaviour
{
    public virtual UILayerEnum PanelLayer
    {
        get
        {
            return UILayerEnum.Base;
        }
    }

    protected virtual bool IsAdapterScreen { get => true; }

    #region 字段

    private object mTransParam;
    [HideInInspector]
    public int childPanel;
    [HideInInspector]
    public object[] refreshParam;
    [HideInInspector]
    public bool isAddCollider;

    Rect LastSafeArea = new Rect(0, 0, 0, 0);

    /// <summary> 透传参数 </summary>
    public object TransParam
    {
        get { return mTransParam; }
        set { mTransParam = value; }
    }

    #endregion

    public T Get<T>(string path, ref T obj) where T : UnityEngine.Object
    {
        return obj ?? (obj = Get<T>(path));
    }

    private T Get<T>(string path) where T : UnityEngine.Object
    {
        if (this == null) return null;
        return Utility.Get<T>(this.transform, path) as T;
    }

    private void Awake()
    {
        Init();

        //if (IsAdapterScreen)
        //{
        //    var rtTrans = transform.GetComponent<RectTransform>();
        //    if (rtTrans != null)
        //    {
        //        rtTrans.sizeDelta = new Vector2(ScreenAdaptationManager.Instance.GetScrrenWidth(), ScreenAdaptationManager.Instance.GetSafeAreaScrrenHeight());
        //        rtTrans.localPosition += Vector3.up * ScreenAdaptationManager.Instance.OffsetY();
        //    }
        //}
    }

    private void Start()
    {
        OnStat();
    }

    protected virtual void OnStat()
    {

    }
    protected virtual void RegisterMessage()
    { }

    public virtual void SetParent(Transform parent)
    {
        if (parent == null) return;
        transform.SetParent(parent);
        transform.localPosition = Vector3.zero;
        transform.localScale = Vector3.one;
    }


    protected abstract void Init();

    public abstract void Show();
    /*    {
            //Reset();
            //发送面板打开消息TODO   
        }*/

    public virtual void SelectChildPanel(int childIndex = 0)
    {
        this.childPanel = childIndex;
    }

    public virtual void RefreshData(object[] refreshParam)
    {
        this.refreshParam = refreshParam;
    }

    public virtual void HidePanel(bool isDestroy = true)
    {
        if (isDestroy)
        {
            UIManager.Instance.ClosePanel(this.GetType());
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    protected virtual void OnDestroy()
    {

    }

    private void OnMeshClick(GameObject go)
    {
        CloseBasePanel();
    }
    public virtual void CloseBasePanel()
    {
        UIManager.Instance.ClosePanel(this.GetType());
    }
}
