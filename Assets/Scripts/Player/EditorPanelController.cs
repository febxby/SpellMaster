using UnityEngine;
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
    [SerializeField] WandPanel wandPanel;
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
                (inventoryModel.wands[e.current], inventoryModel.wands[e.target]) =
                (inventoryModel.wands[e.target], inventoryModel.wands[e.current]);
                (wandInventory.slots[e.current], wandInventory.slots[e.target]) =
                (wandInventory.slots[e.target], wandInventory.slots[e.current]);
                MEventSystem.Instance.Send(new ChangeCastWand { index = e.current });
            }
        ).UnRegisterWhenGameObjectDestroy(gameObject);
        MEventSystem.Instance.Register<SwitchSpellPos>(
            e =>
            {
                if (e.wand != null)
                {
                    Spell temp = e.wand.mDeck[e.current];
                    e.wand.AddSpell(inventoryModel.spells[e.target], e.current);
                    Debug.Log(temp.name);
                    inventoryModel.spells[e.target] = temp;
                }
                else
                {
                    (inventoryModel.spells[e.current], inventoryModel.spells[e.target]) =
                    (inventoryModel.spells[e.target], inventoryModel.spells[e.current]);
                    (spellInventory.slots[e.current], spellInventory.slots[e.target]) =
                    (spellInventory.slots[e.target], spellInventory.slots[e.current]);
                }
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
        MEventSystem.Instance.Register<AddWand>(
        e =>
        {
            wandInventory.UpdateUI(inventoryModel.wands);
        }
        ).UnRegisterWhenGameObjectDestroy(gameObject);
        MEventSystem.Instance.Register<AddSpell>(
        e =>
        {
            spellInventory.UpdateUI(inventoryModel.spells);
        }
        ).UnRegisterWhenGameObjectDestroy(gameObject);
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
