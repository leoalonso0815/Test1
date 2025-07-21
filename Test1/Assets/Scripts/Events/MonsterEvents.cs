/// <summary>
/// 角色Init完成事件
/// </summary>
public class MonsterInitEvent : IEvent
{
    public int MonsterID;
}

public class MonsterHurtEvent : IEvent
{
    public int MonsterID;
    public float MonsterHpRatio;
}

public class MonsterDeathEvent : IEvent
{
    public int MonsterID;
}

public class MonsterDeathAddCoinEvent : IEvent
{

}

public class MonsterDeathAddSteakEvent : IEvent
{
    
}

public class MonsterDeathAddSteakSpawnEvent : IEvent
{
    
}
