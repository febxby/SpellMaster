using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WandPanel : MonoBehaviour, IShowable
{
    [SerializeField] Image image;
    [SerializeField] GameObject spells;
    [SerializeField] RectTransform rectTransform;
    public Wand wand;
    public GameObject infoPanel;
    public GameObject spellSlotPrefab;
    public List<SpellSlot> spellSlots;
    public void Init(Wand wand)
    {
        spellSlots = new List<SpellSlot>();
        //TODO:优化，不用每次都生成，检测是否有子物体，有就直接SetActive(true)
        this.wand = wand;
        if (wand != null)
        {
            gameObject.SetActive(true);
            image.sprite = wand.spriteRenderer.sprite;
            image.color = wand.spriteRenderer.color;
        }
        else
        {
            gameObject.SetActive(false);
            spellSlots = GetComponentsInChildren<SpellSlot>(true).ToList();
            return;
        }
        for (int i = 0; i < wand.mCapacity; i++)
        {
            GameObject obj = Instantiate(spellSlotPrefab, spells.transform);
            if (i < wand.mDeck.Count)
                if (wand.mDeck[i] != null)
                {
                    obj.GetComponentInChildren<SpellSlot>().Init(wand.mDeck[i]);
                }
        }
        spellSlots = GetComponentsInChildren<SpellSlot>(true).ToList();
    }
    public void UpdateUI(Wand wand)
    {
        this.wand = wand;
        if (wand != null)
        {
            gameObject.SetActive(true);
            image.sprite = wand.spriteRenderer.sprite;
            image.color = wand.spriteRenderer.color;
        }
        else
        {
            gameObject.SetActive(false);
            spellSlots = GetComponentsInChildren<SpellSlot>(true).ToList();
            return;
        }
        Debug.Log(gameObject.name);

        if (spellSlots != null)
            if (spellSlots.Count > wand.mCapacity)
            {
                for (int i = 0; i < spellSlots.Count; i++)
                {
                    if (i < wand.mCapacity)
                    {
                        spellSlots[i].gameObject.SetActive(true);
                        spellSlots[i].Init(wand.mDeck[i]);
                    }
                    else
                    {
                        spellSlots[i].gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                for (int i = 0; i < wand.mCapacity; i++)
                {
                    Debug.Log(i);
                    if (i < spellSlots.Count)
                    {
                        spellSlots[i].gameObject.SetActive(true);
                        if (i < wand.mDeck.Count)
                            if (wand.mDeck[i] != null)
                                spellSlots[i].Init(wand.mDeck[i]);
                    }
                    else
                    {
                        GameObject obj = Instantiate(spellSlotPrefab, spells.transform);
                        //索引小于法杖容量的就进行初始化，大于容量说明法杖后面没有法术了，不用初始化
                        if (i < wand.mDeck.Count)
                            if (wand.mDeck[i] != null)
                            {
                                obj.GetComponentInChildren<SpellSlot>().Init(wand.mDeck[i]);
                            }
                    }
                }
            }

        spellSlots = GetComponentsInChildren<SpellSlot>(true).ToList();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
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
            infoPanel.SetActive(false);
        }
    }
    private void OnDisable()
    {
        if (infoPanel != null)
        {
            infoPanel.SetActive(false);
        }
    }
}
