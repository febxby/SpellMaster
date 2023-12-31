using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
public class ProjectileComponent : MonoBehaviour
{
    public Spell spell;
    public string uniqueId;
    public virtual void Init(Spell spell)
    {
        this.spell = spell;
    }
    public virtual void Init(Spell spell, string uniqueId)
    {
        this.spell = spell;
        this.uniqueId = uniqueId;
    }
}
public class Tracking : ProjectileComponent, ICast
{
    // public enum TrackingType
    // {
    //     Target,
    //     Direction
    // }
    RaycastHit2D[] hits = new RaycastHit2D[1];
    Rigidbody2D rb2D;
    Rigidbody rb;

    // public TrackingType trackingType;
    // public float trackingSpeed;
    public float trackingRange;
    public float rotateSpeed;
    public void Cast(Vector2 start, Vector2 end, Vector2 direction, Spell spell, string uniqueId)
    {
        for (int i = 0; i < spell.spells.Count; i++)
        {
            spell.spells[i].casts = spell.casts;
            //检测spell.spells[i].casts数组是否含有Tracking类型的值
            if (!spell.spells[i].casts.Exists(x => x.GetType() == GetType()))
            {
                spell.spells[i].casts.Add(this);
            }
            spell.spells[i].attaches = spell.attaches;
            // spell.spells[i].castDict.Add(typeof(Tracking), this);
            spell.spells[i].Cast(start, end, direction, spell.owner, uniqueId);
        }
    }
    private void Awake()
    {
        if (rb2D == null || rb == null)
        {
            TryGetComponent<Rigidbody>(out rb);
            TryGetComponent<Rigidbody2D>(out rb2D);
        }

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (hits[0].collider == null)
            Physics2D.CircleCastNonAlloc(transform.position, trackingRange, Vector2.zero, hits, 0,
            spell.owner != "Enemy" ? LayerMask.GetMask("Enemy") : LayerMask.GetMask("Player"));
        else
        {
            Vector3 direction = (hits[0].collider.transform.position - transform.position).normalized;
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            quaternion quaternion = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.right = Vector3.Slerp(transform.right, direction, rotateSpeed / Vector2.Distance(transform.position, hits[0].collider.transform.position));
            // transform.rotation = Quaternion.RotateTowards(transform.rotation, quaternion, rotateSpeed * Time.deltaTime);
            //改变子弹的方向但不改变速度;
            //让方向转变更平滑
            if (rb2D != null)
                rb2D.velocity = transform.right * rb2D.velocity.magnitude;
            else if (rb != null)
                rb.velocity = transform.right * rb.velocity.magnitude;

        }
    }
    private void OnDrawGizmos()
    {
        // Gizmos.DrawWireSphere(transform.position, trackingRange);
    }
    private void OnDisable()
    {
        Array.Clear(hits, 0, hits.Length);
    }
}
