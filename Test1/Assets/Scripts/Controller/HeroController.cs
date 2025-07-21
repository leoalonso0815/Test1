using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

public class HeroController : BaseController
{
    private float rotationSpeed = 10f;
    private float cooldownTime = 0.8f;
    private float cooldownTimer = 0f;

    /// <summary>
    /// 角色头顶位置
    /// </summary>
    private Transform heroHead;

    /// <summary>
    /// 角色模型
    /// </summary>
    private Transform heroModel;

    /// <summary>
    /// 角色脚下位置(地面)
    /// </summary>
    private Transform heroFoot;

    private Transform heroBack;
    private Transform steakTopTrans;

    private bool isMonsterInAttackRange = false;

    private float steakPosYOffset = 0.06f;

    void Awake()
    {
        rotationSpeed = 15f;
        cooldownTimer = 0.8f;
        Init();
    }

    internal override void Init()
    {
        base.Init();
        heroHead = transform.Find("HeroHead");
        heroModel = transform.Find("HeroModel");
        heroFoot = transform.Find("HeroFoot");
        heroBack = transform.Find("HeroBack");
        steakTopTrans = heroBack.Find("SteakTop");
        HeroHelper.SetHero(heroHead);
        HeroHelper.SetHeroFoot(heroFoot);
        HeroHelper.SetHeroSteakPosTrans(steakTopTrans);
        InitCharacter();
        if (!animeController)
        {
            animeController = heroModel.AddComponent<AnimeController>();
            animeController.Init(true);
        }

        EventManager.Instance.AddListener<MonsterDeathAddSteakSpawnEvent>(OnMonsterDeathSteakSpawn);
        EventManager.Instance.AddListener<MonsterDeathAddSteakEvent>(OnMonsterDeathAddSteak);
    }

    /// <summary>
    /// 初始化角色
    /// </summary>
    internal override void InitCharacter()
    {
        base.InitCharacter();
        var id = CharacterConfig.heroId;
        characterData = CharacterData.CreateNewCharacterData(id, 100, 50, 5, 100);
        CharacterManager.Instance.SetHeroCharacterData(characterData);
        //CharacterManager.Instance.AddNewCharacterTransform(characterData.Id, transform);
        EventManager.Instance.Invoke<HeroInitEvent>();
    }

    void Update()
    {
        if (characterState == CharacterState.Death)
        {
            return;
        }

        if (JoyStickHelper.GetJoyStickState() == JoyStickHelper.JoyStickState.Move)
        {
            var outPos = JoyStickHelper.GetCurJoyStickPos();
            if (characterState != CharacterState.Run)
            {
                characterState = CharacterState.Run;
                animeController.SwitchAnime("Run", 0.05f);
            }

            var forwardTarget = GetForwardTarget(outPos);
            transform.forward = Vector3.Lerp(transform.forward, forwardTarget, Time.deltaTime * rotationSpeed);
            transform.Translate(Vector3.forward * (characterData.Speed * Time.deltaTime));
        }
        else
        {
            if (characterState != CharacterState.Idle)
            {
                characterState = CharacterState.Idle;
                animeController.SwitchAnime("Idle", 0.2f);
            }
        }

        CalculateAttackRangeIsHaveMonster();
        //Debug.Log("angle isMonsterInAttackRange === " + isMonsterInAttackRange);

        cooldownTimer += Time.deltaTime;
        if (isMonsterInAttackRange)
        {
            characterAttackState = CharacterAttackState.Start;

            if (cooldownTimer >= cooldownTime)
            {
                animeController.SwitchAnime("Attack", 0.2f);
                cooldownTimer = 0;
            }
        }
        else
        {
            if (characterAttackState != CharacterAttackState.Stop)
            {
                characterAttackState = CharacterAttackState.Stop;
                animeController.SwitchAnime("AttackStop");
            }
        }
    }

    /// <summary>
    /// 根据摇杆方向获取人物朝向
    /// </summary>
    /// <param name="outPos"></param>
    /// <returns></returns>
    private Vector3 GetForwardTarget(Vector2 outPos)
    {
        var rotAngle = Vector3.Angle((outPos - Vector2.zero), Vector3.up);
        if (outPos.x < 0)
        {
            rotAngle = 360 - rotAngle;
        }

        return Quaternion.AngleAxis(rotAngle, Vector3.up) * Vector3.forward;
    }

    /// <summary>
    /// 受伤
    /// </summary>
    /// <param name="damage"></param>
    public override void Hurt(float damage)
    {
        base.Hurt(damage);
        CharacterManager.Instance.ChangeHeroCurHp(damage);
        if (characterData.CurHp <= 0)
        {
            Death();
        }

        var curRatio = characterData.CurHp / characterData.MaxHp;
        EventManager.Instance.Invoke(new HeroHurtEvent
            { curHp = characterData.CurHp, maxHp = characterData.MaxHp, HeroHpRatio = curRatio });
    }

    /// <summary>
    /// 角色死亡
    /// </summary>
    internal override void Death()
    {
        base.Death();
        EventManager.Instance.Invoke<HeroDeathEvent>();
        if (characterState == CharacterState.Death) return;
        heroBack.gameObject.SetActive(false);
        CharacterManager.Instance.SetHeroCharacterDataDeath();
        characterState = CharacterState.Death;
        characterAttackState = CharacterAttackState.Stop;
        animeController.SwitchAnime("AttackStop");
        animeController.SwitchAnime("Death", 0.1f);
    }

    /// <summary>
    /// 死亡动画播放结束时处理
    /// </summary>
    public override void DeathEnd()
    {
        base.DeathEnd();
        //EventManager.Instance.Invoke(new HeroDeathEvent());
    }

    /// <summary>
    /// 获取是否有怪物在攻击范围内
    /// </summary>
    private void CalculateAttackRangeIsHaveMonster()
    {
        if (CharacterManager.Instance.CharactersTransDic.Count <= 0)
        {
            isMonsterInAttackRange = false;
            return;
        }

        for (int i = 0; i < CharacterManager.Instance.CharactersTransDic.Count; i++)
        {
            isMonsterInAttackRange = false;
            var id = CharacterManager.Instance.CharactersTransDic.ElementAt(i).Key;
            if (CharacterManager.Instance.CheckIsCharacterDead(id))
            {
                continue;
            }

            var trans = CharacterManager.Instance.CharactersTransDic.ElementAt(i).Value;
            var distance = Vector3.Distance(trans.position, transform.position);
            Vector3 toMonster = (trans.position - transform.position).normalized;
            //float dotResult = Vector3.Dot(playerForward, toMonster);
            float angle = Vector3.Angle(transform.forward, toMonster);
            //Debug.Log("angle === " + angle);
            // 绘制角色前方（蓝色）
            //Debug.DrawRay(transform.position, transform.forward * 5, Color.blue);
            // 绘制到怪物的方向（红色）
            //Debug.DrawRay(transform.position, toMonster * 5, Color.red);
            if (distance <= 1.5f && angle <= 80)
            {
                isMonsterInAttackRange = true;
                break;
            }
        }
    }

    /// <summary>
    /// 怪物死亡时获得肉排处理
    /// </summary>
    /// <param name="e"></param>
    private void OnMonsterDeathAddSteak(MonsterDeathAddSteakEvent e)
    {
        CharacterManager.Instance.AddHeroCharacterDataPorkSteak();
        // if (steakTransforms.Count > 0)
        // {
        //     steakTopTrans.localPosition = new Vector3(0, steakTopTrans.localPosition.y + steakPosYOffset, 0);
        // }
    }

    private void OnMonsterDeathSteakSpawn(MonsterDeathAddSteakSpawnEvent e)
    {
        var spawnPosition = Vector3.zero;
        if (HeroHelper.SteakTransforms.Count > 0)
        {
            spawnPosition = new Vector3(0, (HeroHelper.SteakTransforms.Count - 1) * steakPosYOffset, 0);
        }

        var steakGameObject =
            ObjectPool.Instance.SpawnFromPoolToLocalPosition("Steak", heroBack, spawnPosition, quaternion.identity);

        if (!steakGameObject)
        {
            return;
        }
        
        HeroHelper.AddHeroSteakTrans(steakGameObject.transform);
    }
}