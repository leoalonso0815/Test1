using UnityEngine;

public class BaseController : MonoBehaviour
{
    public CharacterData characterData;

    internal enum CharacterState
    {
        Idle,
        Run,
        Attack,
        Death
    }

    internal CharacterState characterState = CharacterState.Idle;

    internal enum CharacterAttackState
    {
        Stop,
        Start,
        Null
    }

    internal CharacterAttackState characterAttackState = CharacterAttackState.Stop;

    //internal LevelManager LevelManager = null;
    internal AnimeController animeController = null;

    internal virtual void Init()
    {
        //LevelManager = GameObject.Find("GameMgr").GetComponent<LevelManager>();
    }

    internal virtual void InitCharacter()
    {
    }

    public virtual void Hurt(float damage)
    {
        
    }
    
    public virtual void Hurt()
    {
        
    }
    
    internal virtual void Death()
    {
        
    }
    
    public virtual void DeathEnd()
    {
        
    }
}