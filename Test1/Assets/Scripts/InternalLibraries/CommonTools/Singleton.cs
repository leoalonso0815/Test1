using System;
using UnityEngine;

public interface ISingleton
{
    void Init();

    void Clear();
}

public abstract class Singleton<T> : ISingleton where T : class, new()
{
    private static T mInstance;

    public static T Instance
    {
        get
        {
            if (mInstance == null)
            {
                mInstance = new T();
                Debug.Log($"#单例#Init {mInstance.GetType().Name}");
                Application.quitting += OnApplicationQuit;
            }

            return mInstance;
        }
    }

    private static void OnApplicationQuit()
    {
        mInstance = null;
    }

    public virtual void Init()
    {
    }

    public virtual void Clear()
    {
    }
}

public abstract class MonoSingleton<T> : MonoBehaviour, ISingleton where T : MonoSingleton<T>
{
    private static T mInstance = null;
    
    /// <summary>
    /// <see cref="https://hextantstudios.com/unity-singletons/"/>>
    /// </summary>
    private static bool mDestroyed = false;

    public static T Instance
    {
        get
        {
            if (mInstance == null && !mDestroyed && UnityEngine.Application.isPlaying)
            {
                mInstance = GameObject.FindObjectOfType(typeof(T)) as T;
                if (mInstance != null) return mInstance;

                GameObject go = new GameObject(typeof(T).Name);
                mInstance = go.AddComponent<T>();
                GameObject parent = GameObject.Find("Boot");
                if (parent == null)
                {
                    parent = new GameObject("Boot");
                    GameObject.DontDestroyOnLoad(parent);
                }

                if (parent != null)
                {
                    go.transform.parent = parent.transform;
                }
            }

            return mInstance;
        }
    }

    private void Awake()
    {
        if (mInstance == null)
        {
            mInstance = this as T;
        }

        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        mDestroyed = true;
    }

    public virtual void Init()
    {
    }

    public virtual void Clear()
    {
    }
}