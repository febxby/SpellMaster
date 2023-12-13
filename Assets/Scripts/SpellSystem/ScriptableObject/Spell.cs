using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.InteropServices;
using System.Reflection;
public enum SpellType
{
    [Description("投掷物")]
    Projectile,
    [Description("修正")]
    Modifier,
    [Description("修正")]
    Multicast
}
public enum ScriptType
{
    Formation,
    MultiCast,
    Divide
}
public class DescriptionAttribute : Attribute
{
    public string Description { get; set; }

    public DescriptionAttribute(string description)
    {
        Description = description;
    }
}
public static class EnumExtensions
{
    public static string GetDescription(this Enum value)
    {
        FieldInfo field = value.GetType().GetField(value.ToString());
        DescriptionAttribute attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
        return attribute == null ? value.ToString() : attribute.Description;
    }
}
[CreateAssetMenu(fileName = "New Spell", menuName = "SpellSystem/Spell"), Serializable]
public class Spell : ScriptableObject
{
    [Header("基础属性")]
    public string spellName;
    public string info;
    [PnShowSprite] public Sprite sprite;
    public SpellType spellType;
    public float magicCost;
    public float burstRadius;
    public int damage;
    public float speed;
    public float spread;
    public float lifeTime;
    public float gravity;
    public int bounce;
    public int drawCount;
    public bool isTrigger;
    public bool canHurtSelf;
    public string owner;
    [Header("修正属性")]
    public float castDelay;
    public float chargeTime;
    public float speedModifier = 1;
    public float spreadModifier = 0;
    public float lifeTimeModifier = 0;
    [Header("脚本类型")]
    [PnShowSprite] public GameObject prefab;
    public ScriptType scriptType;
    public List<ProjectileComponent> casts = new();
    public List<GameObject> attaches = new();
    [HideInInspector] public List<Spell> spells;
    private Type mType => Type.GetType(scriptType.ToString());

    public float LifeTime => lifeTime / 60;
    // public Dictionary<Type,ICast> castDict = new();
    protected virtual void Awake()
    {
        spellName = name;
    }
    // private void OnValidate()
    // {
    //     if (sprite == null)
    //         if (prefab)
    //             if (prefab.GetComponent<SpriteRenderer>())
    //                 sprite = prefab.GetComponent<SpriteRenderer>().sprite;
    // }
    /// <summary>
    /// 施法法术
    /// </summary>
    /// <param name="start">法术生成位置</param>
    /// <param name="end">发射目标位置</param>
    /// <param name="direction">方向</param>
    /// <param name="owner">发射者</param>
    public virtual void Cast(Vector2 start, Vector2 end, Vector2 direction, string owner)
    {
        // Debug.Log(spellName+"-"+casts.Count);
        this.owner = owner;
        if (prefab)
        {
            prefab.GetComponent<ICast>().Cast(start, end, direction, this);
        }
        else
        {
            dynamic spell = ObjectPoolFactory.Instance.Get(mType);
            spell?.Cast(start, end, direction, this);
        }
    }

    public Spell Copy(Spell spell)
    {
        JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(spell), this);
        return this;
    }
}
