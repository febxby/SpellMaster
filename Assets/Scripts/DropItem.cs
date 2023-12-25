using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class DropItem : MonoBehaviour, IPickUpable
{
    public bool isRandom = true;
    public Spell spell;
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
    void Start()
    {
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
}
