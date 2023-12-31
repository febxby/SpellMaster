using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class DropItem : MonoBehaviour, IPickUpable, IShowable
{
    public bool isRandom = false;
    public Spell spell;
    [SerializeField] SpriteRenderer spriteRenderer;
    public GameObject infoPanel;


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
        if (isRandom && spell == null)
        {
            int index = Random.Range(0, GameManger.Instance.spellCount);
            spell = GameManger.Instance.Get<Spell>(index);
            spriteRenderer.sprite = spell.sprite;
        }
    }
    // void Start()
    // {
    //     if (isRandom)
    //     {
    //         int index = Random.Range(0, GameManger.Instance.spellCount);
    //         spell = GameManger.Instance.Get<Spell>(index);
    //     }
    //     spriteRenderer.sprite = spell.sprite;
    // }
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
