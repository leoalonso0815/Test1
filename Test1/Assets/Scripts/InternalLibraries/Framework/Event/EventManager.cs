using System;

// ReSharper disable once CheckNamespace
public class EventManager : MonoSingleton<EventManager>
{
    private Messenger messenger;
    public override void Init()
    {
        messenger = gameObject.AddComponent<Messenger>();
    }

    public void AddListener<T>(Action<T> action) where T : IEvent
    {
        if (null == messenger)
            return;
        messenger.AddListener(action);
    }

    public void RemoveListener<T>(Action<T> action) where T : IEvent
    {
        if (null == messenger)
            return;
        messenger.RemoveListener(action);
    }

    public void RemoveListeners<T>() where T : IEvent
    {
        if (null == messenger)
            return;
        messenger.RemoveAllListeners<T>();
    }

    public void Invoke<T>(T v = default) where T : IEvent
    {
        if (null == messenger)
            return;
        messenger.Invoke(v);
    }
    
    public void InvokeAsync<T>(T v = default) where T : IAsyncEvent
    {
        if (null == messenger)
            return;
        messenger.InvokeAsync(v);
    }
}