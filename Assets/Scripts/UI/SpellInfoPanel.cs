using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using System;
using Unity.Properties;
using System.Reflection;
public class SpellInfoPanel : MonoBehaviour
{
    [SerializeField] Image spellSprite;
    [SerializeField] List<Text> attributeNamesList;
    [SerializeField] List<Text> attributeValuesList;
    [SerializeField] RectTransform rectTransform;
    [SerializeField] GameObject attributeNames;
    [SerializeField] GameObject attributeValues;
    public void Init(Spell spell, PointerEventData eventData)
    {
        // attributeNamesList = attributeNames.transform.GetComponentsInChildren<Text>().ToList();
        // attributeValuesList = attributeValues.transform.GetComponentsInChildren<Text>().ToList();
        // var sr = spell.prefab.GetComponent<SpriteRenderer>();
        SetPosition(eventData.position);
        foreach (Text text in attributeValuesList)
        {
            text.gameObject.SetActive(false);
            attributeNamesList[attributeValuesList.IndexOf(text)].gameObject.SetActive(false);
        }
        spellSprite.sprite = spell.sprite;
        DisplayAttribute(() => spell.spellName, "spellName");
        DisplayAttribute(() => spell.info, "info");
        DisplayAttribute(() => spell.spellType, "spellType");
        DisplayAttribute(() => spell.magicCost, "magicCost");
        DisplayAttribute(() => spell.damage, "damage");
        DisplayAttribute(() => spell.burstRadius, "burstRadius");
        DisplayAttribute(() => spell.spread, "spread");
        DisplayAttribute(() => spell.speed, "speed");
        DisplayAttribute(() => spell.lifeTime, "lifeTime");
        DisplayAttribute(() => spell.castDelay, "castDelay");
        DisplayAttribute(() => spell.chargeTime, "chargeTime");
        DisplayAttribute(() => spell.spreadModifier, "spreadModifier");
        DisplayAttribute(() => spell.speedModifier, "speedModifier");
        DisplayAttribute(() => spell.lifeTimeModifier, "lifeTimeModifier");
        var count = attributeNames.GetComponentsInChildren<Text>().Length;
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, 600 / 14 * count + 100);
        attributeNames.GetComponent<RectTransform>().sizeDelta = new Vector2(attributeNames.GetComponent<RectTransform>().sizeDelta.x, 600 / 14 * count);
        attributeValues.GetComponent<RectTransform>().sizeDelta = new Vector2(attributeValues.GetComponent<RectTransform>().sizeDelta.x, 600 / 14 * count);
    }
    void DisplayAttribute<T>(Func<T> getValue, string attributeName)
    {

        foreach (Text text in attributeValuesList)
        {
            if (text.name == attributeName)
            {
                T value = getValue();
                if (value is float val && val <= 0 || value is int i && i <= 0)
                {
                    return;
                }
                if (value is SpellType spellType)
                {
                    text.text = spellType.GetDescription();
                }
                else
                {
                    text.text = value.ToString();
                }
                text.gameObject.SetActive(true);
                attributeNamesList[attributeValuesList.IndexOf(text)].gameObject.SetActive(true);
                return;
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
