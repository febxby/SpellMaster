using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SineWave : ProjectileComponent, ICast
{
    // 法术的初始速度
    public float speed = 10f;
    // 法术的振幅
    public float amplitude = 1f;
    // 法术的频率
    public float frequency = 1f;
    // 法术的刚体组件
    // 法术的初始方向
    private Vector3 direction;
    float lifetime;

    Rigidbody2D rb;
    Vector3 pos;
    float initialSpeed;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        pos = transform.position;
    }
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
    public override void Init(Spell spell, string uniqueId)
    {
        base.Init(spell, uniqueId);
        lifetime = 0;
    }
    private void OnEnable()
    {
        pos = transform.position;
    }
    private void FixedUpdate()
    {
        // direction = transform.right;
        // pos += speed * Time.fixedDeltaTime * direction;
        // transform.position = (pos + amplitude * Mathf.Sin(Time.time * frequency) * transform.up);
        lifetime += Time.fixedDeltaTime;
        // 计算法术的速度的水平分量，即初始方向乘以初始速度
        Vector3 horizontal = transform.right;
        // 计算法术的速度的垂直分量，即与初始方向垂直的方向乘以正弦函数
        Vector3 vertical = amplitude * Mathf.Sin(lifetime * frequency) * transform.up;
        // 更新法术的刚体的速度，加上水平分量和垂直分量
        rb.velocity = (horizontal + vertical) * spell.speed;
    }
}
