using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public struct SwitchSpellPos
{
    public int target;
    public int current;
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
    RectTransform rectTransform;
    Canvas canvas;
    GraphicRaycaster graphicRaycaster;
    List<RaycastResult> raycastResults = new List<RaycastResult>();
    public GameObject infoPanel;
    bool isDraging = false;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        graphicRaycaster = canvas.GetComponent<GraphicRaycaster>();
        lastParent = transform.parent;
        image = GetComponent<Image>();
    }
    public void Init(Spell spell)
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
        rectTransform.SetParent(canvas.transform, true);
        rectTransform.SetAsLastSibling();
        isDraging = true;
        infoPanel?.SetActive(false);
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
                rectTransform.offsetMax = Vector2.zero;
                rectTransform.offsetMin = Vector2.zero;

                temp.rectTransform.SetParent(lastParent, false);
                temp.rectTransform.offsetMax = Vector2.zero;
                rectTransform.offsetMin = Vector2.zero;
                temp.lastParent = temp.transform.parent;
                MEventSystem.Instance.Send<SwitchSpellPos>(
                    new SwitchSpellPos
                    {
                        target = temp.transform.parent.GetSiblingIndex(),
                        current = transform.parent.GetSiblingIndex()
                    }
                );
            }
            else if (raycastResults[idx].gameObject.CompareTag("SpellParentSlot"))
            {
                rectTransform.SetParent(raycastResults[idx].gameObject.transform, false);
                rectTransform.offsetMax = Vector2.zero;
                rectTransform.offsetMin = Vector2.zero;
                MEventSystem.Instance.Send<SwitchSpellPos>(
                    new SwitchSpellPos
                    {
                        target = raycastResults[idx].gameObject.transform.GetSiblingIndex(),
                        current = transform.parent.GetSiblingIndex()
                    }
                );
            }
            else
            {
                rectTransform.SetParent(lastParent, false);
                rectTransform.offsetMax = Vector2.zero;
                rectTransform.offsetMin = Vector2.zero;
            }
        }
        else
        {
            rectTransform.SetParent(lastParent, false);
            rectTransform.offsetMax = Vector2.zero;
            rectTransform.offsetMin = Vector2.zero;
        }
        lastParent = transform.parent;
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
