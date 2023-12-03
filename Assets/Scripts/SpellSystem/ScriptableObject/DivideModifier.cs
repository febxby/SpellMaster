using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New DivideModifier", menuName = "SpellSystem/DivideModifier")]
public class DivideModifier : Spell
{
    [Header("扩展属性")]
    public int divideCount = 2;

}
