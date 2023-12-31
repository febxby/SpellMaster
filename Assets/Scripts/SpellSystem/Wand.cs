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
    public int damage;
    public float speed;
    public float spread;
    public float gravity;
    public int bounce;
    public Modify(float castDelay = 0, int damage = 0, float speed = 1, float spread = 0, float gravity = 0, int bounce = 0)
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
            usedSpellCount = 0;
            deck[index] = value;
            MEventSystem.Instance.Send<UpdateCurrentWandPanel>(new UpdateCurrentWandPanel
            {
                wand = this,
                isChange = false
            });
            // LoadSpell();
        }
    }
    public List<Spell> Deck => deck;
    public string WandName => wandName;
    public int DrawCount => drawCount;
    public float CastDelay => castDelay;
    public float MaxMagic => maxMagic;
    public float MagicRestoreRate => magicRestoreRate;
    public float ChargeTime => chargeTime;
    public float Spread => spread;
    public int Capacity => capacity;

    public SpriteRenderer spriteRenderer;
    public int CurrentSpellIndex => currentSpellIndex;
    public Spell CastSpell => castSpell;
    public int UsedSpellCount => usedSpellCount;
    public int NonNullSpellCount => nonNullSpellCount;
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
    float currentMagic;
    int length;
    Spell castSpell;
    BoxCollider2D boxCollider2D;
    Rigidbody2D rb;
    int usedSpellCount = 0;
    int nonNullSpellCount = 0;
    string owner;
    string uniqueId;
    // Queue<Spell> spellQueue = new Queue<Spell>();
    private void Awake()
    {
        currentMagic = maxMagic;
        boxCollider2D = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        modify = new Modify(castDelay, 1, 1, spread);
        defaultModify = modify;
        childModify = modify;
        spellDict = new Dictionary<string, Spell>();
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
        // LoadSpell();
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
    // void LoadSpell()
    // {
    //     while (usedSpellCount + 1 <= nonNullSpellCount)
    //     {
    //         castSpell = Draw(ref modify);
    //         spellQueue.Enqueue(castSpell);
    //     }
    // }
    public void Cast(Vector2 pos, string owner)
    {
        this.owner = owner;
        if (deck.Count == 0)
        {
            return;
        }
        //如果法术充能未完成且正在充能则返回
        if (isCharging)
        {
            return;
        }
        if (Time.time - lastCastTime < currentCastDelay)
        {
            return;
        }
        lastCastTime = Time.time;
        currentCastDelay = 0;
        for (int i = 0; i < drawCount; i++)
        {
            castSpell = Draw(ref modify);
            // castSpell = spellQueue.Dequeue();
        }
        if (castSpell != null)
        {
            //修改法术属性
            Modify(castSpell, ref modify);
            uniqueId = System.Guid.NewGuid().ToString();
            castSpell.Cast(castPoint.position, transform.right * 10, transform.right, owner, uniqueId);
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
        // spellQueue.Enqueue(castSpell);
        //重置修正属性
        modify = defaultModify;
        childModify = defaultModify;
    }
    public void EnterCharge()
    {
        if (usedSpellCount >= nonNullSpellCount && !isCharging)
        {
            ResetDiscard();
            currentChargeTime += chargeTime;
            currentSpellIndex = 0;
            lastChargeTime = Time.time;
            isCharging = true;
            usedSpellCount = 0; // 重置计数器的值
        }
    }
    public void PreLoad(Spell spell, ref Modify modify)
    {
        modify.damage += spell.damage;
        modify.speed *= (spell.speedModifier == 0 ? 1 : spell.speedModifier);
        modify.spread += spell.spreadModifier;
        modify.castDelay += spell.castDelay;
        // modify.gravity += spell.gravity;
        modify.bounce += spell.bounce;
        currentCastDelay += spell.castDelay;
        currentChargeTime += spell.chargeTime;
        for (int i = 0; i < spell.drawCount; i++)
        {
            //如果当前索引超过持有法术数量
            if (usedSpellCount + 1 > nonNullSpellCount)
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
        //法术消耗大于当前魔力值，跳过
        if (deck[currentSpellIndex].magicCost > currentMagic)
        {
            currentSpellIndex++;
            usedSpellCount++;
            return Draw(ref modify);
        }
        currentMagic -= deck[currentSpellIndex].magicCost;
        // Spell currentSpell = Instantiate(deck[currentSpellIndex]);
        Spell currentSpell = ObjectPoolFactory.Instance.Get(deck[currentSpellIndex].GetType());
        currentSpell.Copy(deck[currentSpellIndex]);
        // Spell currentSpell = ObjectPool<Spell>.Instance.GetObject(deck[currentSpellIndex].GetType());
        // currentSpell.Copy(deck[currentSpellIndex]);
        currentSpellIndex++;
        usedSpellCount++;
        if (currentSpell.isTrigger)
        {
            PreLoad(currentSpell, ref childModify);
        }
        else
        {
            PreLoad(currentSpell, ref modify);
        }
        // ObjectPoolFactory.Instance.Push(currentSpell.GetType(), currentSpell);
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
                spell.damage += modify.damage;
                spell.speed *= modify.speed;
                spell.spread += Math.Clamp(spell.spread + modify.spread, 0, float.MaxValue);
                // spell.gravity += modify.gravity;
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
        if (transform.parent != null)
            if (transform.parent.CompareTag("Player"))
            {
                MEventSystem.Instance.Send(new MagicChange { value = 1 });
                MEventSystem.Instance.Send(new ChargeChange { value = 1 });
            }
    }

    // Update is called once per frame
    void Update()
    {

        if (currentMagic < maxMagic)
        {
            currentMagic += magicRestoreRate * Time.deltaTime;
            MEventSystem.Instance.Send<MagicChange>(new MagicChange
            {
                value = currentMagic / maxMagic
            });
        }
        else
        {
            currentMagic = maxMagic;
            MEventSystem.Instance.Send<MagicChange>(new MagicChange
            {
                value = currentMagic / maxMagic
            });
        }
        if (transform.parent != null)
            if (transform.parent.CompareTag("Player"))
            {
                gameObject.layer = LayerMask.NameToLayer("Wand");
                rb.isKinematic = true;
                rb.velocity = Vector2.zero;
                rb.constraints = RigidbodyConstraints2D.FreezeAll;

            }
            else
            {
                gameObject.layer = LayerMask.NameToLayer("PickUpable");
                // rb.isKinematic = false;
                // rb.constraints = RigidbodyConstraints2D.FreezeRotation;
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
                if (owner == "Player")
                    MEventSystem.Instance.Send<ChargeChange>(new ChargeChange
                    {
                        value = currentChargeProgress / currentChargeTime < 0 ? 0 : currentChargeProgress / currentChargeTime
                    });
            }
            else
            {
                if (owner == "Player")
                    MEventSystem.Instance.Send<ChargeChange>(new ChargeChange
                    {
                        value = 1
                    });
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
