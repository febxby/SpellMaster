using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DamageText : MonoBehaviour
{
    // Start is called before the first frame update
    public TextMeshPro text;
    public float duration = 1f;
    float timer = 0;
    UnityAction action;
    private void Awake()
    {
        text = GetComponent<TextMeshPro>();
    }
    public DamageText Init(float value, UnityAction action = null)
    {
        //TODO:优化伤害数字
        timer = Time.time;
        this.action = action;
        text.text = value.ToString();
        // transform.localScale = new Vector3(1, 1, 1);
        // transform.DOScale(new Vector3(2, 2, 2), 0.4f);
        transform.DOMoveY(transform.position.y + 1, 2f);
        return this;
    }
    private void OnEnable()
    {
        timer = Time.time;
    }
    public void UpdateDamage(int value)
    {
        int currentDamage = int.Parse(text.text);
        text.text = (currentDamage + value).ToString();
        timer = Time.time;
    }
    private void Update()
    {
        if (Time.time - timer > duration)
        {
            action?.Invoke();
            GameObjectPool.Instance.PushObject(gameObject);
        }
    }
    // void OnEnable()
    // {
    //     transform.localScale = new Vector3(0, 0, 1);
    //     transform.DOScale(new Vector3(1, 1, 0), 0.4f);
    //     transform.DOMoveY(transform.position.y + 10, 0.5f);
    //     StartCoroutine(nameof(DestroyText));
    // }


}
