using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class AttachComponent : MonoBehaviour
{
    public virtual void Init(Spell spell)
    {

    }
}
public class GravityField : AttachComponent, ICast
{
    public int force;
    [SerializeField] Spell spell;
    public void Cast(Vector2 start, Vector2 end, Vector2 direction, Spell spell)
    {
        for (int i = 0; i < spell.spells.Count; i++)
        {
            spell.spells[i].attaches = spell.attaches;
            //检测spell.spells[i].casts数组是否含有Tracking类型的值
            // if (!spell.spells[i].attaches.Exists(x => x.GetType() == GetType()))
            // {
            spell.spells[i].attaches.Add(spell.prefab);
            // }
            spell.spells[i].casts = spell.casts;
            // spell.spells[i].castDict.Add(typeof(Tracking), this);
            spell.spells[i].Cast(start, end, direction, spell.owner);
        }
    }
    public override void Init(Spell spell)
    {
        // this.spell = spell;
    }
    private void OnEnable()
    {
        StartCoroutine(UpdateLifeTime());
    }
    IEnumerator UpdateLifeTime()
    {
        yield return new WaitForSeconds(spell.lifeTime);
        Destroy(gameObject);
    }
    private void Update()
    {
    }
    private void OnDisable()
    {
        StopAllCoroutines();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent<Rigidbody2D>(out var rb))
        {
            Vector2 dir = (transform.position - other.transform.position).normalized;
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            Quaternion quaternion = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.right = Vector3.Slerp(transform.right, dir, 1);
            rb.velocity = transform.right * rb.velocity.magnitude;
        }
    }
}
