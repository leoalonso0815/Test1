using UnityEngine;

public class MonsterWeaponController : MonoBehaviour
{
    private CharacterData monsterData;

    void Start()
    {
        monsterData = GetComponentInParent<MonsterController>().characterData;
    }

    public float GetMonsterAttack()
    {
        return monsterData?.Attack ?? 0f;
    }

    public int GetMonsterId()
    {
        return monsterData?.Id ?? 0;
    }
}