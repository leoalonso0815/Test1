public class CharacterData
{
    public int Id;
    public float MaxHp;
    public float Attack;
    public float Speed;
    public float CurHp;
    public int DropCoin;
    public bool IsDead;
    public int Coin;
    public int PorkSteak;

    public static CharacterData CreateNewCharacterData(int id, float maxHp, float attack, float speed, float curHp,
        int dropCoin)
    {
        return new CharacterData()
        {
            Id = id,
            MaxHp = maxHp,
            CurHp = curHp,
            Attack = attack,
            Speed = speed,
            DropCoin = dropCoin,
            IsDead = false,
        };
    }
    
    public static CharacterData CreateNewCharacterData(int id, float maxHp, float attack, float speed, float curHp)
    {
        return new CharacterData()
        {
            Id = id,
            MaxHp = maxHp,
            CurHp = curHp,
            Attack = attack,
            Speed = speed,
            DropCoin = 0,
            Coin = 0,
            PorkSteak = 0,
            IsDead = false,
        };
    }
}