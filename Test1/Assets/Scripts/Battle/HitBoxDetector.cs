using System;
using UnityEngine;

public class HitBoxDetector : MonoBehaviour
{
    private void Start()
    {
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Hero"), LayerMask.NameToLayer("HeroDetect"));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("EnemyDetect"))
        {
            Debug.Log("攻击成功: " + other.gameObject.name);
        }
    }
}