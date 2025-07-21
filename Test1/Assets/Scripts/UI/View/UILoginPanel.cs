using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UILoginPanel : UIBase
{
    private Button startButton;

    //临时存放启动设置
    private void Awake()
    {
        Application.targetFrameRate = 30;
        Init();
    }

    protected override void Init()
    {
        startButton = transform.Find("Buttons/StartGame").GetComponent<Button>();
        startButton.onClick.AddListener(OnStartClick);
    }

    public override void Show()
    {
        //UnityEngine.Debug.Log("UILoginPanel Show");
    }

    private void OnStartClick()
    {
        HidePanel(false);
        //SceneManager.LoadScene("Level");
        UIManager.Instance.CreatePanel<UILoadingPanel>();
        GameSceneManager.Instance.LoadScenes("Level");
    }
}