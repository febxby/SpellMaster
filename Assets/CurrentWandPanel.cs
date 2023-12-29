using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public struct UpdateCurrentWandPanel
{
    public bool isChange;
    public Wand wand;
}
public class CurrentWandPanel : MonoBehaviour
{
    [SerializeField] GameObject spells;
    [SerializeField] RectTransform rectTransform;
    public Wand wand;
    public GameObject spellSlotPrefab;
    public List<SpellSlot> spellSlots;
    private void Awake()
    {
        spellSlots = new List<SpellSlot>();
        MEventSystem.Instance.Register<UpdateCurrentWandPanel>(
            e =>
            {
                if (e.isChange)
                    UpdateUI(e.wand, null, null);
                else
                {
                    if (e.wand == wand)
                    {
                        UpdateUI(e.wand, null, null);
                    }
                }
            }
        ).UnRegisterWhenGameObjectDestroy(gameObject);
    }
    public void Init(Wand wand, Action<int, Spell> action, Func<int, Spell> func)
    {
        spellSlots = new List<SpellSlot>();
        this.wand = wand;
        if (wand != null)
        {
            transform.parent.gameObject.SetActive(true);
        }
        else
        {
            transform.parent.gameObject.SetActive(false);
            spellSlots = GetComponentsInChildren<SpellSlot>(true).ToList();
            return;
        }
        for (int i = 0; i < wand.Capacity; i++)
        {
            GameObject obj = Instantiate(spellSlotPrefab, spells.transform);
            if (wand.Deck[i] != null)
                obj.GetComponentInChildren<SpellSlot>().Init(wand.Deck[i]).AddDelegate(action, func);
            else
                obj.GetComponentInChildren<SpellSlot>().Init(null).AddDelegate(action, func);
        }
        spellSlots = GetComponentsInChildren<SpellSlot>(true).ToList();
    }
    public void UpdateUI(Wand wand, Action<int, Spell> action, Func<int, Spell> func)
    {
        this.wand = wand;
        if (wand != null)
        {
            transform.parent.gameObject.SetActive(true);
        }
        else
        {
            transform.parent.gameObject.SetActive(false);
            return;
        }
        spellSlots = GetComponentsInChildren<SpellSlot>(true).ToList();
        int maxCount = Math.Max(spellSlots.Count, wand.Capacity);
        for (int i = 0; i < maxCount; i++)
        {
            SpellSlot slot;
            if (i < spellSlots.Count)
            {
                slot = spellSlots[i];
                slot.transform.parent.gameObject.SetActive(i < wand.Capacity);
            }
            else
            {
                GameObject obj = Instantiate(spellSlotPrefab, spells.transform);
                slot = obj.GetComponentInChildren<SpellSlot>();
            }

            if (i < wand.Capacity)
            {
                slot.Init(wand.Deck[i]).AddDelegate(action, func);
            }
            else if (i < spellSlots.Count)
            {
                slot.Init(null).AddDelegate(action, func);
            }
        }
        spellSlots = GetComponentsInChildren<SpellSlot>(true).ToList();

    }
}
