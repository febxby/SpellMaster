using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Bag", menuName = "Inventory/bag")]
public class InventoryModel : ScriptableObject
{
    [SerializeField] private int maxWandCount = 4;
    [SerializeField] private int maxSpellCount = 24;
    public List<Wand> wands;
    public List<Spell> spells;
    private void OnEnable()
    {
        wands = new List<Wand>(maxWandCount);
        spells = new List<Spell>(maxSpellCount);
    }
    public bool Add<T>(T obj)
    {
        if (obj is Wand wand)
        {
            return AddWand(wand);
        }
        else if (obj is Spell spell)
        {
            return AddSpell(spell);
        }
        return false;
    }
    public bool AddWand(Wand wand)
    {
        if (wands.Count >= maxWandCount)
            return false;
        wands.Add(wand);
        return true;
    }

    public bool AddSpell(Spell spell)
    {
        if (wands.Count >= maxSpellCount)
            return false;
        spells.Add(spell);
        return true;
    }
    public void Remove<T>(T obj)
    {
    }
    public void RemoveSpell(Spell spell)
    {
        spells.Remove(spell);
    }

    public void RemoveWand(Wand wand)
    {
        wands.Remove(wand);
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
