using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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
                // if (e.wand != null)
                // {
                //     //TODO:丢弃时，要更换层级
                //     if (currentWand != null)
                //     {
                //         currentWand.gameObject.SetActive(false);
                //     }
                //     e.wand.gameObject.layer = LayerMask.NameToLayer("Wand");
                //     e.wand.transform.SetParent(transform);
                //     e.wand.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                //     inventoryModel.Add(e.wand);

                //     currentWand = e.wand;
                // }
            }
        ).UnRegisterWhenGameObjectDestroy(gameObject);
        MEventSystem.Instance.Register<ChangeCastWand>(
            e =>
            {
                if (e.index < 0 || e.index >= inventoryModel.wands.Count)
                {
                    currentWand = null;
                    return;
                }
                if (currentWand != null && currentWand != inventoryModel.wands[e.index])
                {
                    currentWand.gameObject.SetActive(false);
                }
                currentWand = inventoryModel.wands[e.index];
                if (currentWand != null)
                {
                    currentWand.gameObject.SetActive(true);
                }

            }
        ).UnRegisterWhenGameObjectDestroy(gameObject);
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
            //TODO:完善背包满时的提示
            // pickUpPanel.SetActive(true);
        }
        else
        {
            MEventSystem.Instance.Send<AddWand>();
            if (currentWand != null)
            {
                currentWand.gameObject.SetActive(false);
            }
            wand.gameObject.layer = LayerMask.NameToLayer("Wand");
            wand.transform.SetParent(transform);
            wand.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            currentWand = wand;

        }
    }
    public void PickUp()
    {
        if (pickUpable is null) return;
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
                    //TODO：添加法术
                    MEventSystem.Instance.Send<AddSpell>(new AddSpell { spell = item.spell });
                }
                GameObjectPool.Instance.PushObject(item.gameObject);

            }
        }
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
}
