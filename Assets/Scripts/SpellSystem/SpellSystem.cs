using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpellSystem : MonoBehaviour
{
    private static SpellSystem instance;
    public static SpellSystem Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SpellSystem>();
            }
            return instance;
        }
    }
    [SerializeField] GameObject spellList;
    public void CastSpell(Spell spell, Vector2 start, Vector2 end)
    {
        GameObject spellObj = Instantiate(spell.prefab, start, Quaternion.identity);
        spellObj.GetComponent<Rigidbody2D>().AddForce(spell.speed * (start - end).normalized, ForceMode2D.Impulse);
    }
    // Start is called before the first frame update
    public void OpenEditor()
    {
        spellList.SetActive(true);
    }
    public void CloseEditor()
    {
        spellList.SetActive(false);
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
