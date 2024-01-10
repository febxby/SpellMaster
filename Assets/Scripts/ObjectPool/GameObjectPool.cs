using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public static class PoolObjectExtension
{
    public static GameObject SetPositionAndRotation(this GameObject poolObject, Vector3 position, Quaternion rotation)
    {
        // if (poolObject is Component component)
        poolObject.transform.SetPositionAndRotation(position, rotation);
        return poolObject;
    }
    public static GameObject SetParent(this GameObject poolObject, Transform parent)
    {
        poolObject.transform.SetParent(parent);
        return poolObject;
    }
    public static GameObject DisableWhenGameObjectDisable(this GameObject source, GameObject gameObject)
    {
        var trigger = gameObject.GetComponent<DisableOnDisableTrigger>();
        if (!trigger)
        {
            trigger = gameObject.AddComponent<DisableOnDisableTrigger>();
        }
        trigger.AddGameObject(source);
        return source;
    }
}
public class DisableOnDisableTrigger : MonoBehaviour
{
    private readonly List<GameObject> mDisable = new();
    public void AddGameObject(GameObject obj)
    {
        mDisable.Add(obj);
    }
    public void RemoveGameObject(GameObject obj)
    {
        mDisable.Remove(obj);
    }
    private void OnDisable()
    {
        foreach (var obj in mDisable)
        {
            GameObjectPool.Instance.PushObject(obj);
        }
        mDisable.Clear();
    }
}
public class GameObjectPool : Singleton<GameObjectPool>
{
    private readonly Dictionary<string, Queue<GameObject>> objectPool = new();
    private readonly List<GameObject> activePoolObjects = new();
    private GameObject pool;
    private WaitForSecondsRealtime waitForSecondsRealtime = new(0f);
    public GameObject GetObject(GameObject prefab, bool recycle = false)
    {
        GameObject _obj;
        if (pool == null || !GameObject.Find("ObjectPool"))
        {
            pool = new GameObject("ObjectPool");
            GameObject.DontDestroyOnLoad(pool);
        }
        GameObject child = GameObject.Find(prefab.name);
        if (!child)
        {
            child = new GameObject(prefab.name);
            child.transform.SetParent(pool.transform);
        }
        if (objectPool.TryGetValue(prefab.name, out Queue<GameObject> queue) && queue.Count > 0)
        {
            _obj = queue.Dequeue();
            if (recycle)
                activePoolObjects.Add(_obj);
            _obj.SetActive(true);

            return _obj;
        }
        if (queue == null)
            objectPool.Add(prefab.name, new Queue<GameObject>());
        _obj = GameObject.Instantiate(prefab);
        _obj.SetActive(true);
        _obj.transform.SetParent(child.transform);
        if (recycle)
            activePoolObjects.Add(_obj);
        return _obj;
    }
    // public GameObject GetObject(GameObject prefab)
    // {
    //     GameObject _obj;
    //     if (pool == null || !GameObject.Find("ObjectPool"))
    //     {
    //         pool = new GameObject("ObjectPool");
    //         GameObject.DontDestroyOnLoad(pool);
    //     }
    //     GameObject child = GameObject.Find(prefab.name);
    //     if (!child)
    //     {
    //         child = new GameObject(prefab.name);
    //         child.transform.SetParent(pool.transform);
    //     }
    //     if (objectPool.TryGetValue(prefab.name, out Queue<GameObject> queue) && queue.Count > 0)
    //     {
    //         _obj = queue.Dequeue();
    //         _obj.SetActive(true);

    //         return _obj;
    //     }
    //     if (queue == null)
    //         objectPool.Add(prefab.name, new Queue<GameObject>());
    //     _obj = GameObject.Instantiate(prefab);
    //     _obj.transform.SetParent(child.transform);
    //     return _obj;
    // }
    public void PushObject(GameObject prefab)
    {
        try
        {
            string _name = prefab.name.Replace("(Clone)", string.Empty);
            if (!objectPool.ContainsKey(_name))
                objectPool.Add(_name, new Queue<GameObject>());
            if (objectPool[_name].Contains(prefab))
                return;
            objectPool[_name].Enqueue(prefab);
            prefab.SetActive(false);
            if (activePoolObjects.Contains(prefab))
                activePoolObjects.Remove(prefab);
        }
        catch (Exception e)
        {
            Debug.LogError("Error when pushing object " + prefab.name + ": " + e.Message);
        }

    }
    public void ResetObject()
    {
        objectPool.Clear();
    }
    public void PushObjects(string name)
    {
        for (int i = 0; i < pool.transform.Find(name).childCount; i++)
        {
            PushObject(pool.transform.Find(name).GetChild(i).gameObject);
        }
    }
    public void AddRecycleObject(GameObject obj)
    {
        activePoolObjects.Add(obj);
    }
    public void RemoveRecycleObject(GameObject obj)
    {
        if (activePoolObjects.Contains(obj))
            activePoolObjects.Remove(obj);
    }
    public void RecycleAll()
    {

        for (int i = activePoolObjects.Count - 1; i >= 0; i--)
        {
            PushObject(activePoolObjects[i]);
        }
        activePoolObjects.Clear();
    }
    public IEnumerator RecycleAllCoroutine()
    {
        Debug.Log(activePoolObjects.Count);
        for (int i = activePoolObjects.Count - 1; i >= 0; i--)
        {
            Debug.Log(activePoolObjects[i].name);
            PushObject(activePoolObjects[i]);
        }
        activePoolObjects.Clear();
        yield return waitForSecondsRealtime;
    }

}
