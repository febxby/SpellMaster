using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public interface IPool
{
}
public interface IPoolable
{
}

public class ObjectPool<T> where T : class, new()
{
    private readonly Queue<T> objectPool = new();
    public int curCount => objectPool.Count;
    public T GetObject()
    {
        if (objectPool.Count == 0)
            //如果T是ScriptableObject，则使用ScriptableObject.CreateInstance创建实例
            if (typeof(T).IsSubclassOf(typeof(ScriptableObject)))
            {
                return ScriptableObject.CreateInstance(typeof(T)) as T;
            }
            else
            {
                return new T();
            }
        Debug.Log("出栈" + typeof(T) + GetType());
        return objectPool.Dequeue();
    }
    public void PushObject(T obj)
    {
        if (obj is null)
            return;
        if (obj is not T)
            return;
        if (objectPool.Contains(obj))
            return;
        Debug.Log("入栈" + typeof(T) + GetType());
        objectPool.Enqueue(obj);
    }
    public void ResetObject()
    {
        objectPool.Clear();
    }
}
