using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoSingleton<GameSceneManager>
{
    public override void Init()
    {
       
    }
    
    //准备做自动生成枚举
    public void LoadScenes(string scenePath)
    {
        StartCoroutine(ILoadScene(scenePath));
    }

    private IEnumerator ILoadScene(string sceneName)
    {
        var loadingPanel = UIManager.Instance.GetPanel<UILoadingPanel>();
        Resources.UnloadUnusedAssets();
        GC.Collect(2, GCCollectionMode.Forced);
        loadingPanel.UpdateSlider(0.2f);
        yield return new WaitForSeconds(0.5f);
        AsyncOperation asyncScene = SceneManager.LoadSceneAsync(sceneName);
        asyncScene.completed += OnLoadedScene;
        loadingPanel.UpdateSlider(1f);
        yield return new WaitForSeconds(0.5f);
        loadingPanel.HidePanel(false);
    }

    private void OnLoadedScene(AsyncOperation obj)
    {
        
    }
}
