using UnityEngine;
using UnityEngine.UI;

public class DropItem : MonoBehaviour, IPickUpable
{
    public Spell spell;
    [SerializeField] SpriteRenderer spriteRenderer;

    public bool CanPickUp(GameObject gameObject)
    {
        return gameObject.CompareTag("Player");
    }
    private void Awake()
    {

    }
    void Start()
    {
        int index = Random.Range(0, GameManger.Instance.spellCount);
        spell = GameManger.Instance.Get<Spell>(index);
        spriteRenderer.sprite = spell.sprite;
    }
}
