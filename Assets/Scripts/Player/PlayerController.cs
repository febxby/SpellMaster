using System;
using UnityEngine;
/// <summary>
/// 切换使用的法杖
/// </summary>
public struct ChangeCastWand
{
    public int index;
    // public Wand wand;
}
/// <summary>
/// 添加法杖
/// </summary>
public struct AddWand
{
    /// <summary>
    /// 法杖类
    /// </summary>
    public Wand wand;
}
/// <summary>
/// 添加法术
/// </summary>
public struct AddSpell
{
    /// <summary>
    /// 法术类
    /// </summary>
    public Spell spell;
}
public interface IPickUpable
{
    bool CanPickUp(GameObject gameObject);
}
public interface IPickUp
{
    void PickUp();
}

public class PlayerController : MonoBehaviour
{
    // [SerializeField] List<Wand> wands;
    [SerializeField] Wand currentWand;
    [SerializeField] float speed = 5f;
    [SerializeField] float flySPeed = 5f;
    [SerializeField] float acceleration = 5f;
    [SerializeField] GameObject pickUpPanel;
    [SerializeField] IPickUpable pickUpable;
    // [SerializeField] float pickUpCooldown = 0.5f;
    [SerializeField] bool Test = false;
    // float lastPickUpTime = -1f;
    bool isPickingUp;
    InventoryModel inventoryModel;
    Rigidbody2D rb;
    Vector2 moveDirection;
    bool canFly;
    float currentSpeed;
    RaycastHit2D[] hits = new RaycastHit2D[1];
    private void Awake()
    {
        inventoryModel = IOCContainer.Instance.Get<InventoryModel>();
        rb = GetComponent<Rigidbody2D>();
        // wands = GetComponentsInChildren<Wand>(true).ToList();
        MEventSystem.Instance.Register<AddWand>(
            e =>
            {
                if (e.wand == null) return;
                AddWandToInventory(e.wand);
            }
        ).UnRegisterWhenGameObjectDestroy(gameObject);
        MEventSystem.Instance.Register<ChangeCastWand>(
            e =>
            {
                ChangeCastWand(e.index);

            }
        ).UnRegisterWhenGameObjectDestroy(gameObject);
    }
    public void ChangeCastWand(int index)
    {
        if (index < 0 || index >= inventoryModel.wands.Count)
        {
            currentWand = null;
            return;
        }
        if (currentWand != null && currentWand != inventoryModel.wands[index])
        {
            currentWand.gameObject.SetActive(false);
        }
        currentWand = inventoryModel.wands[index];
        if (currentWand != null)
        {
            currentWand.gameObject.SetActive(true);
        }
    }
    void Start()
    {

        currentWand = inventoryModel.wands[0];

    }
    public void Cast(Vector2 pos)
    {
        if (currentWand == null || !currentWand.gameObject.activeSelf)
            return;
        currentWand.Cast(pos);

    }
    // Update is called once per frame
    void Update()
    {

    }
    private void FixedUpdate()
    {
        Move();
        Fly();
    }
    public void MoveInput(Vector2 direction)
    {
        moveDirection = direction;
        if (direction.y > 0)
        {
            canFly = true;
        }
        else
        {
            canFly = false;
        }
    }
    public void FlyInput(bool value)
    {
        canFly = value;
    }
    public void Move()
    {
        currentSpeed = Mathf.MoveTowards(currentSpeed, speed, Time.deltaTime * acceleration);
        rb.velocity = new Vector2(moveDirection.x * currentSpeed, rb.velocity.y);
    }
    public void Fly()
    {
        if (!canFly) return;
        rb.velocity = new Vector2(rb.velocity.x, flySPeed);
    }
    private void AddWandToInventory(Wand wand)
    {
        if (wand == null) return;
        if (!inventoryModel.Add(wand))
        {
            Wand temp = wand;
            pickUpPanel.SetActive(true);
            pickUpPanel.GetComponent<PickUpPanel>().Init(wand,
            (e, index) =>
            {
                if (e == null) return;
                inventoryModel.Add(temp, index);
                MEventSystem.Instance.Send<AddWand>();
                MEventSystem.Instance.Send<ChangeCastWand>(
                new ChangeCastWand { index = inventoryModel.wands.IndexOf(temp) });
                e.gameObject.layer = LayerMask.NameToLayer("PickUpable");
                e.transform.SetParent(null);
                e.gameObject.SetActive(true);
                temp.gameObject.layer = LayerMask.NameToLayer("Wand");
                temp.transform.SetParent(transform);
                temp.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                pickUpPanel.SetActive(false);
            });
        }
        else
        {
            MEventSystem.Instance.Send<AddWand>();
            MEventSystem.Instance.Send<ChangeCastWand>(
                new ChangeCastWand { index = inventoryModel.wands.IndexOf(wand) });
            wand.gameObject.layer = LayerMask.NameToLayer("Wand");
            wand.transform.SetParent(transform);
            wand.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        }
    }
    public void PickUp()
    {
        if (pickUpable is null || isPickingUp) return;
        isPickingUp = true;
        if (pickUpable.CanPickUp(gameObject))
        {
            if (pickUpable is Wand wand)
            {
                AddWandToInventory(wand);
            }
            else if (pickUpable is DropItem item)
            {
                if (!inventoryModel.Add(obj: item.spell))
                {

                    // pickUpPanel.SetActive(true);
                }
                else
                {
                    MEventSystem.Instance.Send<AddSpell>(new AddSpell { spell = item.spell });
                }
                GameObjectPool.Instance.PushObject(item.gameObject);

            }
        }
        pickUpable = null;
        isPickingUp = false;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Wand"))
            return;
        if (!other.TryGetComponent<IPickUpable>(out var obj))
            return;
        pickUpable = obj;
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Wand"))
            return;
        if (!other.TryGetComponent<IPickUpable>(out var obj))
            return;
        pickUpable = obj;
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Wand"))
            return;
        if (!other.TryGetComponent<IPickUpable>(out var obj))
            return;
        pickUpable = null;
    }
    private void OnGUI()
    {
        if (Test)
        {

            GUIStyle style = new GUIStyle();
            style.fontSize = 25;
            style.normal.textColor = Color.black;
            GUI.Label(new Rect(10, 10, 200, 20), "Current Spell Index: " + currentWand.CurrentSpellIndex.ToString(), style);
            if (currentWand.CastSpell != null)
                GUI.Label(new Rect(10, 30, 200, 20), "Current Charge Time: " + currentWand.CastSpell.name.ToString(), style);
            GUI.Label(new Rect(10, 50, 200, 20), "Current UsedSpellCount: " + currentWand.UsedSpellCount.ToString(), style);
            GUI.Label(new Rect(10, 70, 200, 20), "Current NoNullSpellCount: " + currentWand.NonNullSpellCount.ToString(), style);
            GUI.Label(new Rect(10, 90, 200, 20), "Divide数量：" + ObjectPoolFactory.Instance.GetPoolCount<Divide>(), style);
            GUI.Label(new Rect(10, 110, 200, 20), "Formation数量：" + ObjectPoolFactory.Instance.GetPoolCount<Formation>(), style);
            GUI.Label(new Rect(10, 130, 200, 20), "MultiCast数量：" + ObjectPoolFactory.Instance.GetPoolCount<MultiCast>(), style);
            GUI.Label(new Rect(10, 150, 200, 20), "Spell数量：" + ObjectPoolFactory.Instance.GetPoolCount<Spell>(), style);
            GUI.Label(new Rect(10, 170, 200, 20), "DivideModifier数量：" + ObjectPoolFactory.Instance.GetPoolCount<DivideModifier>(), style);
        }

    }
}
