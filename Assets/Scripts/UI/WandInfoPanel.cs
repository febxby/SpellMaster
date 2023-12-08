using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public interface IInfoPanel { }
public class WandInfoPanel : MonoBehaviour
{
    [SerializeField] Image wandSprite;
    [SerializeField] Text wandName;
    [SerializeField] Text drawCount;
    [SerializeField] Text castDelay;
    [SerializeField] Text chargeTime;
    [SerializeField] Text maxMagic;
    [SerializeField] Text magicRestoreRate;
    [SerializeField] Text capacity;
    [SerializeField] Text spread;
    [SerializeField] GameObject spells;
    [SerializeField] GameObject spellSlotPrefab;
    [SerializeField] RectTransform rectTransform;
    public void Init(Wand wand, PointerEventData eventData)
    {
        //TODO:优化，不用每次都生成，检测是否有子物体，有就直接SetActive(true)
        SetPosition(eventData.position);
        wandSprite.sprite = wand.spriteRenderer.sprite;
        wandSprite.color = wand.spriteRenderer.color;
        castDelay.text = wand.CastDelay.ToString();
        chargeTime.text = wand.ChargeTime.ToString();
        maxMagic.text = wand.MaxMagic.ToString();
        drawCount.text = wand.DrawCount.ToString();
        spread.text = wand.Spread.ToString();
        capacity.text = wand.Capacity.ToString();
        magicRestoreRate.text = wand.MagicRestoreRate.ToString();
        wandName.text = wand.WandName;
        for (int i = 0; i < wand.Capacity; i++)
        {
            // var obj = new GameObject("Spell", typeof(RectTransform), typeof(Image));
            GameObject obj = Instantiate(spellSlotPrefab, spells.transform);
            if (i < wand.Deck.Count)
                if (wand.Deck[i] != null)
                {
                    obj.GetComponentInChildren<SpellSlot>().Init(wand.Deck[i]);
                }
        }

    }
    public void SetPosition(Vector3 pos)
    {
        int tempPivotX = ((pos.x <= Screen.width / 2.0f) ? 0 : 1);
        int tempPivotY = ((pos.y <= Screen.height / 2.0f) ? 0 : 1);
        if (rectTransform.pivot.x != tempPivotX || rectTransform.pivot.y != tempPivotY)
        {
            rectTransform.pivot = new Vector2(tempPivotX, tempPivotY);
        }
        var temp = new Vector3(rectTransform.pivot.x == 0 ? 10 : -10, rectTransform.pivot.y == 0 ? 10 : -10, 0);
        transform.position = pos + temp;
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.activeSelf)
        {
            SetPosition(Input.mousePosition);
        }
    }
}
