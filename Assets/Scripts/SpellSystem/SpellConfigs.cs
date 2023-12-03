using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New SpellConfigs", menuName = "SpellSystem/SpellConfigs"), System.Serializable]
public class SpellConfigs : ScriptableObject
{
    public List<Spell> configs;
}
