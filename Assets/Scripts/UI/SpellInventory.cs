using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
/// <summary>
/// 法术仓库
/// </summary>
public class SpellInventory : MonoBehaviour
{
    [SerializeField] List<SpellSlot> slots;
    [SerializeField] GameObject spellSlotPrefab;
    private void Awake()
    {
        slots = GetComponentsInChildren<SpellSlot>(true).ToList();
    }
    public void Init(List<Spell> spells)
    {
        // slots = new List<SpellSlot>(spells.Count);
        for (int i = 0; i < spells.Count; i++)
        {
            slots[i].Init(spells[i]);
            // var slot = Instantiate(spellSlotPrefab, transform).GetComponentInChildren<SpellSlot>();
            // slot.spell = spells[i];
            // slots.Add(slot);
        }
        transform.parent.gameObject.SetActive(false);
        // for (int i = 0; i < spells.Count; i++)
        // {
        //     slots[i].spell = spells[i];
        // }
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
