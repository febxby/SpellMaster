using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Divide : ICast
{

    public void Cast(Vector2 start, Vector2 end, Vector2 direction, Spell spell)
    {
        DivideModifier spell1 = (DivideModifier)spell;
        if (spell1 is null)
        {
            Debug.LogError("spell is not DivideModifier");
            return;
        }
        for (int i = 0; i < spell1.divideCount; i++)
        {
            for (int j = 0; j < spell1.spells.Count; j++)
            {
                spell1.spells[j].casts = spell1.casts;
                spell1.spells[j].Cast(start, end, direction,spell.owner);
            }
        }
        ObjectPoolFactory.Instance.Push(GetType(), this);
    }
}
