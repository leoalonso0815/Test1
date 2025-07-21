using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStart : MonoBehaviour
{
    private void Start()
    {
        AppSetting();
    }
    
    private void AppSetting()
    {
        EventManager.Instance.Init();
        UpdaterManager.Instance.Init();
        TimerManager.Instance.Init();
        AsstesManager.Instance.Init();
        GameObjManager.Instance.Init();
        UILayerManager.Instance.Init();
        UIManager.CreateInstance();
        
        UIManager.Instance.CreatePanel<UILoginPanel>();
    }
}
