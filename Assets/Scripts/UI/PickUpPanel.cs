using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PickUpPanel : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] WandInfoPanel target;
    [SerializeField] List<WandInfoPanel> current;
    PlayerModel inventoryModel;
    Canvas canvas;
    GraphicRaycaster graphicRaycaster;
    List<RaycastResult> raycastResults = new List<RaycastResult>();
    Action<Wand, int> action;
    WandInfoPanel temp;
    int index;


    private void Awake()
    {
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        graphicRaycaster = canvas.GetComponent<GraphicRaycaster>();
        inventoryModel = IOCContainer.Instance.Get<PlayerModel>();
        gameObject.SetActive(false);
    }
    public void Init(Wand wand, Action<Wand, int> action)
    {
        this.action = action;
        target.gameObject.SetActive(true);
        target.Init(wand, null);
        for (int i = 0; i < inventoryModel.wands.Count; i++)
        {
            current[i].gameObject.SetActive(true);
            current[i].Init(inventoryModel.wands[i], null);
        }
    }
    private void OnEnable()
    {
        // MEventSystem.Instance.Send<SwitchActionMap>(new SwitchActionMap()
        // {
        //     actionMapName = "Editor"
        // });
        Time.timeScale = 0;

    }
    private void OnDisable()
    {
        Time.timeScale = 1;
    }
    private void Update()
    {
        //按Esc键调用action
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            action?.Invoke(null, -1);
            gameObject.SetActive(false);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        graphicRaycaster.Raycast(eventData, raycastResults);
        if (raycastResults.Count > 0)
        {
            if (raycastResults[0].gameObject.CompareTag("PickUpPanel"))
            {
                temp = raycastResults[0].gameObject.GetComponent<WandInfoPanel>();
                index = temp.transform.GetSiblingIndex();
                action?.Invoke(inventoryModel.GetWand(index), index);
            }

        }
        raycastResults.Clear();
    }
}
