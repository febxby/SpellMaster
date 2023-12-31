using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageText : MonoBehaviour
{
    // Start is called before the first frame update
    public Text text;
    public Canvas canvas;
    private void Awake()
    {
        text = GetComponent<Text>();
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
    }
    public DamageText Init(float value)
    {
        text.text = value.ToString();
        transform.SetParent(canvas.transform);
        transform.localScale = new Vector3(0, 0, 1);
        transform.DOScale(new Vector3(1, 1, 0), 0.4f);
        transform.DOMoveY(transform.position.y + 10, 0.5f);
        StartCoroutine(nameof(DestroyText));
        return this;
    }
    // void OnEnable()
    // {
    //     transform.localScale = new Vector3(0, 0, 1);
    //     transform.DOScale(new Vector3(1, 1, 0), 0.4f);
    //     transform.DOMoveY(transform.position.y + 10, 0.5f);
    //     StartCoroutine(nameof(DestroyText));
    // }
    IEnumerator DestroyText()
    {
        yield return new WaitForSeconds(0.5f);
        GameObjectPool.Instance.PushObject(gameObject);
    }


}
