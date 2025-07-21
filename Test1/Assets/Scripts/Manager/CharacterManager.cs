using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterManager : Singleton<CharacterManager>
{
    private Dictionary<int, CharacterData> charactersDic = new();

    private Dictionary<int, Transform> charactersTransDic = new();

    /// <summary>
    /// 怪物Transform Dictionary
    /// </summary>
    public Dictionary<int, Transform> CharactersTransDic => charactersTransDic;

    private CharacterData heroCharacterData;

    public void SetHeroCharacterData(CharacterData characterData)
    {
        heroCharacterData = characterData;
    }

    public CharacterData GetHeroCharacterData()
    {
        return heroCharacterData;
    }

    /// <summary>
    /// 添加新角色数据
    /// </summary>
    /// <param name="maxHp"></param>
    /// <param name="attack"></param>
    /// <param name="speed"></param>
    /// <param name="dropCoin"></param>
    /// <returns></returns>
    public CharacterData AddNewCharacterData(float maxHp, float attack, float speed, int dropCoin)
    {
        var id = GetNewMonsterId();
        if (id == -1)
        {
            return null;
        }

        var characterData = CharacterData.CreateNewCharacterData(id, maxHp, attack, speed, maxHp, dropCoin);
        if (charactersDic.TryAdd(characterData.Id, characterData)) return characterData;
        Debug.LogWarning("Monster already added");
        return null;
    }

    public void SetCharacterDataDeath(int id)
    {
        if (charactersDic.TryGetValue(id, out CharacterData characterData))
        {
            characterData.IsDead = true;
        }
    }

    public bool CheckIsCharacterDead(int id)
    {
        if (charactersDic.TryGetValue(id, out CharacterData characterData))
        {
            return characterData.IsDead;
        }

        return true;
    }

    public void AddHeroCharacterDataCoin(int num)
    {
        if (heroCharacterData != null)
        {
            heroCharacterData.Coin += num;
        }
    }

    public void AddHeroCharacterDataPorkSteak()
    {
        if (heroCharacterData != null)
        {
            heroCharacterData.PorkSteak++;
        }
    }

    public int GetHeroCoin()
    {
        return heroCharacterData?.Coin ?? 0;
    }
    
    public int GetHeroPorkSteak()
    {
        return heroCharacterData?.PorkSteak ?? 0;
    }

    public void SetHeroCharacterDataDeath()
    {
        if (heroCharacterData != null)
        {
            heroCharacterData.IsDead = true;
        }
    }

    public bool CheckIsHeroDead()
    {
        return heroCharacterData == null || heroCharacterData.IsDead;
    }

    public void RemoveCharacterData(int id)
    {
        if (charactersDic.ContainsKey(id))
        {
            charactersDic.Remove(id);
        }
    }

    public void AddNewCharacterTransform(int id, Transform trans)
    {
        if (!charactersTransDic.TryAdd(id, trans))
        {
            Debug.LogWarning("Monster already added");
        }
    }

    public void RemoveCharacterTransform(int id)
    {
        if (charactersTransDic.ContainsKey(id))
        {
            charactersTransDic.Remove(id);
        }
    }

    public CharacterData GetCharacterDataById(int id)
    {
        return charactersDic.GetValueOrDefault(id);
    }

    public Transform GetCharacterTransformById(int id)
    {
        return charactersTransDic.GetValueOrDefault(id);
    }

    public void ChangeHeroCurHp(float damage)
    {
        var curHp = heroCharacterData.CurHp - damage;
        if (curHp <= 0)
        {
            heroCharacterData.CurHp = 0;
            heroCharacterData.IsDead = true;
        }
        else
        {
            heroCharacterData.CurHp = curHp;
        }
    }

    public void ChangeCurHp(int id, float damage)
    {
        if (!charactersDic.TryGetValue(id, out var characterData)) return;
        var curHp = characterData.CurHp - damage;
        if (curHp <= 0)
        {
            characterData.CurHp = 0;
            characterData.IsDead = true;
        }
        else
        {
            characterData.CurHp = curHp;
        }
    }

    public int GetCurMonsterCount()
    {
        return charactersDic.Count;
    }

    /// <summary>
    /// 随机怪物ID
    /// </summary>
    /// <returns></returns>
    private int GetNewMonsterId()
    {
        var existingKeys = charactersDic.Keys
            .Where(key => key >= 1 && key <= 100)
            .ToHashSet();

        // 创建1-100的候选数字列表，并排除已存在的Key
        var availableNumbers = Enumerable.Range(1, 100)
            .Where(num => !existingKeys.Contains(num))
            .ToList();

        // 如果没有可用数字则返回-1（表示失败）
        if (availableNumbers.Count == 0)
            return -1;

        // 随机选择一个可用数字
        var random = new System.Random();
        int index = random.Next(availableNumbers.Count);
        return availableNumbers[index];
    }
}