using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ElectricArc : ProjectileComponent, ICast
{
    public GameObject prefab;
    public GameObject child;
    public LineRenderer lineRenderer;
    public Collider2D[] colliders = new Collider2D[10];
    public RaycastHit2D[] hits = new RaycastHit2D[10];
    public LayerMask layerMask;
    Dictionary<(int, int), (ElectricArc, ElectricArc)> dict;
    int length;
    public void Cast(Vector2 start, Vector2 end, Vector2 direction, Spell spell, string uniqueId)
    {
        // string uniqueId = System.Guid.NewGuid().ToString();
        this.spell = spell;
        this.uniqueId = uniqueId;
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
    override public void Init(Spell spell, string uniqueId)
    {
        base.Init(this.spell, uniqueId);
        dict = new Dictionary<(int, int), (ElectricArc, ElectricArc)>();
        if (child == null)
            child = Instantiate(prefab, transform.position, Quaternion.identity, transform);
        else
            child.SetActive(true);
        if (child.TryGetComponent(out lineRenderer))
            lineRenderer.enabled = true;
        else
            lineRenderer = child.AddComponent<LineRenderer>();
        ConnectWithExistingArcs();
    }
    private void Awake()
    {
        // lineRenderer = GetComponent<LineRenderer>();
    }
    private void OnEnable()
    {
        // Debug.Log(prefab);
        // if (child == null)
        //     child = Instantiate(prefab, transform.position, Quaternion.identity, transform);
        // if (child.TryGetComponent(out lineRenderer))
        //     lineRenderer.enabled = true;
        // else
        //     lineRenderer = child.AddComponent<LineRenderer>();
        // ConnectWithExistingArcs();
    }

    private void ConnectWithExistingArcs()
    {
        int length = Physics2D.OverlapCircleNonAlloc(transform.position, 10f, colliders, layerMask);
        if (length == 0)
            return;
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i] == null)
                continue;
            if (colliders[i].gameObject == gameObject)
                continue;
            if (!colliders[i].TryGetComponent<ElectricArc>(out var arc))
            {
                colliders[i] = null;
                continue;
            }
            if (arc.uniqueId != uniqueId)
                continue;
            ConnectWithLineRenderer(arc, this);
        }
    }
    private void ConnectWithLineRenderer(ElectricArc source, ElectricArc target)
    {
        // 设置 LineRenderer 的起始点和结束点
        lineRenderer.positionCount += 2;
        dict.Add((lineRenderer.positionCount - 2, lineRenderer.positionCount - 1), (source, target));
        lineRenderer.SetPosition(lineRenderer.positionCount - 2, source.transform.position);
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, target.transform.position);
    }
    private void OnDisable()
    {
        lineRenderer.positionCount = 0;
        child.SetActive(false);
        Array.Clear(colliders, 0, colliders.Length);
        Array.Clear(hits, 0, hits.Length);
        dict.Clear();
    }
    void Update()
    {
        if (dict.Count > 0)
        {
            foreach (var item in dict)
            {
                if (!item.Value.Item1.gameObject.activeSelf)
                {
                    lineRenderer.SetPosition(item.Key.Item1, item.Value.Item1.transform.position);
                    lineRenderer.SetPosition(item.Key.Item2, item.Value.Item1.transform.position);
                    // dict.Remove(item.Key);
                    continue;
                }
                Vector2 distance = item.Value.Item2.transform.position - item.Value.Item1.transform.position;
                lineRenderer.material.SetFloat("_Tiling",
                Vector3.Distance(item.Value.Item2.transform.position, item.Value.Item1.transform.position) / 5);
                length = Physics2D.RaycastNonAlloc(
                    item.Value.Item1.transform.position,
                    distance.normalized,
                    hits, distance.magnitude);
                lineRenderer.SetPosition(item.Key.Item1, item.Value.Item1.transform.position);
                lineRenderer.SetPosition(item.Key.Item2, item.Value.Item2.transform.position);
            }
        }
        if (length > 0)
        {
            for (int i = 0; i < length; i++)
            {

                if (hits[i] == default(RaycastHit2D))
                    continue;
                if (hits[i].collider.gameObject.CompareTag(spell.owner))
                {
                    hits[i] = default;
                    continue;
                }
                hits[i].collider.gameObject.TryGetComponent<IDamageable>(out IDamageable damageable);
                damageable?.TakeDamage(spell.damage);
                hits[i] = default;
            }
        }
    }
}
