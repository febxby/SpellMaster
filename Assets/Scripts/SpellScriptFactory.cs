using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellScriptFactory : Singleton<SpellScriptFactory>
{
    // private readonly Dictionary<ScriptType, object> objectPool = new();


    // private object GetPool(ScriptType type)
    // {
    //     switch (type)
    //     {
    //         case ScriptType.Formation:
    //             if (!objectPool.ContainsKey(ScriptType.Formation))
    //                 objectPool.Add(ScriptType.Formation, new ObjectPool<Formation>());
    //             return objectPool[ScriptType.Formation];
    //         case ScriptType.MultiCast:
    //             if (!objectPool.ContainsKey(ScriptType.MultiCast))
    //                 objectPool.Add(ScriptType.MultiCast, new ObjectPool<MultiCast>());
    //             return objectPool[ScriptType.MultiCast];
    //         case ScriptType.Divide:
    //             if (!objectPool.ContainsKey(ScriptType.Divide))
    //                 objectPool.Add(ScriptType.Divide, new ObjectPool<Divide>());
    //             return objectPool[ScriptType.Divide];
    //         default:
    //             return null;
    //     }
    // }
    // public ICast Get(ScriptType type)
    // {
    //     var pool = GetPool(type);
    //     switch (type)
    //     {
    //         case ScriptType.Formation:
    //             return (pool as ObjectPool<Formation>).GetObject();
    //         case ScriptType.MultiCast:
    //             return (pool as ObjectPool<MultiCast>).GetObject();
    //         case ScriptType.Divide:
    //             return (pool as ObjectPool<Divide>).GetObject();
    //         default:
    //             return null;
    //     }
    // }
    // public void Push(MCast cast, ScriptType type)
    // {
    //     var pool = GetPool(type);
    //     switch (type)
    //     {
    //         case ScriptType.Formation:
    //             (pool as ObjectPool<Formation>).PushObject(cast as Formation);
    //             break;
    //         case ScriptType.MultiCast:
    //             (pool as ObjectPool<MultiCast>).PushObject(cast as MultiCast);
    //             break;
    //         case ScriptType.Divide:
    //             (pool as ObjectPool<Divide>).PushObject(cast as Divide);
    //             break;
    //         default:
    //             break;
    //     }
    // }
    public ICast GetScript(ScriptType type)
    {
        return type switch
        {
            ScriptType.Formation => new Formation(),
            ScriptType.MultiCast => new MultiCast(),
            ScriptType.Divide => new Divide(),
            _ => null,
        };
    }

}
