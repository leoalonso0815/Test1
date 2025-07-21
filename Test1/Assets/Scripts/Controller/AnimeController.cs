using System;
using Drakkar.GameUtils;
using UnityEngine;
using UnityEngine.VFX;

public class AnimeController : MonoBehaviour
{
    private Animator animator;
    private Collider heroHitBox;
    private MonsterController monsterController;
    private HeroController heroController;
    private bool isHero = false;
    public DrakkarTrail trail;

    public void Init(bool isHeroInit = false)
    {
        animator = GetComponent<Animator>();
        isHero = isHeroInit;
        if (isHero)
        {
            heroHitBox = transform
                .Find(
                    "mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:RightShoulder/mixamorig:RightArm/mixamorig:RightForeArm/mixamorig:RightHand/fuzi")
                .GetComponent<Collider>();
            heroController = transform.parent.GetComponent<HeroController>();
            trail = transform.GetComponentInChildren<DrakkarTrail>();
        }
        else
        {
            heroHitBox = transform
                .Find(
                    "root/BOAR_/BOAR_ Pelvis/BOAR_ Spine/BOAR_ Spine1/BOAR_ Neck/BOAR_ Neck1/BOAR_ Head/BOAR_ Queue de cheval 1")
                .GetComponent<Collider>();
            monsterController = transform.parent.GetComponent<MonsterController>();
        }

        if (!animator)
        {
            Debug.LogWarning("Animator not found");
            return;
        }
    }

    /// <summary>
    /// 切换动画
    /// </summary>
    /// <param name="animeName"></param>
    /// <param name="delay"></param>
    public void SwitchAnime(string animeName, float delay)
    {
        animator.CrossFadeInFixedTime(animeName, delay);
    }

    /// <summary>
    /// 切换动画(Trigger)
    /// </summary>
    /// <param name="animeTrigger"></param>
    public void SwitchAnime(string animeTrigger)
    {
        animator.SetTrigger(animeTrigger);
    }

    public void DeathAnimeEnd()
    {
        if (isHero)
        {
            EventManager.Instance.Invoke<HeroDeathEndEvent>();
        }
        else
        {
            monsterController.DeathEnd();
        }
    }

    public void EnableHitBox()
    {
        if (isHero)
        {
            trail.Begin();
        }

        heroHitBox.enabled = true;
    }

    public void DisableHitBox()
    {
        if (isHero)
        {
            trail.End();
        }
        
        heroHitBox.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isHero)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("EnemyWeapon"))
            {
                var damage = other.GetComponent<MonsterWeaponController>().GetMonsterAttack();
                heroController.Hurt(damage);
                //Debug.Log("被敌人命中: " + other.GetComponent<MonsterWeaponController>().GetMonsterId() + "伤害 === " +
                //damage);
            }
        }
        else
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("HeroWeapon"))
            {
                monsterController.Hurt();
                //Debug.Log("被命中: " + other.gameObject.name + "enemy === " +
                //monsterController.characterData.Id);
            }
        }
    }
}