/// <summary>
/// 角色Init完成事件
/// </summary>
public class HeroInitEvent : IEvent
{
    
}

public class HeroHurtEvent : IEvent
{
    public float curHp;
    public float maxHp;
    public float HeroHpRatio;
}

public class HeroDeathEvent : IEvent
{

}

public class HeroDeathEndEvent : IEvent
{

}
