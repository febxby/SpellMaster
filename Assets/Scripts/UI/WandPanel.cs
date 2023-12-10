using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WandPanel : MonoBehaviour, IShowable, IPointerMoveHandler
{
    [SerializeField] Image image;
    [SerializeField] GameObject spells;
    [SerializeField] RectTransform rectTransform;
    public Wand wand;
    public GameObject infoPanel;
    public GameObject spellSlotPrefab;
    public List<SpellSlot> spellSlots;
    public void Init(Wand wand, Action<int, Spell> action, Func<int, Spell> func)
    {
        spellSlots = new List<SpellSlot>();
        this.wand = wand;
        if (wand != null)
        {
            gameObject.SetActive(true);
            image.sprite = wand.spriteRenderer.sprite;
            image.color = Color.white;
        }
        else
        {
            gameObject.SetActive(false);
            spellSlots = GetComponentsInChildren<SpellSlot>(true).ToList();
            return;
        }
        for (int i = 0; i < wand.Capacity; i++)
        {
            GameObject obj = Instantiate(spellSlotPrefab, spells.transform);
            // if (i < wand.mDeck.Count)
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
            gameObject.SetActive(true);
            image.sprite = wand.spriteRenderer.sprite;
            image.color = Color.white;
        }
        else
        {
            gameObject.SetActive(false);
            spellSlots = GetComponentsInChildren<SpellSlot>(true).ToList();
            return;
        }
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
        #region 
        // if (spellSlots != null)
        //     if (spellSlots.Count > wand.Capacity)
        //     {
        //         for (int i = 0; i < spellSlots.Count; i++)
        //         {
        //             if (i < wand.Capacity)
        //             {
        //                 spellSlots[i].gameObject.SetActive(true);
        //                 spellSlots[i].Init(wand.Deck[i]).SetParentObj(wand.Deck).AddDelegate(action, func);
        //             }
        //             else
        //                 spellSlots[i].gameObject.SetActive(false);
        //         }
        //     }
        //     else
        //     {
        //         for (int i = 0; i < wand.Capacity; i++)
        //         {
        //             if (i < spellSlots.Count)
        //             {
        //                 spellSlots[i].gameObject.SetActive(true);
        //                 // if (i < wand.mDeck.Count)
        //                 if (wand.Deck[i] != null)
        //                     spellSlots[i].Init(wand.Deck[i]).SetParentObj(wand.Deck).AddDelegate(action, func);
        //                 else
        //                     spellSlots[i].Init(null).SetParentObj(wand.Deck).AddDelegate(action, func);
        //             }
        //             else
        //             {
        //                 GameObject obj = Instantiate(spellSlotPrefab, spells.transform);
        //                 //索引小于法杖容量的就进行初始化，大于容量说明法杖后面没有法术了，不用初始化
        //                 // if (i < wand.mDeck.Count)
        //                 if (wand.Deck[i] != null)
        //                     obj.GetComponentInChildren<SpellSlot>().Init(wand.Deck[i]).SetParentObj(wand.Deck).AddDelegate(action, func);
        //                 else
        //                     obj.GetComponentInChildren<SpellSlot>().Init(null).SetParentObj(wand.Deck).AddDelegate(action, func);
        //             }
        //         }
        //     }
        #endregion
        spellSlots = GetComponentsInChildren<SpellSlot>(true).ToList();
        if (infoPanel != null)
        {
            GameObjectPool.Instance.PushObject(infoPanel);
            infoPanel = null;
        }

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerEnter != gameObject)
            return;
        MEventSystem.Instance.Send<ShowInfoPanel>(new ShowInfoPanel
        {
            showable = this,
            eventData = eventData
        });
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (infoPanel != null)
        {
            // infoPanel.SetActive(false);
            GameObjectPool.Instance.PushObject(infoPanel);
            infoPanel = null;
        }
    }
    private void OnDisable()
    {
        if (infoPanel != null)
        {
            GameObjectPool.Instance.PushObject(infoPanel);
            infoPanel = null;
            // infoPanel.SetActive(false);
        }
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject != gameObject)
        {
            if (infoPanel != null)
            {
                // infoPanel.SetActive(false);
                GameObjectPool.Instance.PushObject(infoPanel);
                infoPanel = null;
            }
        }
        else
            MEventSystem.Instance.Send<ShowInfoPanel>(new ShowInfoPanel
            {
                showable = this,
                eventData = eventData
            });
    }
}
