using System;
using System.Collections.Generic;
using UnityEngine;

public class IOCContainer : Singleton<IOCContainer>
{
    private readonly Dictionary<Type, object> container = new();

    public void Register<T>(T obj)
    {
        var key = typeof(T);
        if (container.ContainsKey(key))
        {
            container[key] = obj;
            return;
        }
        container.Add(key, obj);
    }

    public T Get<T>()
    {
        var key = typeof(T);
        if (!container.ContainsKey(key))
        {
            return default;
        }
        return (T)container[key];
    }
}
