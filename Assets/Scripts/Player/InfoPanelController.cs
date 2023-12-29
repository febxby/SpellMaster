using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public struct ShowInfoPanel
{
    public IShowable showable;
    public PointerEventData eventData;
}
public class InfoPanelController : MonoBehaviour
{
    [SerializeField] Canvas canvas;
    [SerializeField] GameObject wandInfoPanelPrefab;
    [SerializeField] GameObject spellInfoPanelPrefab;
    void Awake()
    {
        MEventSystem.Instance.Register<ShowInfoPanel>(
            e => ShowInfoPanel(e.showable, e.eventData)
        ).UnRegisterWhenGameObjectDestroy(gameObject);
    }
    //TODO:添加显示数据接口
    public void ShowInfoPanel(IShowable obj, PointerEventData eventData)
    {
        if (obj == null)
            return;
        else if (obj is WandSlot wandSlot)
        {
            if (wandSlot.wand == null)
                return;

            if (wandSlot.infoPanel == null)
            {
                // wandSlot.infoPanel = Instantiate(wandInfoPanelPrefab, canvas.transform);
                wandSlot.infoPanel = GameObjectPool.Instance.GetObject(wandInfoPanelPrefab);
                wandSlot.infoPanel.transform.SetParent(canvas.transform);
                wandSlot.infoPanel.GetComponent<WandInfoPanel>().Init(wandSlot.wand, eventData);
            }
            else
            {
                wandSlot.infoPanel.SetActive(true);
                wandSlot.infoPanel.GetComponent<WandInfoPanel>().SetPosition(eventData.position);
            }
        }
        else if (obj is SpellSlot || obj is Saleable || obj is DropItem)
        {
            dynamic showable = null;
            bool isFollowMouse = true;
            if (obj.GetType() == typeof(SpellSlot))
            {
                showable = obj as SpellSlot;
                isFollowMouse = true;
            }
            else if (obj.GetType() == typeof(Saleable))
            {
                showable = obj as Saleable;
                isFollowMouse = false;
            }
            else if (obj.GetType() == typeof(DropItem))
            {
                showable = obj as DropItem;
                isFollowMouse = false;
            }
            if (showable.spell == null)
                return;

            if (showable.infoPanel == null)
            {
                // spellSlot.infoPanel = Instantiate(spellInfoPanelPrefab, canvas.transform);
                showable.infoPanel = GameObjectPool.Instance.GetObject(spellInfoPanelPrefab);
                showable.infoPanel.transform.SetParent(canvas.transform);
                showable.infoPanel.GetComponent<SpellInfoPanel>().Init(showable.spell, eventData, isFollowMouse);
            }
            else
            {
                showable.infoPanel.SetActive(true);
                showable.infoPanel.GetComponent<SpellInfoPanel>().SetPosition(eventData.position);
            }
        }
        else if (obj is WandPanel wandPanel)
        {
            if (wandPanel.wand == null)
                return;

            if (wandPanel.infoPanel == null)
            {
                wandPanel.infoPanel = GameObjectPool.Instance.GetObject(wandInfoPanelPrefab);
                wandPanel.infoPanel.transform.SetParent(canvas.transform);
                // wandPanel.infoPanel = Instantiate(wandInfoPanelPrefab, canvas.transform);
                wandPanel.infoPanel.GetComponent<WandInfoPanel>().Init(wandPanel.wand, eventData);
            }
            else
            {
                wandPanel.infoPanel.SetActive(true);
                wandPanel.infoPanel.GetComponent<WandInfoPanel>().SetPosition(eventData.position);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
