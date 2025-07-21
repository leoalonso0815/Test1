using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityExtensions.Tween;

public class MonsterController : BaseController
{
    private CharacterData heroData;
    private Transform monsterModel;
    private bool isHeroDead = false;

    /// <summary>
    /// 怪物追踪主角的最大距离
    /// </summary>
    private float serchRange = 4.5f;

    /// <summary>
    /// 怪物开始攻击的距离
    /// </summary>
    private float attackRange = 1.8f;

    /// <summary>
    /// 旋转朝向玩家的速度
    /// </summary>
    private float rotationSpeed = 5f;

    /// <summary>
    /// 停止距离
    /// </summary>
    private float stoppingDistance = 1.2f;

    /// <summary>
    /// 加速度
    /// </summary>
    private float acceleration = 5f;

    /// <summary>
    /// 检测其他怪物的范围
    /// </summary>
    private float detectionRadius = 2f;

    /// <summary>
    /// 最大排斥力强度
    /// </summary>
    private float maxRepulsionForce = 1.2f;

    /// <summary>
    /// 排斥起始距离
    /// </summary>
    private float repulsionStartDistance = 1.2f;

    /// <summary>
    /// 最大排斥距离
    /// </summary>
    private float repulsionMaxDistance = 0.6f;

    /// <summary>
    /// 排斥力平滑
    /// </summary>
    private float repulsionSmoothness = 0.5f; //0.1f - 1f

    private Rigidbody rb;
    private Vector3 currentVelocity;
    private Vector3 smoothedRepulsion;
    private Transform steakPosTrans;

    void Awake()
    {
        Init();
    }

    internal override void Init()
    {
        base.Init();
        characterState = CharacterState.Idle;
        monsterModel = transform.Find("MonsterModel");
        steakPosTrans = transform.Find("SteakPos");
        rb = transform.GetComponent<Rigidbody>();
        rb.linearDamping = 6f;
        rb.angularDamping = 12f;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        // 添加物理材质实现更平滑的碰撞
        var col = GetComponent<Collider>();
        if (col)
        {
            var mat = new PhysicsMaterial
            {
                dynamicFriction = 0.1f,
                staticFriction = 0.1f,
                bounciness = 0.05f, // 极低的弹性
                frictionCombine = PhysicsMaterialCombine.Minimum
            };
            col.material = mat;
        }

        InitCharacter();
        if (!animeController)
        {
            animeController = monsterModel.AddComponent<AnimeController>();
            animeController.Init();
        }

        EventManager.Instance.AddListener<HeroDeathEvent>(OnHeroDeath);
    }

    /// <summary>
    /// 初始化角色
    /// </summary>
    internal override void InitCharacter()
    {
        base.InitCharacter();
        isHeroDead = CharacterManager.Instance.CheckIsHeroDead();
        characterData = CharacterManager.Instance.AddNewCharacterData(100, 1, 5, 5);
        if (characterData != null)
        {
            //transform.name = $"{id}";
            characterState = CharacterState.Idle;
            CharacterManager.Instance.AddNewCharacterTransform(characterData.Id, transform);
            EventManager.Instance.Invoke(new MonsterInitEvent { MonsterID = characterData.Id });
        }
        else
        {
            ObjectPool.Instance.ReturnToPool("Boar", gameObject);
        }
    }

    public void ReInitCharacter()
    {
        InitCharacter();
    }

    public CharacterData GetCharacterData()
    {
        return characterData;
    }

    void FixedUpdate()
    {
        if (characterData == null || characterState == CharacterState.Death)
        {
            return;
        }

        var distanceToHero = Vector3.Distance(transform.position, HeroHelper.CurHeroFoot.position);
        if (distanceToHero >= serchRange)
        {
            Idle();
        }
        else
        {
            if (isHeroDead)
            {
                return;
            }

            Vector3 directionToPlayer = (HeroHelper.CurHeroFoot.position - transform.position).normalized;
            directionToPlayer.y = 0;
            // 旋转朝向玩家
            if (directionToPlayer != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
                transform.rotation =
                    Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            if (distanceToHero < serchRange && distanceToHero >= attackRange)
            {
                if (characterState != CharacterState.Run)
                {
                    characterState = CharacterState.Run;
                    animeController.SwitchAnime("Run", 0.05f);
                }

                // 计算排斥力
                Vector3 repulsion = CalculateRepulsion();

                // 平滑排斥力
                smoothedRepulsion = Vector3.Lerp(smoothedRepulsion, repulsion, repulsionSmoothness);

                // 计算与玩家的距离
                float distanceToPlayer = Vector3.Distance(transform.position, HeroHelper.CurHeroFoot.position);

                // 移动逻辑：当距离大于停止距离时前进
                Vector3 targetMoveDirection = Vector3.zero;
                if (distanceToPlayer > stoppingDistance)
                {
                    targetMoveDirection = directionToPlayer * characterData.Speed;
                }

                // 应用加速度
                Vector3 targetVelocity = targetMoveDirection + smoothedRepulsion;
                currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity, acceleration * Time.deltaTime);

                // 应用最终速度
                rb.linearVelocity = new Vector3(currentVelocity.x, rb.linearVelocity.y, currentVelocity.z);
            }
            else if (distanceToHero < attackRange)
            {
                if (!isHeroDead)
                {
                    Attack();
                }
                else
                {
                    Idle();
                }
            }
        }
    }

    /// <summary>
    /// 计算排斥力
    /// </summary>
    /// <returns></returns>
    private Vector3 CalculateRepulsion()
    {
        Vector3 totalRepulsion = Vector3.zero;
        int neighborCount = 0;

        // 检测周围怪物
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius);
        foreach (Collider col in colliders)
        {
            if (col.CompareTag("Enemy") && col.gameObject != this.gameObject)
            {
                // 计算远离邻居的方向
                Vector3 toNeighbor = col.transform.position - transform.position;
                float distance = toNeighbor.magnitude;

                // 忽略过远或位置相同的怪物
                if (distance > detectionRadius || distance < 0.01f) continue;

                Vector3 awayDirection = -toNeighbor.normalized;

                // 使用平滑曲线计算排斥强度
                float repulsionStrength = 0;
                if (distance < repulsionMaxDistance)
                {
                    // 近距离：线性排斥
                    repulsionStrength = Mathf.Lerp(maxRepulsionForce, 0, distance / repulsionMaxDistance);
                }
                else if (distance < repulsionStartDistance)
                {
                    // 中距离：二次曲线（更平滑）
                    float t = (distance - repulsionMaxDistance) / (repulsionStartDistance - repulsionMaxDistance);
                    repulsionStrength = Mathf.Lerp(maxRepulsionForce, 0, t * t);
                }

                totalRepulsion += awayDirection * repulsionStrength;
                neighborCount++;
            }
        }

        // 平均排斥力
        if (neighborCount > 0) totalRepulsion /= neighborCount;

        return totalRepulsion;
    }

    private void Attack()
    {
        if (characterState != CharacterState.Attack)
        {
            characterState = CharacterState.Attack;
            animeController.SwitchAnime("Attack", 0.05f);
        }
    }

    private void Idle()
    {
        if (characterState != CharacterState.Idle)
        {
            characterState = CharacterState.Idle;
            animeController.SwitchAnime("Idle", 0.05f);
        }
    }

    /// <summary>
    /// 受伤
    /// </summary>
    public override void Hurt()
    {
        base.Hurt();
        heroData ??= CharacterManager.Instance.GetHeroCharacterData();
        if (heroData == null)
        {
            return;
        }

        CharacterManager.Instance.ChangeCurHp(characterData.Id, heroData.Attack);
        if (characterData.CurHp <= 0)
        {
            Death();
        }

        var curRatio = characterData.CurHp / characterData.MaxHp;
        EventManager.Instance.Invoke(new MonsterHurtEvent { MonsterID = characterData.Id, MonsterHpRatio = curRatio });
    }

    /// <summary>
    /// 角色死亡
    /// </summary>
    internal override void Death()
    {
        
        base.Death();
        CharacterManager.Instance.SetCharacterDataDeath(characterData.Id);
        CharacterManager.Instance.AddHeroCharacterDataCoin(characterData.DropCoin);
        EventManager.Instance.Invoke<MonsterDeathAddSteakEvent>();
        EventManager.Instance.Invoke<MonsterDeathAddCoinEvent>();
        EventManager.Instance.Invoke(new MonsterDeathEvent { MonsterID = characterData.Id });

        var transSteak = ObjectPool.Instance
            .SpawnFromPoolToLocalPosition("SteakWithTween", steakPosTrans, Vector3.zero, Quaternion.identity).transform;
        if (transSteak != null)
        {
            var tweenSteak = transSteak.GetComponent<TweenPlayer>();
            var tweenTrans = tweenSteak.GetAnimation<TweenTransform>();
            tweenTrans.@from = steakPosTrans;
            var steakTargetTrans = HeroHelper.GetHeroLastSteakTrans();
            tweenTrans.@to = steakTargetTrans == null ? HeroHelper.CurHeroSteakTop : steakTargetTrans;
            tweenSteak.onForwardArrived += () =>
            {
                EventManager.Instance.Invoke<MonsterDeathAddSteakSpawnEvent>();
                ObjectPool.Instance.ReturnToPool("Steak", transSteak.gameObject);
            };
            tweenSteak.Play();
        }
        
        if (characterState == CharacterState.Death) return;
        characterState = CharacterState.Death;
        animeController.SwitchAnime("Death", 0.1f);
    }

    /// <summary>
    /// 死亡动画播放结束时处理
    /// </summary>
    public override void DeathEnd()
    {
        base.DeathEnd();
        ObjectPool.Instance.ReturnToPool("Boar", gameObject);
        CharacterManager.Instance.RemoveCharacterTransform(characterData.Id);
        CharacterManager.Instance.RemoveCharacterData(characterData.Id);
        characterData = null;
    }

    private void OnHeroDeath(HeroDeathEvent e)
    {
        isHeroDead = true;
        Idle();
    }
}