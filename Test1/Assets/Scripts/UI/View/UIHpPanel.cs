using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHpPanel : UIBase
{
    private RectTransform heroHp;
    private Transform monsterHpBarParent;
    private Image heroHpBar;
    private Camera mainCamera;
    private Dictionary<int, CharacterHpBarData> monsterHpBarDic = new();

    /// <summary>
    /// hero头顶血条偏移量
    /// </summary>
    private int heroHpBarOffset = 35;

    /// <summary>
    /// monster头顶血条偏移量
    /// </summary>
    private int monsterHpOffset = 70;

    protected override void Init()
    {
        EventManager.Instance.AddListener<HeroInitEvent>(SetHpBarPos);
        EventManager.Instance.AddListener<HeroHurtEvent>(OnHeroHurt);
        EventManager.Instance.AddListener<MonsterInitEvent>(SetMonsterHpBarPos);
        EventManager.Instance.AddListener<MonsterHurtEvent>(OnMonsterHurt);
        EventManager.Instance.AddListener<MonsterDeathEvent>(OnMonsterDeath);
        mainCamera = Camera.main;
        monsterHpBarParent = transform.Find("MonsterHpBarParent");
        heroHp = transform.Find("HeroHp").GetComponent<RectTransform>();
        heroHpBar = heroHp.Find("HpBar").GetComponent<Image>();
        heroHpBar.color = Color.green;
    }

    public override void Show()
    {
    }

    private void Update()
    {
        //if (JoyStickHelper.GetJoyStickState() != JoyStickHelper.JoyStickState.Move) return;
        if (!mainCamera)
        {
            return;
        }

        SetHpBarPos();
        SetMonsterHpBarPos();
    }

    /// <summary>
    /// 主角初始化时设置足额条初始位置
    /// </summary>
    /// <param name="e"></param>
    private void SetHpBarPos(HeroInitEvent e)
    {
        SetHpBarPos();
        EventManager.Instance.RemoveListener<HeroInitEvent>(SetHpBarPos);
    }

    /// <summary>
    /// 设置主角血条位置
    /// </summary>
    private void SetHpBarPos()
    {
        var screenPos = mainCamera.WorldToScreenPoint(HeroHelper.CurHero.position);
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(transform as RectTransform, screenPos, null,
                out var uiPosition))
        {
            heroHp.position = new Vector3(uiPosition.x, uiPosition.y + heroHpBarOffset, 0);
        }
    }

    /// <summary>
    /// 敌人生成时初始化头顶血条
    /// </summary>
    /// <param name="e"></param>
    private void SetMonsterHpBarPos(MonsterInitEvent e)
    {
        var monsterId = e.MonsterID;
        var monsterTransform = CharacterManager.Instance.GetCharacterTransformById(monsterId);
        var screenPos = mainCamera.WorldToScreenPoint(monsterTransform.position);
        var barPos = Vector3.zero;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(transform as RectTransform, screenPos, null,
                out var uiPosition))
        {
            barPos.x = uiPosition.x;
            barPos.y = uiPosition.y + monsterHpOffset;
        }

        //GameObjManager.Instance.CreatGameObject(monsterHpPrefabPath, monsterHpBarParent).transform;
        var monsterHpBar =
            ObjectPool.Instance.SpawnFromPool("MonsterHpBar", monsterHpBarParent, barPos).transform;

        if (!monsterHpBar)
        {
            return;
        }
        
        var monsterHpBarImage = monsterHpBar.Find("HpBar").GetComponent<Image>();
        monsterHpBarImage.fillAmount = 1;
        var characterHpBarData =
            CharacterHpBarData.CreateNewCharacterHpBarData(monsterId, monsterHpBar, monsterHpBarImage);
        monsterHpBarDic.TryAdd(monsterId, characterHpBarData);
    }

    /// <summary>
    /// 更新怪物血条位置
    /// </summary>
    /// <param name="monsterId"></param>
    /// <param name="monsterHpBarrData"></param>
    private void OnUpdateSetMonsterHpBarPos(int monsterId, CharacterHpBarData monsterHpBarrData)
    {
        var monsterTransform = CharacterManager.Instance.GetCharacterTransformById(monsterId);
        var screenPos = mainCamera.WorldToScreenPoint(monsterTransform.position);
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(transform as RectTransform, screenPos, null,
                out var uiPosition))
        {
            monsterHpBarrData.HpBarTransform.position = new Vector3(uiPosition.x, uiPosition.y + monsterHpOffset, 0);
        }
    }

    /// <summary>
    /// 更新怪物血条位置
    /// </summary>
    private void SetMonsterHpBarPos()
    {
        monsterHpBarDic.Foreach(OnUpdateSetMonsterHpBarPos);
    }

    /// <summary>
    /// 怪物受伤时更新血量显示
    /// </summary>
    /// <param name="e"></param>
    private void OnMonsterHurt(MonsterHurtEvent e)
    {
        if (monsterHpBarDic.TryGetValue(e.MonsterID, out var hpBarData))
        {
            hpBarData.HpBarImage.fillAmount = e.MonsterHpRatio;
        }
    }

    /// <summary>
    /// 怪物死亡时血条处理
    /// </summary>
    /// <param name="e"></param>
    private void OnMonsterDeath(MonsterDeathEvent e)
    {
        if (!monsterHpBarDic.TryGetValue(e.MonsterID, out var hpBarData)) return;
        ObjectPool.Instance.ReturnToPool("MonsterHpBar", hpBarData.HpBarTransform.gameObject);
        monsterHpBarDic.Remove(e.MonsterID);
    }

    /// <summary>
    /// 主角受伤时更新血量显示
    /// </summary>
    /// <param name="e"></param>
    private void OnHeroHurt(HeroHurtEvent e)
    {
        var ratio = e.HeroHpRatio;
        heroHpBar.fillAmount = ratio;
        if (ratio is >= 0.3f and < 0.5f)
        {
            heroHpBar.color = Color.yellow;
        }
        else if (ratio < 0.3f)
        {
            heroHpBar.color = Color.red;
        }
    }
}