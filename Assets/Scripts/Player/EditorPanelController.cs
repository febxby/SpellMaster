using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
public interface IDragable : IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{

}

public interface IShowable : IPointerEnterHandler, IPointerExitHandler
{

}
// public struct ChangeCastWand
// {
//     public UnityAction<Vector2> action;
// }
public class EditorPanelController : MonoBehaviour
{
    // Start is called before the first frame update
    // [SerializeField] int wandCount = 4;
    // [SerializeField] int spellCount = 24;
    // [SerializeField] List<Wand> inventoryModel.wands;
    // [SerializeField] List<Spell> inventoryModel.spells;
    [SerializeField] WandInventory wandInventory;
    [SerializeField] SpellInventory spellInventory;
    [SerializeField] GameObject editorPanel;
    //挡住点击事件
    [SerializeField] GameObject blockPanel;
    InventoryModel inventoryModel;
    private void Awake()
    {
        inventoryModel = IOCContainer.Instance.Get<InventoryModel>();
        // editorPanel.SetActive(false);
        MEventSystem.Instance.Register<SwitchWandPos>(
            e =>
            {
                (inventoryModel.wands[e.bIndex], inventoryModel.wands[e.aIndex]) = (inventoryModel.wands[e.aIndex], inventoryModel.wands[e.bIndex]);
            }
        ).UnRegisterWhenGameObjectDestroy(gameObject);
        MEventSystem.Instance.Register<SwitchSpellPos>(
            e =>
            {
                (inventoryModel.spells[e.bIndex], inventoryModel.spells[e.aIndex]) = (inventoryModel.spells[e.aIndex], inventoryModel.spells[e.bIndex]);
            }
        ).UnRegisterWhenGameObjectDestroy(gameObject);
        MEventSystem.Instance.Register<AddWand>(
            e =>
            {
                wandInventory.Init(inventoryModel.wands);
            }
        ).UnRegisterWhenGameObjectDestroy(gameObject);
        MEventSystem.Instance.Register<AddSpell>(
            e =>
            {
                spellInventory.Init(inventoryModel.spells);
            }
        ).UnRegisterWhenGameObjectDestroy(gameObject);
        MEventSystem.Instance.Register<ChangeCastWand>(
            e =>
            {
                wandInventory.SelectSlot(e.index);
            }
        ).UnRegisterWhenGameObjectDestroy(gameObject);
    }
    void Start()
    {
        wandInventory.Init(inventoryModel.wands);
        spellInventory.Init(inventoryModel.spells);
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void OpenEditor()
    {
        editorPanel.SetActive(true);
        blockPanel.SetActive(false);
    }
    public void CloseEditor()
    {
        editorPanel.SetActive(false);
        blockPanel.SetActive(true);
    }
}
