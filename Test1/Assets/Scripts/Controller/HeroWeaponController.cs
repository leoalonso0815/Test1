using Drakkar.GameUtils;
using UnityEngine;

public class HeroWeaponController : MonoBehaviour
{
    public DrakkarTrail trail;

    public void StartTrail()
    {
        trail.Begin();
    }

    public void StopTrail()
    {
        trail.End();
    }
}
