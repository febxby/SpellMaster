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
    [SerializeField] Transform wandParent;
    [SerializeField] Wand currentWand;
    [SerializeField] float speed = 5f;
    [SerializeField] float flySPeed = 5f;
    [SerializeField] float acceleration = 5f;
    [SerializeField] GameObject pickUpPanel;
    [SerializeField] IPickUpable pickUpable;
    // [SerializeField] float pickUpCooldown = 0.5f;
    [SerializeField] bool Test = false;
    Animator animator;
    SpriteRenderer spriteRenderer;
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
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
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
        if (inventoryModel != null)
            currentWand = inventoryModel.wands[0];
        else
            currentWand = wandParent.GetComponentInChildren<Wand>();

    }
    public void Cast(Vector2 pos)
    {
        if (currentWand == null || !currentWand.gameObject.activeSelf)
            return;
        currentWand.Cast(pos, tag);

    }
    // Update is called once per frame
    void Update()
    {
        if (currentWand != null)
        {
            currentWand.transform.right = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - (Vector2)currentWand.transform.position;
        }
        //spriteRenderer.flipX跟随鼠标翻转方向
        if (Camera.main.ScreenToWorldPoint(Input.mousePosition).x > transform.position.x)
        {
            spriteRenderer.flipX = false;
        }
        else
        {
            spriteRenderer.flipX = true;
        }
    }
    private void FixedUpdate()
    {
        Move();
        // Fly();
    }
    public void MoveInput(Vector2 direction)
    {

        moveDirection = direction;
        animator.SetBool("Run", true);
        // if (direction.y > 0)
        // {
        //     canFly = true;
        // }
        // else
        // {
        //     canFly = false;
        // }
    }
    public void FlyInput(bool value)
    {
        canFly = value;
    }
    public void Move()
    {
        if (moveDirection == Vector2.zero)
        {
            animator.SetBool("Run", false);
        }
        currentSpeed = Mathf.MoveTowards(currentSpeed, speed, Time.deltaTime * acceleration);
        rb.velocity = new Vector2(moveDirection.x * currentSpeed, moveDirection.y * currentSpeed);
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
                temp.transform.SetParent(wandParent);
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
            wand.transform.SetParent(wandParent);
            wand.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        }
    }
    public void PickUp()
    {
        if (TryGetComponent<ISaleable>(out var saleable))
        {
            var price = saleable.Sell(inventoryModel.Coin);
            if (price != -1)
            {
                inventoryModel.Coin = price;
                Debug.Log("卖出成功");
            }
            else
            {
                Debug.Log("卖出失败");
                return;
            }
        }
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
        if (other.gameObject.layer == LayerMask.NameToLayer("Coin"))
        {
            inventoryModel.Coin += 5;
            GameObjectPool.Instance.PushObject(other.gameObject);
        }
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
    // private void OnGUI()
    // {
    //     if (Test)
    //     {

    //         GUIStyle style = new GUIStyle();
    //         style.fontSize = 25;
    //         style.normal.textColor = Color.black;
    //         GUI.Label(new Rect(10, 10, 200, 20), "Current Spell Index: " + currentWand.CurrentSpellIndex.ToString(), style);
    //         if (currentWand.CastSpell != null)
    //             GUI.Label(new Rect(10, 30, 200, 20), "Current Charge Time: " + currentWand.CastSpell.name.ToString(), style);
    //         GUI.Label(new Rect(10, 50, 200, 20), "Current UsedSpellCount: " + currentWand.UsedSpellCount.ToString(), style);
    //         GUI.Label(new Rect(10, 70, 200, 20), "Current NoNullSpellCount: " + currentWand.NonNullSpellCount.ToString(), style);
    //         GUI.Label(new Rect(10, 90, 200, 20), "Divide数量：" + ObjectPoolFactory.Instance.GetPoolCount<Divide>(), style);
    //         GUI.Label(new Rect(10, 110, 200, 20), "Formation数量：" + ObjectPoolFactory.Instance.GetPoolCount<Formation>(), style);
    //         GUI.Label(new Rect(10, 130, 200, 20), "MultiCast数量：" + ObjectPoolFactory.Instance.GetPoolCount<MultiCast>(), style);
    //         GUI.Label(new Rect(10, 150, 200, 20), "Spell数量：" + ObjectPoolFactory.Instance.GetPoolCount<Spell>(), style);
    //         GUI.Label(new Rect(10, 170, 200, 20), "DivideModifier数量：" + ObjectPoolFactory.Instance.GetPoolCount<DivideModifier>(), style);
    //     }

    // }
}
