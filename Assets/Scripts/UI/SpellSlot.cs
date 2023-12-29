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
public class SpellSlot : MonoBehaviour, IDragable, IShowable, IPointerEnterHandler, IPointerExitHandler
{
    public Spell spell;
    public Image image;
    public GameObject infoPrefab;
    public Transform lastParent;
    public GameObject infoPanel;
    public bool canDrag => SetFunc != null;
    public GameObject dropItemPrefab;
    RectTransform rectTransform;
    Canvas canvas;
    GraphicRaycaster graphicRaycaster;
    List<RaycastResult> raycastResults = new List<RaycastResult>();
    bool isDraging = false;
    WandPanel wandPanel;
    public Action<int, Spell> SetFunc;
    public Func<int, Spell> GetFunc;
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        graphicRaycaster = canvas.GetComponent<GraphicRaycaster>();
        lastParent = transform.parent;
        image = GetComponent<Image>();
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
        if (!canDrag)
            return;
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
        if (!canDrag)
            return;
        if (spell == null)
        {
            return;
        }
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!canDrag)
            return;
        // infoPanel?.SetActive(true);
        isDraging = false;
        graphicRaycaster.Raycast(eventData, raycastResults);
        //BUG:当拖拽后放在原地会直接扔掉法术
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
                rectTransform.offsetMax = Vector2.zero;
                rectTransform.offsetMin = Vector2.zero;

                temp.rectTransform.SetParent(lastParent, false);
                temp.rectTransform.offsetMax = Vector2.zero;
                temp.rectTransform.offsetMin = Vector2.zero;
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
                rectTransform.offsetMax = Vector2.zero;
                rectTransform.offsetMin = Vector2.zero;
                //屏幕中间位置
                Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 0));
                pos.z = 0;
                GameObjectPool.Instance.GetObject(dropItemPrefab).
                SetPositionAndRotation(pos, Quaternion.identity).
                GetComponent<DropItem>().Init(spell);
                SetFunc(transform.parent.GetSiblingIndex(), null);
                Init(null);
            }
        }
        else
        {
            rectTransform.SetParent(lastParent, false);
            rectTransform.offsetMax = Vector2.zero;
            rectTransform.offsetMin = Vector2.zero;
            //屏幕中间位置
            Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            pos.z = 0;
            GameObjectPool.Instance.GetObject(dropItemPrefab).
            SetPositionAndRotation(pos, Quaternion.identity).
            GetComponent<DropItem>().Init(spell);
            SetFunc(transform.parent.GetSiblingIndex(), null);
            Init(null);
        }
        lastParent = transform.parent;
        raycastResults.Clear();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
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
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (infoPanel != null)
        {
            GameObjectPool.Instance.PushObject(infoPanel);
            infoPanel = null;
            // infoPanel.SetActive(false);
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

}
