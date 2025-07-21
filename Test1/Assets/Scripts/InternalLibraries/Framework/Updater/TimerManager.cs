using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 技能系统的计时器
/// </summary>
public class TimerManager : MonoSingleton<TimerManager>
{
    public class Timer
    {
        private Action callback;
    
        private readonly float time;
    
        private float dt = 0f;

        public int id { get; }

        public bool IsInvoked
        {
            get { return null == callback; }
        }

        public Timer(float time, Action callback)
        {
            this.time = time;
            this.callback = callback;
            this.id = Time.frameCount;
        }

        public bool Update(float deltaTime)
        {
            if (IsInvoked)
            {
                return false;
            }
            dt += deltaTime;
            if (dt >= time)
            {
                return true;
            }

            return false;
        }

        public void Invoke()
        {
            // Debug.Log($"#Timer#{Time.frameCount}触发开始:  timer:{id}    {GetHashCode()}");
            callback?.Invoke();
            // Debug.Log($"#Timer#{Time.frameCount}触发结束:  timer:{id}    {GetHashCode()}");
            callback = null;
        }

        public void Kill()
        {
            callback = null;
        }
    }
    
    private List<Timer> timers = new List<Timer>();

    /// <summary>
    /// 统计同时在更新的Timer的最大数量。
    /// </summary>
    private int maxTimerNum = 0;
    
    public override void Init()
    {
        if (null == timers)
        {
            timers = new List<Timer>();
        }
    }

    private void Awake()
    {
        Init();
    }

    public override void Clear()
    {
        timers.Clear();
        LogMaxTimerNum();
    }

    private void Update()
    {
        maxTimerNum = Math.Max(maxTimerNum, timers.Count);
        var dt = 0.0333f * Time.timeScale;
        for (var i = 0; i < timers.Count; i++)
        {
            var timer = timers[i];
            var isTrigger = timer.Update(dt);
            if (isTrigger)
            {
                timers.RemoveAt(i);
                i--;
                timer.Invoke();
            }
            
            if (null == timers || timers.Count == 0)
            {
                break;  // 防止在Timer触发的回调事件里调用了TimerManager.Clear后的报错。
            }
        }
    }

    public Timer AddTimer(float time, Action callback)
    {
        var timer = new Timer(time, callback);
        timers.Add(timer);
        if (0 >= time)
        {
            timer.Invoke();
        }
        return timer;
    }

    public void LogMaxTimerNum()
    {
        Debug.Log($"Timers max num is {maxTimerNum}");
    }

    void OnDestroy()
    {
        LogMaxTimerNum();
    }
}