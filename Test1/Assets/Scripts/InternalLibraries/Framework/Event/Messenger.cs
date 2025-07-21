using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// ReSharper disable once CheckNamespace
public interface IEvent { }
public interface IAsyncEvent : IEvent { }

public class Messenger : MonoBehaviour
{
    private readonly Dictionary<Type, Delegate> eventDic = new Dictionary<Type, Delegate>();
    private readonly Queue<IInvokeAsyncEvent> invokeQueue = new Queue<IInvokeAsyncEvent>();
    private float timeSlice = 0.05f;
    private float lastSampleTime;

    private bool isBusy
    {
        get
        {
            var res = lastSampleTime - Time.realtimeSinceStartup >= timeSlice;
            if (res) lastSampleTime = Time.realtimeSinceStartup;
            return res;
        }
    }

    public void AddListener<T>(Action<T> action) where T : IEvent
    {
        if (!eventDic.TryGetValue(typeof(T), out var holder))
            eventDic.Add(typeof(T), action);
        else
        {
            if (DelegateContainsAction(holder, action)) return;
            holder = Delegate.Combine(holder, action);
            eventDic[typeof(T)] = holder;
        }
    }
    
    public void RemoveListener<T>(Action<T> action) where T : IEvent
    {
        if (!eventDic.TryGetValue(typeof(T), out var holder)) return;
        if (!DelegateContainsAction(holder, action)) return;
        holder = Delegate.Remove(holder, action);
        if (holder == null)
            eventDic.Remove(typeof(T));
        else
            eventDic[typeof(T)] = holder;
    }

    public void RemoveAllListeners<T>() where T : IEvent
    {
        if (!eventDic.ContainsKey(typeof(T))) return;
        eventDic.Remove(typeof(T));
    }
    
    public void Invoke<T>(T v) where T : IEvent
    {
        var t = typeof(T);

        if (t.GetInterface(nameof(IAsyncEvent)) != null)
        {
            Debug.LogError($"对异步事件调用了同步触发方法：{t}");
        }
        
        if (!eventDic.TryGetValue(t, out var dDelegate))
        {
            Debug.LogWarning($"#事件触发#未注册 {t.Name}");
            return;
        }

        //LogInvokedEvent(t, dDelegate);
        var action = ((Action<T>) dDelegate);
        action.Invoke(v);
    }

    private void LogInvokedEvent(Type t, Delegate del)
    {
        Debug.Log($"#事件触发#{t.Name}:  {del.GetInvocationList().Length}");
    }

    public void InvokeAsync<T>(T v) where T : IAsyncEvent
    {
        if (!eventDic.TryGetValue(typeof(T), out var dDelegate))
        {
            Debug.LogWarning($"#事件触发#未注册 {typeof(T).Name}");
            return;
        }
        var invokeList = dDelegate.GetInvocationList();
        foreach (var @delegate in invokeList)
        {
            InvokeAsyncEvent<T> invokeAsyncEvent = null;
            var contains = invokeQueue.Any(invokeEvent =>
            {
                if (!(invokeEvent is InvokeAsyncEvent<T> invokeInfo))
                    return false;
                if (invokeInfo.action == (Action<T>) @delegate)
                {
                    invokeAsyncEvent = invokeInfo;
                    return true;
                }
                return false;
            });
            if (!contains)
                invokeQueue.Enqueue(new InvokeAsyncEvent<T> {action = (Action<T>) @delegate, asyncEvent = v});
            else 
                invokeAsyncEvent.asyncEvent = v;
        }
    }

    private void Update()
    {
        if (invokeQueue.Count == 0) return;
        while (invokeQueue.Count > 0)
        {
            var invokeInfo = invokeQueue.Dequeue();
            Debug.Log($"#异步事件#{invokeInfo.GetType()}：    {invokeInfo.GetActionLength()}");
            invokeInfo.Invoke();
            if (isBusy) break;
        }
    }
    
    private bool DelegateContainsAction<T>(Delegate @delegate, Action<T> action)
    {
        var invokeList = @delegate.GetInvocationList();
        return invokeList.Contains(action);
    }
    
    private class InvokeAsyncEvent<TAsyncEvent> : IInvokeAsyncEvent where TAsyncEvent : IAsyncEvent
    {
        internal Action<TAsyncEvent> action;
        internal TAsyncEvent asyncEvent;

        public int GetActionLength()
        {
            return null == action ? 0 : action.GetInvocationList().Length;
        }

        public void Invoke() { action?.Invoke(asyncEvent); }
    }
    
    private interface IInvokeAsyncEvent
    {
        int GetActionLength();
        
        void Invoke();
    }
}