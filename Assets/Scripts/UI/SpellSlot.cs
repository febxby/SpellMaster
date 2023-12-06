using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
/// <summary>
/// 交换法术UI位置
/// </summary>
public struct SwitchSpellPos
{
    public int target;
    public int current;
    public List<Spell> targetList;
    public List<Spell> currentList;
}
public interface ISlot<T>
{

}
/// <summary>
/// 法术槽
/// </summary>
public class SpellSlot : MonoBehaviour, IDragable, IShowable
{
    public Spell spell;
    public Image image;
    public GameObject infoPrefab;
    public Transform lastParent;
    public string parentTag;
    public GameObject infoPanel;
    RectTransform rectTransform;
    Canvas canvas;
    GraphicRaycaster graphicRaycaster;
    List<RaycastResult> raycastResults = new List<RaycastResult>();
    bool isDraging = false;
    WandPanel wandPanel;
    public List<Spell> ownerList;
    public Action<int, Spell> SetFunc;
    public Func<int, Spell> GetFunc;
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        graphicRaycaster = canvas.GetComponent<GraphicRaycaster>();
        lastParent = transform.parent;
        image = GetComponent<Image>();
        parentTag = transform.parent.parent.tag;
    }
    public SpellSlot SetParentObj(List<Spell> spells)
    {
        ownerList = spells;
        return this;
    }
    public SpellSlot AddDelegate(Action<int, Spell> setFunc, Func<int, Spell> getFunc)
    {
        SetFunc = setFunc;
        GetFunc = getFunc;
        return this;
    }
    public SpellSlot Init(Spell spell)
    {
        this.spell = spell;
        if (spell != null)
        {
            image.sprite = spell.sprite;
            image.color = Color.white;
        }
        else
        {
            image.sprite = null;
            image.color = Color.clear;
        }
        return this;

    }
    private void Start()
    {
        // if (spell != null)
        // {
        //     image.sprite = spell.sprite;
        // }
    }
    void Update()
    {
        if (spell == null)
        {
            image.sprite = null;
            image.color = Color.clear;
        }
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (spell == null)
        {
            return;
        }
        lastParent = transform.parent;
        transform.parent.parent.TryGetComponent<WandPanel>(out wandPanel);
        rectTransform.SetParent(canvas.transform, true);
        rectTransform.SetAsLastSibling();
        isDraging = true;
        if (infoPanel != null)
            infoPanel.SetActive(false);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (spell == null)
        {
            return;
        }
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        infoPanel?.SetActive(true);
        isDraging = false;
        graphicRaycaster.Raycast(eventData, raycastResults);
        if (raycastResults.Count > 0)
        {
            var idx = 0;
            if (raycastResults.Count > 1)
            {
                idx = 1;
            }
            if (raycastResults[idx].gameObject.CompareTag("SpellSlot"))
            {
                var temp = raycastResults[idx].gameObject.GetComponent<SpellSlot>();
                rectTransform.SetParent(temp.lastParent, false);
                rectTransform.offsetMax = new Vector2(-10, -10);
                rectTransform.offsetMin = new Vector2(10, 10);

                temp.rectTransform.SetParent(lastParent, false);
                temp.rectTransform.offsetMax = new Vector2(-10, -10);
                temp.rectTransform.offsetMin = new Vector2(10, 10);
                temp.lastParent = temp.transform.parent;
                var tempIndex = temp.transform.parent.GetSiblingIndex();
                var thisIndex = transform.parent.GetSiblingIndex();
                var spell = temp.GetFunc(thisIndex);
                temp.SetFunc(thisIndex, GetFunc(tempIndex));
                SetFunc(tempIndex, spell);
                (SetFunc, temp.SetFunc) = (temp.SetFunc, SetFunc);
                (GetFunc, temp.GetFunc) = (temp.GetFunc, GetFunc);
            }
            else
            {
                rectTransform.SetParent(lastParent, false);
                rectTransform.offsetMax = new Vector2(-10, -10);
                rectTransform.offsetMin = new Vector2(10, 10);
            }
        }
        else
        {
            rectTransform.SetParent(lastParent, false);
            rectTransform.offsetMax = new Vector2(-10, -10);
            rectTransform.offsetMin = new Vector2(10, 10);
        }
        lastParent = transform.parent;
        parentTag = transform.parent.parent.tag;
        raycastResults.Clear();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //TODO:切换法术
        // MEventSystem.Instance.Send<ChangeCastWand>(
        //     new ChangeCastWand { index = transform.parent.GetSiblingIndex() + 1 }
        // );
    }

    public void ShowInfo()
    {
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isDraging)
            return;
        MEventSystem.Instance.Send<ShowInfoPanel>(new ShowInfoPanel
        {
            showable = this,
            eventData = eventData
        });
        // if (isDraging || spell == null)
        //     return;

        // if (infoPanel == null)
        // {
        //     infoPanel = Instantiate(infoPrefab, canvas.transform);
        //     infoPanel.GetComponent<SpellInfoPanel>().Init(spell, eventData);
        // }
        // else
        // {
        //     infoPanel.SetActive(true);
        //     infoPanel.GetComponent<SpellInfoPanel>().SetPosition(eventData.position);
        // }
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
