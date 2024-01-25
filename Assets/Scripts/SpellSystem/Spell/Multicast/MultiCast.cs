using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiCast : ICast
{
    public void Cast(Vector2 start, Vector2 end, Vector2 direction, Spell spell, string uniqueId)
    {
        for (int i = 0; i < spell.spells.Count; i++)
        {
            spell.spells[i].casts = spell.casts;
            spell.spells[i].attaches = spell.attaches;
            spell.spells[i].Init(start, end, direction, spell.owner, uniqueId);
        }
        ObjectPoolFactory.Instance.Push(GetType(), this);
    }
}
