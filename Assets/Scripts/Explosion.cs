using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    private Animator animator;
    private AnimatorStateInfo info;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (animator != null)
        {
            info = animator.GetCurrentAnimatorStateInfo(0);
            if (info.normalizedTime >= 1)
            {
                // Destroy(gameObject);
                GameObjectPool.Instance.PushObject(gameObject);
            }
        }

        if (GetComponent<ParticleSystem>() != null)
        {
            if (!GetComponent<ParticleSystem>().IsAlive())
            {
                // Destroy(gameObject);
                GameObjectPool.Instance.PushObject(gameObject);
            }
        }
    }
}
