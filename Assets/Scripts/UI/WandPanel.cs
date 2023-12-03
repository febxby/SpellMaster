using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WandPanel : MonoBehaviour, IShowable
{
    [SerializeField] Image wandSprite;
    [SerializeField] GameObject spells;
    [SerializeField] RectTransform rectTransform;
    public Wand wand;
    public GameObject infoPanel;
    public GameObject spellSlotPrefab;
    public void Init(Wand wand)
    {
        //TODO:优化，不用每次都生成，检测是否有子物体，有就直接SetActive(true)
        wandSprite.sprite = wand.spriteRenderer.sprite;
        wandSprite.color = wand.spriteRenderer.color;
        for (int i = 0; i < wand.mCapacity; i++)
        {
            var obj = new GameObject("Spell", typeof(RectTransform), typeof(Image));
            if (i < wand.mDeck.Count)
                obj.GetComponent<Image>().sprite = wand.mDeck[i].sprite;
            obj.transform.SetParent(spells.transform);
        }

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
