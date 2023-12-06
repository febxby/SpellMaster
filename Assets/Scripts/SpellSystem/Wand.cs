using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
[Serializable]
public struct Modify
{
    public float castDelay;
    public float damage;
    public float speed;
    public float spread;
    public float gravity;
    public int bounce;
    public Modify(float castDelay = 0, float damage = 0, float speed = 1, float spread = 0, float gravity = 0, int bounce = 0)
    {
        this.castDelay = castDelay;
        this.damage = damage;
        this.speed = speed;
        this.spread = spread;
        this.gravity = gravity;
        this.bounce = bounce;
    }
}
[Serializable]
public class Wand : MonoBehaviour, IPickUpable
{
    // Start is called before the first frame update
    [SerializeField] SpellConfigs spellConfigs;
    Dictionary<string, Spell> spellDict;
    [SerializeField] string wandName;
    [SerializeField] float castDelay;
    [SerializeField] float maxMagic;
    [SerializeField] float magicRestoreRate;
    [SerializeField] float chargeTime;
    [SerializeField] float spread;
    [SerializeField] int capacity;
    [SerializeField] int drawCount;
    [SerializeField] Transform castPoint;
    public UnityEvent<float> chargeEvent;

    [SerializeField] List<Spell> deck;
    public Spell this[int index]
    {
        get { return deck[index]; }
        set
        {
            if (deck[index] != null)
            {
                nonNullSpellCount--;
            }
            if (value != null)
            {
                nonNullSpellCount++;
            }
            currentSpellIndex = 0;
            deck[index] = value;
        }
    }
    public List<Spell> mDeck => deck;
    public string mWandName => wandName;
    public int mDrawCount => drawCount;
    public float mCastDelay => castDelay;
    public float mMaxMagic => maxMagic;
    public float mMagicRestoreRate => magicRestoreRate;
    public float mChargeTime => chargeTime;
    public float mSpread => spread;
    public int mCapacity => capacity;

    public SpriteRenderer spriteRenderer;
    public int CurrentSpellIndex => currentSpellIndex;
    public Spell CastSpell => castSpell;
    bool[] hand;
    bool[] discard;
    Modify modify;
    Modify childModify;
    Modify defaultModify;
    int currentSpellIndex = 0;
    float lastCastTime;
    float lastChargeTime;
    public bool isCharging = false;
    float currentCastDelay;
    float currentChargeTime;
    float currentChargeProgress => Time.time - lastChargeTime;
    int length;
    Spell castSpell;
    BoxCollider2D boxCollider2D;
    Rigidbody2D rb;
    int usedSpellCount = 0;
    int nonNullSpellCount = 0;
    private void Awake()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        modify = new Modify(castDelay, 1, 1, spread);
        defaultModify = modify;
        childModify = modify;
        spellDict = new Dictionary<string, Spell>();
        foreach (Spell spell in spellConfigs.configs)
        {
            spellDict.Add(spell.spellName, spell);
        }
        // deck ??= new List<Spell>(capacity);
        for (int i = 0; i < capacity; i++)
        {
            if (i < deck.Count)
                continue;
            deck.Add(null);
        }
        discard = new bool[deck.Count];
        hand = new bool[deck.Count];
        spriteRenderer = GetComponent<SpriteRenderer>();
        //初始化非空法术
        for (int i = 0; i < deck.Count; i++)
        {
            if (deck[i] != null)
            {
                nonNullSpellCount++;
            }
        }
    }
    public bool AddSpell(Spell spell)
    {
        if (deck.Count >= capacity)
        {
            return false;
        }
        deck.Add(spell);
        return true;
    }
    public void Cast(Vector2 pos)
    {
        if (deck.Count == 0)
        {
            return;
        }
        //如果法术充能未完成且正在充能则返回
        if (isCharging)
        {
            // Debug.Log("正在充能");
            return;
        }
        // isCharging = false;

        if (Time.time - lastCastTime < currentCastDelay)
        {
            return;
        }
        lastCastTime = Time.time;
        currentCastDelay = 0;
        // deck[currentSpellIndex].Cast();
        for (int i = 0; i < drawCount; i++)
        {
            castSpell = Draw(ref modify);
        }
        if (castSpell != null)
        {
            //修改法术属性
            Modify(castSpell, ref modify);
            castSpell.Cast(castPoint.position, pos, (pos - (Vector2)castPoint.position).normalized, this.tag);
        }

        //每次施法后设置施法延迟
        currentCastDelay += castDelay;
        currentCastDelay = Mathf.Clamp(currentCastDelay, 0, float.MaxValue);
        //将手牌区的法术放入弃牌区
        for (int i = 0; i < hand.Length; i++)
        {
            if (!hand[i])
                continue;
            discard[i] = true;
            hand[i] = false;
        }
        //如果所有法术已使用并且法术未在充能则进行充能
        EnterCharge();

        //重置修正属性
        modify = defaultModify;
        childModify = defaultModify;
        // hand.Add(spell);
        // Debug.Log("Casting");
    }
    public void EnterCharge()
    {
        if (usedSpellCount == nonNullSpellCount && !isCharging)
        {
            for (int i = 0; i < discard.Length; i++)
            {
                discard[i] = false;
            }
            currentChargeTime += chargeTime;
            currentSpellIndex = 0;
            lastChargeTime = Time.time;
            isCharging = true;
            usedSpellCount = 0; // 重置计数器的值
        }
    }
    public void PreLoad(Spell spell, ref Modify modify)
    {

        // modify.damage *= spell.damage;

        modify.damage += spell.damage;
        modify.speed *= (spell.speedModifier == 0 ? 1 : spell.speedModifier);
        modify.spread += spell.spreadModifier;
        modify.castDelay += spell.castDelay;
        modify.gravity += spell.gravity;
        modify.bounce += spell.bounce;
        currentCastDelay += spell.castDelay;
        currentChargeTime += spell.chargeTime;
        for (int i = 0; i < spell.drawCount; i++)
        {
            //如果当前索引超过持有法术数量
            if (usedSpellCount + 1 >= nonNullSpellCount)
            {
                //判断弃牌区是否有法术
                if (!IsDiscardHasSpell())
                    return;
                //将弃牌区的法术放入牌库
                ResetDiscard();
                currentSpellIndex = 0;
            }
            Spell spell1 = Draw(ref modify);
            if (spell1 == null)
                return;
            spell.spells.Add(spell1);
        }
    }

    public Spell Draw(ref Modify modify)
    {
        // Debug.Log(deck.Count);
        //如果当前索引超过持有法术数量
        if (currentSpellIndex >= deck.Count)
        {
            return null;
        }
        // 如果当前法术在手牌区，跳过
        if (hand[currentSpellIndex])
        {
            currentSpellIndex++;
            return Draw(ref modify);
        }
        //放入手牌区
        hand[currentSpellIndex] = true;
        if (deck[currentSpellIndex] == null)
        {
            currentSpellIndex++;
            if (currentSpellIndex >= deck.Count)
            {
                return null;
            }
            return Draw(ref modify);
        }
        // Spell currentSpell = Instantiate(deck[currentSpellIndex]);
        Spell currentSpell = ObjectPoolFactory.Instance.Get(Type.GetType(deck[currentSpellIndex].GetType().ToString()));
        currentSpell.Copy(deck[currentSpellIndex]);
        // Spell currentSpell = ObjectPool<Spell>.Instance.GetObject(deck[currentSpellIndex].GetType());
        // currentSpell.Copy(deck[currentSpellIndex]);
        currentSpellIndex++;
        if (currentSpell.isTrigger)
        {
            PreLoad(currentSpell, ref childModify);
        }
        else
        {
            PreLoad(currentSpell, ref modify);
        }
        usedSpellCount++;
        return currentSpell;
    }
    public void Modify(Spell spell, ref Modify modify)
    {
        switch (spell.spellType)
        {
            case SpellType.Projectile:
                for (int i = 0; i < spell.spells.Count; i++)
                {
                    if (spell.isTrigger)
                        Modify(spell.spells[i], ref childModify);
                    else
                        Modify(spell.spells[i], ref modify);
                }
                spell.damage *= modify.damage;
                spell.speed *= modify.speed;
                // UnityEngine.Random.InitState((int)Time.time);
                // float angle = UnityEngine.Random.Range(-modify.spread, modify.spread);
                // spell.spread += angle;
                spell.spread += modify.spread;
                spell.spread = Mathf.Clamp(spell.spread, 0, float.MaxValue);
                spell.gravity += modify.gravity;
                spell.bounce += modify.bounce;
                break;
            case SpellType.Modifier:
            case SpellType.Multicast:
                for (int i = 0; i < spell.spells.Count; i++)
                {
                    Modify(spell.spells[i], ref modify);
                }
                break;
        }

    }
    public void CheckNullVal()
    {
        for (int i = 0; i < deck.Count; i++)
        {
            if (deck[i] == null)
            {
                discard[i] = true;
            }
        }
    }
    public void ResetList()
    {
        discard = new bool[deck.Count];
        hand = new bool[deck.Count];
    }
    public void AddSpell(string name)
    {
        deck.Add(spellDict[name]);
    }
    //TODO：扩充法术栏
    public void AddSpell(Spell spell, int index)
    {
        deck[index] = spell;
    }
    public bool IsDiscardHasSpell()
    {
        for (int i = 0; i < discard.Length; i++)
        {
            if (discard[i])
                return true;
        }
        return false;
    }
    public bool GetHandStatus()
    {
        if (Array.TrueForAll(hand, x => x == true))
            return true;
        else
            return false;
    }

    //将弃牌区的法术放入牌库
    public void ResetDiscard()
    {
        for (int i = 0; i < discard.Length; i++)
        {
            if (deck[i] != null)
                discard[i] = false;
        }
    }
    public void GetChargeStatus()
    {
        if (Array.TrueForAll(discard, x => x == true) && !isCharging)
        {
            isCharging = true;
        }
    }

    void Start()
    {
        length = deck.Count;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.parent != null)
        {
            rb.isKinematic = true;
            rb.velocity = Vector2.zero;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }
        else
        {
            rb.isKinematic = false;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
        //牌库数量发生变化时重置牌库
        if (deck.Count != length)
        {
            ResetList();
            length = deck.Count;
            currentSpellIndex = 0;
        }
        if (isCharging)
        {
            if (currentChargeProgress < currentChargeTime)
            {
                chargeEvent?.Invoke(currentChargeProgress / currentChargeTime);
            }
            else
            {
                chargeEvent?.Invoke(0);
                isCharging = false;
                currentChargeTime = 0;
            }
        }
        //如果所有法术已使用并且法术未在充能则进行充能
        // if (Array.TrueForAll(discard, x => x == true) && !isCharging)
        // {
        //     for (int i = 0; i < discard.Length; i++)
        //     {
        //         discard[i] = false;
        //     }
        //     currentChargeTime += chargeTime;
        //     currentSpellIndex = 0;
        //     lastChargeTime = Time.time;
        //     isCharging = true;
        // }
    }

    public bool CanPickUp(GameObject gameObject)
    {
        return gameObject.CompareTag("Player") || gameObject.CompareTag("Enemy");
    }

}
