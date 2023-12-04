using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
/// <summary>
/// 更换法杖的位置
/// </summary>
public struct SwitchWandPos
{
    public int target;
    public int current;
}
/// <summary>
/// 法杖槽
/// </summary>
public class WandSlot : MonoBehaviour, IDragable, IShowable
{
    public Wand wand;
    public Image image;
    public GameObject infoPrefab;
    public Transform lastParent;
    public Selectable selectable;
    public Toggle toggle;
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
        selectable = GetComponent<Selectable>();
        // toggle = GetComponent<Toggle>();
    }
    public void Init(Wand wand)
    {
        this.wand = wand;
        if (wand != null)
        {
            image.sprite = wand.spriteRenderer.sprite;
            image.color = wand.spriteRenderer.color;
        }
        else
        {
            image.sprite = null;
            image.color = Color.clear;
        }

    }
    private void Start()
    {
        // if (wand != null)
        // {
        //     image.sprite = wand.spriteRenderer.sprite;
        //     image.color = wand.spriteRenderer.color;
        // }
    }
    void Update()
    {
        if (wand == null)
        {
            image.sprite = null;
            image.color = Color.clear;
        }
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        lastParent = transform.parent;
        rectTransform.SetParent(canvas.transform, true);
        rectTransform.SetAsLastSibling();
        isDraging = true;
        infoPanel?.SetActive(false);
    }

    public void OnDrag(PointerEventData eventData)
    {
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
            if (raycastResults[idx].gameObject.CompareTag("WandSlot"))
            {
                var temp = raycastResults[idx].gameObject.GetComponent<WandSlot>();
                rectTransform.SetParent(temp.lastParent, false);
                rectTransform.offsetMax = Vector2.zero;
                rectTransform.offsetMin = Vector2.zero;

                temp.rectTransform.SetParent(lastParent, false);
                temp.rectTransform.offsetMax = Vector2.zero;
                temp.rectTransform.offsetMin = Vector2.zero;
                temp.lastParent = temp.transform.parent;
                MEventSystem.Instance.Send<SwitchWandPos>(
                    new SwitchWandPos
                    {
                        //BUG:只交换了数据，没有交换游戏对象的位置
                        target = temp.transform.parent.GetSiblingIndex(),
                        current = transform.parent.GetSiblingIndex()
                    }
                );
            }
            else if (raycastResults[idx].gameObject.CompareTag("WandParentSlot"))
            {
                rectTransform.SetParent(raycastResults[idx].gameObject.transform, false);
                rectTransform.offsetMax = Vector2.zero;
                rectTransform.offsetMin = Vector2.zero;
                MEventSystem.Instance.Send<SwitchWandPos>(
                    new SwitchWandPos
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

                // rectTransform.SetParent(parentRectTransform, true);
                // rectTransform.SetSiblingIndex(index);
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
        MEventSystem.Instance.Send<ChangeCastWand>(
            new ChangeCastWand { index = transform.parent.GetSiblingIndex() }
        );
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

        // if (infoPanel != null)
        // {
        //     infoPanel.SetActive(true);
        //     infoPanel.GetComponent<WandInfoPanel>().SetPosition(eventData.position);

        //     return;
        // }
        // if (wand != null)
        // {
        //     infoPanel = Instantiate(infoPrefab, canvas.transform);
        //     infoPanel.GetComponent<WandInfoPanel>().Init(wand, eventData);
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
