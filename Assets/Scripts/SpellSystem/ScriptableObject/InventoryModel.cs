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
    public int wandCount=>maxWandCount;
    public int spellCount=>maxSpellCount;
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
    public bool Add<T>(T obj)
    {
        if (obj is Wand wand)
        {
            return AddWand(wand, -1);
        }
        else if (obj is Spell spell)
        {
            return AddSpell(spell, -1);
        }
        return false;
    }
    public bool Add<T>(T obj, int index)
    {
        if (obj is Wand wand)
        {
            return AddWand(wand, index);
        }
        else if (obj is Spell spell)
        {
            return AddSpell(spell, index);
        }
        return false;
    }
    public void Remove<T>(T obj, int index)
    {
        if (obj is Wand wand)
        {
            RemoveWand(wand, index);
        }
        else if (obj is Spell spell)
        {
            RemoveSpell(spell, index);
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
        spells[index] = spell;
        return true;
    }
    public void RemoveSpell(Spell spell, int index)
    {
        nullSpellIndices.Enqueue(index);
        spells[index] = null;
    }

    public void RemoveWand(Wand wand, int index)
    {
        nullWandIndices.Enqueue(index);
        wands[index] = null;
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
