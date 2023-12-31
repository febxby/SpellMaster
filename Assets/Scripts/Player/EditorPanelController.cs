using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements.Experimental;
public interface IDragable : IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{

}

public interface IShowable
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
    PlayerModel playerModel;
    private void Awake()
    {
        playerModel = IOCContainer.Instance.Get<PlayerModel>();
        // editorPanel.SetActive(false);
        MEventSystem.Instance.Register<SwitchWandPos>(
            e =>
            {
                (playerModel.wands[e.current], playerModel.wands[e.target]) =
                (playerModel.wands[e.target], playerModel.wands[e.current]);
                (wandInventory.slots[e.current], wandInventory.slots[e.target]) =
                (wandInventory.slots[e.target], wandInventory.slots[e.current]);
                MEventSystem.Instance.Send(new ChangeCastWand { index = e.current});
                wandInventory.UpdateUI(playerModel.wands);
            }
        ).UnRegisterWhenGameObjectDestroy(gameObject);
        // MEventSystem.Instance.Register<SwitchSpellPos>(
        //     e =>
        //     {
        //         // if (e != null)
        //         // // {
        //         // //     Spell temp = e.wand.mDeck[e.current];
        //         // //     e.wand.AddSpell(inventoryModel.spells[e.target], e.current);
        //         // //     inventoryModel.spells[e.target] = temp;
        //         // }
        //         // else
        //         // {
        //         //     (inventoryModel.spells[e.current], inventoryModel.spells[e.target]) =
        //         //     (inventoryModel.spells[e.target], inventoryModel.spells[e.current]);
        //         //     (spellInventory.slots[e.current], spellInventory.slots[e.target]) =
        //         //     (spellInventory.slots[e.target], spellInventory.slots[e.current]);
        //         // }
        //     }
        // ).UnRegisterWhenGameObjectDestroy(gameObject);

        MEventSystem.Instance.Register<ChangeCastWand>(
            e =>
            {
                wandInventory.SelectSlot(e.index);
            }
        ).UnRegisterWhenGameObjectDestroy(gameObject);

    }
    void Start()
    {
        wandInventory.Init(playerModel.wands);
        spellInventory.Init(playerModel.spells,
        (index, spell) => playerModel.Add<Spell>(spell, index),
        (index) => playerModel.GetSpell(index));
        MEventSystem.Instance.Register<AddWand>(
        e =>
        {
            wandInventory.UpdateUI(playerModel.wands);
        }
        ).UnRegisterWhenGameObjectDestroy(gameObject);
        MEventSystem.Instance.Register<AddSpell>(
        e =>
        {
            spellInventory.UpdateUI(playerModel.spells,
            (index, spell) => playerModel.Add<Spell>(spell, index),
            (index) => playerModel.GetSpell(index));
        }
        ).UnRegisterWhenGameObjectDestroy(gameObject);
        MEventSystem.Instance.Send(new HealthChange { value = playerModel.CurrentHealth / playerModel.MaxHealth });
        MEventSystem.Instance.Send(new CoinChange { value = playerModel.Coin });
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
