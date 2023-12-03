using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Formation : ICast
{
    // [SerializeField] float angle = 360;
    public void Cast(Vector2 start, Vector2 end, Vector2 direction, Spell spell)
    {
        Quaternion quaternion;
        for (int i = 0; i < spell.spells.Count; i++)
        {
            spell.spells[i].casts = spell.casts;
            if (spell.spells.Count == 1)
            {
                quaternion = Quaternion.AngleAxis(0, Vector3.forward);
            }
            else if (spell.spread == 360)
            {
                quaternion = Quaternion.AngleAxis(spell.spread / spell.spells.Count * i, Vector3.forward);
            }
            else
            {
                if (spell.spells.Count % 2 == 1)
                {
                    quaternion = Quaternion.AngleAxis(spell.spread / (spell.spells.Count - 1) * (i - spell.spells.Count / 2), Vector3.forward);
                }
                else
                {
                    quaternion = Quaternion.AngleAxis(spell.spread / (spell.spells.Count - 1) * (i - spell.spells.Count / 2) + (spell.spread / (spell.spells.Count - 1) / 2), Vector3.forward);
                }
            }
            spell.spells[i].Cast(start, end, quaternion * direction, spell.owner);
        }
        ObjectPoolFactory.Instance.Push(GetType(), this);

    }

}
