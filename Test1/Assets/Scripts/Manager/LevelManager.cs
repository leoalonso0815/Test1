using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private void Awake()
    {
        UIManager.Instance.CreatePanel<UILevelPanel>();
        UIManager.Instance.CreatePanel<UIHpPanel>();
        EventManager.Instance.AddListener<HeroDeathEndEvent>(OnHeroDeath);
    }

    private void OnHeroDeath(HeroDeathEndEvent e)
    {
        UIManager.Instance.CreatePanel<UIGameOverPanel>();
        EventManager.Instance.RemoveListener<HeroDeathEndEvent>(OnHeroDeath);
    }
}