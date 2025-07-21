using System.Collections.Generic;
using UnityEngine;

public static class HeroHelper
{
    /// <summary>
    /// 当前控制角色
    /// </summary>
    private static Transform curHero;

    public static Transform CurHero => curHero;

    /// <summary>
    /// 当前控制角色
    /// </summary>
    private static Transform curHeroFoot;

    public static Transform CurHeroFoot => curHeroFoot;

    /// <summary>
    /// 当前控制角色
    /// </summary>
    private static Transform curHeroSteakTop;

    public static Transform CurHeroSteakTop => curHeroSteakTop;

    private static List<Transform> steakTransforms = new List<Transform>();
    public static List<Transform> SteakTransforms => steakTransforms;

    public static void SetHero(Transform hero)
    {
        curHero = hero;
    }

    public static void SetHeroFoot(Transform heroFoot)
    {
        curHeroFoot = heroFoot;
    }

    public static void SetHeroSteakPosTrans(Transform steakTop)
    {
        curHeroSteakTop = steakTop;
    }

    public static void AddHeroSteakTrans(Transform steakTrans)
    {
        steakTransforms.Add(steakTrans);
    }

    public static Transform GetHeroLastSteakTrans()
    {
        if (steakTransforms.Count > 0)
        {
            return steakTransforms[^1];
        }
        
        return null;
    }
}