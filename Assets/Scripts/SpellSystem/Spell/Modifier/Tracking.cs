using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
public class Tracking : MonoBehaviour, ICast
{
    // public enum TrackingType
    // {
    //     Target,
    //     Direction
    // }
    RaycastHit2D[] hits = new RaycastHit2D[1];
    Rigidbody2D rb;
    // public TrackingType trackingType;
    // public float trackingSpeed;
    public float trackingRange;
    public float rotateSpeed;
    public void Cast(Vector2 start, Vector2 end, Vector2 direction, Spell spell)
    {
        for (int i = 0; i < spell.spells.Count; i++)
        {
            spell.spells[i].casts.Add(this);
            // spell.spells[i].castDict.Add(typeof(Tracking), this);
            spell.spells[i].Cast(start, end, direction,spell.owner);
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (hits[0].collider == null)
            Physics2D.CircleCastNonAlloc(transform.position, trackingRange, Vector2.zero, hits, 0, LayerMask.GetMask("Enemy"));
        else
        {
            Vector3 direction = (hits[0].collider.transform.position - transform.position).normalized;
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            quaternion quaternion = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.right = Vector3.Slerp(transform.right, direction, rotateSpeed / Vector2.Distance(transform.position, hits[0].collider.transform.position));
            // transform.rotation = Quaternion.RotateTowards(transform.rotation, quaternion, rotateSpeed * Time.deltaTime);
            rb = transform.GetComponent<Rigidbody2D>();
            //改变子弹的方向但不改变速度;
            //让方向转变更平滑
            rb.velocity = transform.right * rb.velocity.magnitude;

        }
    }
    private void OnDrawGizmos()
    {
        // Gizmos.DrawWireSphere(transform.position, trackingRange);
    }
    private void OnDisable()
    {
        hits = new RaycastHit2D[1];
    }
}
