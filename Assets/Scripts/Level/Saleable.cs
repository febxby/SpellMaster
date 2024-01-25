using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public interface ISaleable : IPickUpable
{
    int Sell(int price);
}
public class Saleable : MonoBehaviour, ISaleable, IShowable
{
    public Text text;
    public int price;
    public bool isRandom = true;
    public Spell spell;
    public GameObject infoPanel;
    public int Sell(int price)
    {
        if (price >= this.price)
            return price - this.price;
        return -1;
    }
    [SerializeField] SpriteRenderer spriteRenderer;

    public bool CanPickUp(GameObject gameObject)
    {
        return gameObject.CompareTag("Player");
    }
    private void Awake()
    {

    }
    public void Init(Spell spell)
    {
        if (spell == null)
        {
            int index = Random.Range(0, GameManger.Instance.spellCount);
            spell = GameManger.Instance.Get<Spell>(index);
        }
        else
        {
            this.spell = spell;
        }
        spriteRenderer.sprite = spell.sprite;
    }
    private void OnEnable()
    {
        if (isRandom)
        {
            int index = Random.Range(0, GameManger.Instance.spellCount);
            spell = GameManger.Instance.Get<Spell>(index);
        }
        spriteRenderer.sprite = spell.sprite;
    }
    void Start()
    {
        text.text = price.ToString();
        if (isRandom)
        {
            int index = Random.Range(0, GameManger.Instance.spellCount);
            spell = GameManger.Instance.Get<Spell>(index);
        }
        spriteRenderer.sprite = spell.sprite;
    }
    private void OnDisable()
    {
        GameObjectPool.Instance.PushObject(gameObject);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        MEventSystem.Instance.Send<ShowInfoPanel>(new ShowInfoPanel
        {
            showable = this,
            eventData = new PointerEventData(EventSystem.current)
            {
                // 使用当前对象的上方的位置
                position = RectTransformUtility.WorldToScreenPoint(Camera.main, transform.position + Vector3.up * 2)
            }
        });
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (infoPanel != null)
        {
            GameObjectPool.Instance.PushObject(infoPanel);
            infoPanel = null;
            // infoPanel.SetActive(false);
        }
    }
}
