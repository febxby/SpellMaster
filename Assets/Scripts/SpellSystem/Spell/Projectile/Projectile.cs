using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
public interface ICast
{
    void Cast(Vector2 start, Vector2 end, Vector2 direction, Spell spell, string uniqueId);
}
public class MCast : ICast
{
    public MCast() { }
    public virtual void Cast(Vector2 start, Vector2 end, Vector2 direction, Spell spell)
    {
        throw new System.NotImplementedException();
    }

    public void Cast(Vector2 start, Vector2 end, Vector2 direction, Spell spell, string uniqueId)
    {
        throw new System.NotImplementedException();
    }
}
[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour, ICast
{
    public static UnityAction<Vector3, int> OnCollider;
    public string uniqueId;
    public Spell spell;
    Vector2 direction;
    RaycastHit2D hit;
    Rigidbody2D rb;
    protected TrailRenderer trail;
    protected int bounce;
    Vector3 lastPos;
    protected WaitForSeconds seconds;
    public Projectile Initialized(Spell spell, string uniqueId)
    {
        // this.spell = Instantiate(spell);
        this.spell = spell;
        this.uniqueId = uniqueId;
        bounce = spell.bounce;
        seconds = new WaitForSeconds(spell.lifeTime);
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = this.spell.gravity;
        if (spell.casts.Count > 0)
            foreach (var cast in spell.casts)
            {
                if (gameObject.transform.TryGetComponent(cast.GetType(), out Component component))
                {
                    (component as Behaviour).enabled = true;
                    (component as ProjectileComponent).Init(this.spell, uniqueId);
                    continue;
                }
                component = gameObject.AddComponent(cast.GetType());
                JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(cast), component);
                (component as ProjectileComponent).Init(this.spell, uniqueId);
            }

        StartCoroutine(Disable());
        return this;
    }

    public void Cast(Vector2 start, Vector2 end, Vector2 direction, Spell spell, string uniqueId)
    {
        GameObject spellObj = GameObjectPool.Instance.GetObject(spell.prefab);
        spellObj.transform.SetPositionAndRotation(start, Quaternion.identity);
        // GameObject spellObj = Instantiate(spell.prefab, start, Quaternion.identity);
        Projectile projectile = spellObj.GetComponent<Projectile>().Initialized(spell, uniqueId);
        float angle = Random.Range(-spell.spread, spell.spread);
        Quaternion quaternion = Quaternion.AngleAxis(angle, Vector3.forward);
        projectile.direction = (quaternion * direction).normalized;
        spellObj.transform.right = projectile.direction;
        if (spellObj.TryGetComponent<TrailRenderer>(out trail))
        {
            trail.Clear();
            trail.enabled = true;
        }
        projectile.rb.AddForce(quaternion * direction * spell.speed, ForceMode2D.Impulse);

    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag(spell.owner) && !spell.canHurtSelf)
        {
            return;
        }
        if (other.gameObject.CompareTag("Obstacle"))
        {
            var lastPos = (Vector2)transform.position - rb.velocity * Time.deltaTime;
            hit = Physics2D.Raycast(lastPos, direction, 10, LayerMask.GetMask("Obstacle"));
            if (bounce > 0)
            {
                bounce--;
                transform.right = Vector2.Reflect(direction, hit.normal);
                rb.velocity = transform.right * rb.velocity.magnitude;
                direction = rb.velocity.normalized;
                return;
            }
            DestroyObject();
            return;
        }
        other.gameObject.TryGetComponent<IDamageable>(out IDamageable damageable);
        if (damageable != null)
        {
            if (spell.attaches.Count > 0)
                foreach (var cast in spell.attaches)
                {
                    var obj = GameObjectPool.Instance.GetObject(cast);
                    obj.GetComponent<AttachComponent>().Init(spell);
                    obj.transform.SetParent(other.transform);
                    obj.transform.localPosition = Vector3.zero;
                }
            damageable.TakeDamage(spell.damage);
            DestroyObject();
            return;
        }
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag(spell.owner) && !spell.canHurtSelf)
        {
            return;
        }
        if (other.gameObject.CompareTag("Obstacle"))
        {
            // var lastPos = (Vector2)transform.position - rb.velocity * Time.deltaTime;
            // hit = Physics2D.Raycast(lastPos, direction, 10, LayerMask.GetMask("Obstacle"));

            if (bounce > 0)
            {
                bounce--;
                Vector2 reflectDirection = Vector2.Reflect(direction, other.GetContact(0).normal);
                Debug.DrawLine(other.GetContact(0).point, other.GetContact(0).point + (Vector2)other.GetContact(0).normal, Color.red, 10f);
                // 添加一个小的随机扰动
                // reflectDirection += new Vector2(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f));
                reflectDirection.Normalize();
                // direction = reflectDirection;
                transform.right = reflectDirection;
                rb.velocity = reflectDirection * spell.speed;
                direction = reflectDirection;
                return;
            }
            DestroyObject();
            return;
        }
        other.gameObject.TryGetComponent<IDamageable>(out IDamageable damageable);
        if (damageable != null)
        {
            if (spell.attaches.Count > 0)
                foreach (var cast in spell.attaches)
                {
                    var obj = GameObjectPool.Instance.GetObject(cast);
                    obj.GetComponent<AttachComponent>().Init(spell);
                    obj.transform.SetParent(other.transform);
                    obj.transform.localPosition = Vector3.zero;
                }
            damageable.TakeDamage(spell.damage);
            DestroyObject();
            return;
        }
    }
    private void DestroyObject(bool isNatural = false)
    {
        if (spell.isTrigger)
        {
            for (int i = 0; i < spell.spells.Count; i++)
            {
                Vector2 newPosition = (Vector2)transform.position + Vector2.Reflect(direction, hit.normal) * 0.1f;
                string uniqueId = System.Guid.NewGuid().ToString();
                if (!isNatural)
                    spell.spells[i].Cast(newPosition, transform.position, Vector2.Reflect(direction, hit.normal), spell.owner, uniqueId);
                else
                    spell.spells[i].Cast(newPosition, transform.position, rb.velocity.normalized, spell.owner, uniqueId);
            }
        }
        if (spell.casts.Count > 0)
        {
            foreach (var cast in spell.casts)
            {
                if (gameObject.TryGetComponent(cast.GetType(), out Component component))
                {
                    (component as Behaviour).enabled = false;
                }
            }
            // spell.casts.Clear();
        }

        if (trail != null)
        {
            trail.Clear();
            trail.enabled = false;
        }
        gameObject.transform.position = Vector3.zero;
        GameObjectPool.Instance.PushObject(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(hit.point, hit.normal);
    }
    // private void OnGUI()
    // {
    //     GUIStyle style = new()
    //     {
    //         fontSize = 80
    //     };
    //     GUI.Label(new Rect(500, 500, 500, 500), rb.velocity.magnitude.ToString(), style);
    // }
    private void OnDestroy()
    {
        StopAllCoroutines();
    }
    IEnumerator Disable()
    {
        if (spell.lifeTime > -1f || spell.lifeTime < -1f)
        {
            yield return seconds;
            DestroyObject(true);
        }


    }

    void OnDisable()
    {
        StopAllCoroutines();
    }
    // private void OnTriggerEnter(Collider other)
    // {
    //     Debug.Log(other.name);
    //     if (other.gameObject.tag == "Obstacle")
    //     {
    //         if (isTrigger)
    //         {
    //             hit = Physics2D.Raycast(transform.position, direction, 100, LayerMask.GetMask("Obstacle"));
    //             for (int i = 0; i < spell.spells.Count; i++)
    //             {
    //                 spell.spells[i].Cast(transform.position, other.transform.position, Vector2.Reflect(direction, hit.normal));
    //             }
    //             Destroy(gameObject);
    //         }
    //         // Destroy(gameObject);
    //     }
    //     if (other.GetComponent<MeshCollider>())
    //     {
    //         OnCollider?.Invoke(transform.position, (int)spell.burstRadius);
    //         Destroy(gameObject);
    //     }
    // }
}
