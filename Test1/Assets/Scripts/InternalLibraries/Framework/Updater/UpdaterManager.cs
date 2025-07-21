using System;
using Debug = UnityEngine.Debug;

public class UpdaterManager : MonoSingleton<UpdaterManager>
{
    private Action onUpdate;
    
    private Action onFixUpdate;
    
    private Action onLateUpdate;

    private Action onApplicationQuit;
    
    public void Init()
    {
    }
    
    public void AddUpdate(Action action)
    {
        if (IsAddCallbackValid(onUpdate, action))
        {
            onUpdate += action;
        }
    }

    public void RemoveUpdate(Action action)
    {
        if (IsRemoveCallbackValid(onUpdate, action))
        {
            onUpdate -= action;
        }
    }

    public void AddFixedUpdate(Action action)
    {
        if (IsAddCallbackValid(onFixUpdate, action))
        {
            onFixUpdate += action;
        }
    }

    public void RemoveFixedUpdate(Action action)
    {
        if (IsRemoveCallbackValid(onFixUpdate, action))
        {
            onFixUpdate -= action;
        }
    }

    public void AddLateUpdate(Action action)
    {
        if (IsAddCallbackValid(onLateUpdate, action))
        {
            onLateUpdate += action;
        }
    }

    public void RemoveLateUpdate(Action action)
    {
        if (IsRemoveCallbackValid(onLateUpdate, action))
        {
            onLateUpdate -= action;
        }
    }

    public void AddOnApplicationQuit(Action action)
    {
        if (IsAddCallbackValid(onApplicationQuit, action))
        {
            onApplicationQuit += action;
        }
    }
    
    private bool IsAddCallbackValid(Action container, Action aim)
    {
        if (aim == null)
        {
            Debug.LogError("添加 Update 回调为 null");
            return false;
        }

        if (null == container)
        {
            return true;
        }
        
        var delList = container.GetInvocationList();
        for (var i = 0; i < delList.Length; i++)
        {
            var del = delList[i];
            if (del.Equals(aim))
            {
                Debug.LogError($"添加 Update 事件重复{aim.Method.Name}");
                return false;
            }
        }

        return true;
    }
    
    private bool IsRemoveCallbackValid(Action container, Action aim)
    {
        if (aim == null)
        {
            Debug.LogError("移除 Update 回调为 null");
            return false;
        }

        if (null == container)
        {
            Debug.LogError("移除 Update，无任何回调");
            return false;
        }
        
        var delList = container.GetInvocationList();
        for (var i = 0; i < delList.Length; i++)
        {
            var del = delList[i];
            if (del.Equals(aim))
            {
                return true;
            }
        }

        Debug.LogError($"移除 Update 事件未监听{aim.Method.Name}");
        return false;
    }
    

    #region UnityEvent

    private void Update()
    {
        onUpdate?.Invoke();
    }

    private void LateUpdate()
    {
        onLateUpdate?.Invoke();
    }

    private void FixedUpdate()
    {
        onFixUpdate?.Invoke();
    }

    private void OnApplicationQuit()
    {
        onApplicationQuit?.Invoke();
    }

    #endregion
}