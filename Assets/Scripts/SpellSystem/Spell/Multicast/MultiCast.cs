using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiCast : ICast
{
    public void Cast(Vector2 start, Vector2 end, Vector2 direction, Spell spell)
    {
        for (int i = 0; i < spell.spells.Count; i++)
        {
            spell.spells[i].casts = spell.casts;
            spell.spells[i].Cast(start, end, direction, spell.owner);
        }
        ObjectPoolFactory.Instance.Push(GetType(), this);
    }
}
