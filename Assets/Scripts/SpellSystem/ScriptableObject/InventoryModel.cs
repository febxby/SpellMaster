using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
[CreateAssetMenu(fileName = "New Bag", menuName = "Inventory/bag")]
public class InventoryModel : ScriptableObject
{
    [SerializeField] private int maxWandCount = 4;
    [SerializeField] private int maxSpellCount = 24;
    public int wandCount => maxWandCount;
    public int spellCount => maxSpellCount;
    public List<Wand> wands;
    public List<Spell> spells;
    PriorityQueue<int> nullSpellIndices;
    PriorityQueue<int> nullWandIndices;
    // private void OnEnable()
    // {

    // }
    public void Init()
    {
        wands = new List<Wand>(maxWandCount);
        spells = new List<Spell>(maxSpellCount);
        nullSpellIndices = new PriorityQueue<int>();
        nullWandIndices = new PriorityQueue<int>();
        for (int i = 0; i < maxWandCount; i++)
        {
            wands.Add(null);
            nullWandIndices.Enqueue(i);
        }
        for (int i = 0; i < maxSpellCount; i++)
        {
            spells.Add(null);
            nullSpellIndices.Enqueue(i);
        }
    }
    /// <summary>
    /// 往最近的一个空位添加
    /// </summary>
    /// <param name="obj"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns>是否有空位</returns>
    public bool Add<T>(T obj)
    {
        var type = typeof(T);
        if (type == typeof(Wand))
        {
            return AddWand(obj as Wand, -1);
        }
        else if (type == typeof(Spell))
        {
            return AddSpell(obj as Spell, -1);
        }
        return false;
    }
    /// <summary>
    /// 往索引处添加
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="index">索引为-1往最近的空位添加</param>
    /// <typeparam name="T"></typeparam>
    /// <returns>是否有空位</returns>
    public bool Add<T>(T obj, int index)
    {
        var type = typeof(T);
        if (type == typeof(Wand))
        {
            return AddWand(obj as Wand, index);
        }
        else if (type == typeof(Spell))
        {
            return AddSpell(obj as Spell, index);
        }
        return false;
    }
    public void Remove<T>(int index)
    {
        var type = typeof(T);
        if (type == typeof(Wand))
        {
            RemoveWand(index);
        }
        else if (type == typeof(Spell))
        {
            RemoveSpell(index);
        }
    }

    public bool AddWand(Wand wand, int index)
    {
        if (index >= maxWandCount || index < -1)
        {
            return false;
        }
        if (index == -1)
        {
            bool result = nullWandIndices.TryDequeue(out index);
            if (!result)
            {
                return false;
            }
        }
        if (wand == null)
            nullSpellIndices.Enqueue(index);
        wands[index] = wand;
        return true;
    }

    public bool AddSpell(Spell spell, int index)
    {
        if (index >= maxSpellCount || index < -1)
        {
            return false;
        }
        if (index == -1)
        {
            bool result = nullSpellIndices.TryDequeue(out index);
            if (!result)
            {
                return false;
            }
        }
        if (spell == null)
            nullSpellIndices.Enqueue(index);
        spells[index] = spell;
        return true;
    }
    public void RemoveSpell(int index)
    {
        nullSpellIndices.Enqueue(index);
        spells[index] = null;
    }

    public void RemoveWand(int index)
    {
        nullWandIndices.Enqueue(index);
        wands[index] = null;
    }
    //Get方法，根据索引返回Wand或Spell
    public Spell GetSpell(int index)
    {
        if (index >= maxSpellCount || index < 0)
            return null;
        return spells[index];
    }
    public Wand GetWand(int index)
    {
        if (index >= maxWandCount || index < 0)
            return null;
        return wands[index];
    }

    public void Clear()
    {
        wands.Clear();
        spells.Clear();
        nullSpellIndices.Clear();
        nullWandIndices.Clear();
    }

    private void OnDisable()
    {
        wands.Clear();
        spells.Clear();
    }
    private void OnDestroy()
    {
        //TODO:测试
        wands.Clear();
        spells.Clear();

    }
}
