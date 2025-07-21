using UnityEngine;

public class MonsterSpawnController : MonoBehaviour
{
    private Transform heroTransform;
    public float spawnRadius = 4f; // 生成半径（以玩家为中心）
    public float minDistance = 2.5f; // 最小距离（避免生成在玩家脸上）
    public float spawnInterval = 0.5f; // 生成间隔（秒）
    public int maxEnemies = 60; // 最大敌人数量

    private float realSpawnInterval = 0;

    /// <summary>
    /// 生成间隔
    /// </summary>
    private float respawnTime = 5.0f;

    /// <summary>
    /// 初始怪物数量
    /// </summary>
    private int startMonsters = 10;

    /// <summary>
    /// 当前怪物数量
    /// </summary>
    private int currentMonsters = 0;

    /// <summary>
    /// 生成计时器
    /// </summary>
    private float timer;

    void Start()
    {
        EventManager.Instance.AddListener<MonsterDeathEvent>(OnMonsterDeath);

        for (int i = 0; i < startMonsters; i++)
        {
            SpawnMonster();
        }
    }

    void Update()
    {
        if (CharacterManager.Instance.CheckIsHeroDead())
        {
            return;
        }

        timer += Time.deltaTime;
        if (timer >= spawnInterval && CharacterManager.Instance.GetCurMonsterCount() < maxEnemies)
        {
            SpawnMonster();
            timer = 0f;
        }
    }

    /// <summary>
    /// 生成敌人
    /// </summary>
    void SpawnMonster()
    {
        if (!heroTransform)
        {
            heroTransform = HeroHelper.CurHeroFoot;
        }

        // 计算随机位置（在环形区域内）
        Vector2 randomCircle = Random.insideUnitCircle.normalized;
        randomCircle *= Random.Range(minDistance, spawnRadius);
        Vector3 spawnPosition = heroTransform.position + new Vector3(randomCircle.x, 0, randomCircle.y);

        // 实例化敌人
        float randomYRotation = Random.Range(0f, 360f);
        Quaternion randomRotation = Quaternion.Euler(0f, randomYRotation, 0f);
        var monster = ObjectPool.Instance.SpawnFromPool("Boar", transform, spawnPosition, randomRotation);
        
        if (!monster) return;
        //monster.transform.LookAt(HeroHelper.CurHeroFoot);
        if (!monster.GetComponent<MonsterController>())
        {
            monster.AddComponent<MonsterController>();
        }
        else
        {
            monster.GetComponent<MonsterController>().ReInitCharacter();
        }
    }

    /// <summary>
    /// 怪物死亡时场上生成数量-1
    /// </summary>
    /// <param name="e"></param>
    private void OnMonsterDeath(MonsterDeathEvent e)
    {
        if (currentMonsters > 0)
        {
            currentMonsters--;
        }
    }
}