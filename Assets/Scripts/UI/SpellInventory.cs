using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
/// <summary>
/// 法术仓库
/// </summary>
public class SpellInventory : MonoBehaviour
{
    public List<SpellSlot> slots;
    [SerializeField] GameObject spellSlotPrefab;
    SpellSlot spellSlot;
    // private void Awake()
    // {
    //     slots = GetComponentsInChildren<SpellSlot>(true).ToList();
    // }
    public void Init(List<Spell> spells)
    {
        slots = new List<SpellSlot>(spells.Count);
        for (int i = 0; i < spells.Count; i++)
        {
            spellSlot = Instantiate(spellSlotPrefab, transform).GetComponentInChildren<SpellSlot>();
            spellSlot.Init(spells[i]);
            slots.Add(spellSlot);
        }
        transform.parent.gameObject.SetActive(false);
    }
    public void UpdateUI(List<Spell> spells)
    {
        for (int i = 0; i < spells.Count; i++)
        {
            slots[i].Init(spells[i]);
        }
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
