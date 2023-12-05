using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
/// <summary>
/// 法杖仓库
/// </summary>
public class WandInventory : MonoBehaviour
{
    // Start is called before the first frame update
    public List<WandSlot> slots;
    public List<WandPanel> wandPanels;
    private void Awake()
    {
        slots = GetComponentsInChildren<WandSlot>(true).ToList();
        slots[0].selectable.Select();
    }
    public void Init(List<Wand> wands)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].Init(wands[i]);
            wandPanels[i].Init(wands[i]);
        }
    }
    public void UpdateUI(List<Wand> wands)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].UpdateUI(wands[i]);
            wandPanels[i].UpdateUI(wands[i]);
        }
    }
    public void SelectSlot(int index)
    {
        slots[index].selectable.Select();
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
