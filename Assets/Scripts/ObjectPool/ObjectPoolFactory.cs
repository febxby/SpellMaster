using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolFactory : Singleton<ObjectPoolFactory>
{
    private readonly Dictionary<Type, object> objectPool = new();
    /// <summary>
    /// 通过泛型创建对象池
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    private ObjectPool<T> GetPool<T>() where T : class, new()
    {
        if (!objectPool.ContainsKey(typeof(T)))
            objectPool.Add(typeof(T), new ObjectPool<T>());
        return objectPool[typeof(T)] as ObjectPool<T>;
    }
    /// <summary>
    /// 通过Type用反射创建泛型对象池
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private dynamic GetPool(Type type)
    {
        if (!objectPool.ContainsKey(type))
        {
            Type pool = typeof(ObjectPool<>).MakeGenericType(type);
            objectPool.Add(type, Activator.CreateInstance(pool));
        }
        return objectPool[type];
    }
    public T Get<T>() where T : class, new()
    {
        return GetPool<T>().GetObject();
    }
    public dynamic Get(Type type)
    {
        return GetPool(type).GetObject();
    }
    public void Push(Type type, dynamic obj)
    {
        GetPool(type).PushObject(obj);
    }
    public void Push<T>(T obj) where T : class, new()
    {
        GetPool<T>().PushObject(obj);
    }

}
