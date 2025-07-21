using UnityEngine;
using UnityEngine.UI;

public class CharacterHpBarData
{
    public int Id;
    public Transform HpBarTransform;
    public Image HpBarImage;

    public static CharacterHpBarData CreateNewCharacterHpBarData(int id, Transform hpBarTransform, Image hpBarImage)
    {
        return new CharacterHpBarData()
        {
            Id = id,
            HpBarTransform = hpBarTransform,
            HpBarImage = hpBarImage,
        };
    }
}
