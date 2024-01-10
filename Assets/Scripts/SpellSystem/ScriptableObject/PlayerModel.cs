using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
[CreateAssetMenu(fileName = "New PlayerModel", menuName = "Model/PlayerModel"), Serializable]
public class PlayerModel : ScriptableObject
{
    [SerializeField] private int maxWandCount = 4;
    [SerializeField] private int maxSpellCount = 24;
    [SerializeField] private int coin = 0;
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;
    public PlayerModel(int maxWandCount, int maxSpellCount, int coin, int maxHealth,
    int currentHealth, List<Wand> wands, List<Spell> spells, PriorityQueue<int> nullSpellIndices, PriorityQueue<int> nullWandIndices)
    {
        this.maxWandCount = maxWandCount;
        this.maxSpellCount = maxSpellCount;
        this.coin = coin;
        this.maxHealth = maxHealth;
        this.currentHealth = currentHealth;
        this.wands = wands;
        this.spells = spells;
        this.nullSpellIndices = nullSpellIndices;
        this.nullWandIndices = nullWandIndices;
    }
    public int CurrentHealth
    {
        get
        {
            return currentHealth;
        }
        set
        {
            currentHealth = Mathf.Clamp(value, 0, maxHealth);
            MEventSystem.Instance.Send<HealthChange>(new HealthChange
            {
                value = (float)currentHealth / (float)maxHealth <= 0f ? 0f : (float)currentHealth / (float)maxHealth
            });
        }
    }
    public int MaxHealth
    {
        get
        {
            return maxHealth;
        }
        set
        {
            maxHealth = value;
        }
    }
    public int Coin
    {
        get
        {
            return coin;
        }
        set
        {
            coin = value;
            MEventSystem.Instance.Send<CoinChange>(new CoinChange
            {
                value = coin
            });
        }
    }
    public int wandCount => maxWandCount;
    public int spellCount => maxSpellCount;
    public List<Wand> wands;
    public List<Spell> spells;
    //存放空位索引的数组
    public PriorityQueue<int> nullSpellIndices;
    public PriorityQueue<int> nullWandIndices;
    // private void OnEnable()
    // {

    // }

    public void Init()
    {
        MEventSystem.Instance.Register<SaveData>(e =>
        {
            SaveData();
        });
        MEventSystem.Instance.Register<PlayerDeath>(e =>
        {
            Clear();
        });
        wands ??= new List<Wand>(maxWandCount);
        spells ??= new List<Spell>(maxSpellCount);
        nullSpellIndices = new PriorityQueue<int>();
        nullWandIndices = new PriorityQueue<int>();
        currentHealth = maxHealth;
        for (int i = 0; i < maxWandCount; i++)
        {
            if (i < wands.Count)
                if (wands[i] != null)
                    continue;
                else
                {
                    nullWandIndices.Enqueue(i);
                    continue;
                }
            wands.Add(null);
            nullWandIndices.Enqueue(i);
        }
        for (int i = 0; i < maxSpellCount; i++)
        {
            if (i < spells.Count)
                if (spells[i] != null)
                    continue;
                else
                {
                    nullSpellIndices.Enqueue(i);
                    continue;
                }
            spells.Add(null);
            nullSpellIndices.Enqueue(i);
        }

        // InitializeList(wands, nullWandIndices, maxWandCount);
        // InitializeList(spells, nullSpellIndices, maxSpellCount);
    }
    // private void InitializeList<T>(List<T> list, PriorityQueue<int> nullIndices, int maxCount)
    // {
    //     for (int i = 0; i < maxCount; i++)
    //     {
    //         if (i >= list.Count || list[i] == null)
    //         {
    //             if (i >= list.Count)
    //             {
    //                 list.Add(default(T));
    //             }
    //             nullIndices.Enqueue(i);
    //         }
    //     }
    // }
    public void SaveData()
    {
        SaveSystem.SaveByJson("Player", this);
        List<WandData> wandDatas = new List<WandData>();
        foreach (var wand in wands)
        {
            if (wand != null)
            {
                wandDatas.Add(wand.GetWandData());
            }
            else
            {
                wandDatas.Add(null);
            }
        }
        SaveSystem.SaveByJson("Wands", new SerializationList<WandData>(wandDatas));
    }
    // public void LoadData()
    // {
    //     PlayerModel data = SaveSystem.LoadFromJson<PlayerModel>("Player", this);
    //     List<WandData> gameObjects = SaveSystem.LoadListFromJson<WandData>("Wands");
    //     if (data != null || wands != null)
    //     {
    //         Coin = data.Coin;
    //         MaxHealth = data.MaxHealth;
    //         CurrentHealth = data.CurrentHealth;
    //         foreach (var wandData in gameObjects)
    //         {
    //             if (wandData != null)
    //             {
    //                 var wand = GameObjectPool.Instance.GetObject(wandData.prefab).GetComponent<Wand>();
    //                 wand.Init(wandData);
    //                 AddWand(wand, -1);
    //             }
    //             else
    //             {
    //                 AddWand(null, -1);
    //             }
    //         }
    //         spells = data.spells;
    //         nullSpellIndices = data.nullSpellIndices;
    //         nullWandIndices = data.nullWandIndices;
    //     }
    // }
    /// <summary>
    /// 往最近的一个空位添加
    /// </summary>
    /// <param name="obj"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns>是否有空位</returns>
    public bool Add<T>(T obj)
    {
        var type = typeof(T);
        if (type == typeof(Wand))
        {
            return AddWand(obj as Wand, -1);
        }
        else if (type == typeof(Spell))
        {
            return AddSpell(obj as Spell, -1);
        }
        return false;
    }
    /// <summary>
    /// 往索引处添加
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="index">索引为-1往最近的空位添加</param>
    /// <typeparam name="T"></typeparam>
    /// <returns>是否有空位</returns>
    public bool Add<T>(T obj, int index)
    {
        var type = typeof(T);
        if (type == typeof(Wand))
        {
            return AddWand(obj as Wand, index);
        }
        else if (type == typeof(Spell))
        {
            return AddSpell(obj as Spell, index);
        }
        return false;
    }
    public void Remove<T>(int index)
    {
        var type = typeof(T);
        if (type == typeof(Wand))
        {
            RemoveWand(index);
        }
        else if (type == typeof(Spell))
        {
            RemoveSpell(index);
        }
    }

    public bool AddWand(Wand wand, int index)
    {
        if (index >= maxWandCount || index < -1)
        {
            return false;
        }
        if (index == -1)
        {
            bool result = nullWandIndices.TryDequeue(out index);
            if (!result)
            {
                return false;
            }
        }
        if (wand == null)
            nullSpellIndices.Enqueue(index);
        wands[index] = wand;
        return true;
    }

    public bool AddSpell(Spell spell, int index)
    {
        if (index >= maxSpellCount || index < -1)
        {
            return false;
        }
        if (index == -1)
        {
            bool result = nullSpellIndices.TryDequeue(out index);
            if (!result)
            {
                return false;
            }
        }
        if (spell == null)
            nullSpellIndices.Enqueue(index);
        spells[index] = spell;
        return true;
    }
    public void RemoveSpell(int index)
    {
        nullSpellIndices.Enqueue(index);
        spells[index] = null;
    }

    public void RemoveWand(int index)
    {
        nullWandIndices.Enqueue(index);
        wands[index] = null;
    }
    //Get方法，根据索引返回Wand或Spell
    public Spell GetSpell(int index)
    {
        if (index >= maxSpellCount || index < 0)
            return null;
        return spells[index];
    }
    public Wand GetWand(int index)
    {
        if (index >= maxWandCount || index < 0)
            return null;
        return wands[index];
    }

    public void Clear()
    {
        wands.Clear();
        spells.Clear();
        nullSpellIndices.Clear();
        nullWandIndices.Clear();
        coin = 0;
    }

    private void OnDisable()
    {
        // wands.Clear();
        // spells.Clear();
    }
    private void OnDestroy()
    {
        //TODO：存档
        // wands.Clear();
        // spells.Clear();

    }
}
