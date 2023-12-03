using System.Collections.Generic;
using UnityEngine;
public class GameObjectPool : Singleton<GameObjectPool>
{
    private readonly Dictionary<string, Queue<GameObject>> objectPool = new();
    private GameObject pool;
    public GameObject GetObject(GameObject prefab)
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
            _obj.SetActive(true);

            return _obj;
        }
        if (queue == null)
            objectPool.Add(prefab.name, new Queue<GameObject>());
        _obj = GameObject.Instantiate(prefab);
        _obj.transform.SetParent(child.transform);
        return _obj;


    }
    public void PushObject(GameObject prefab)
    {
        string _name = prefab.name.Replace("(Clone)", string.Empty);
        if (!objectPool.ContainsKey(_name))
            objectPool.Add(_name, new Queue<GameObject>());
        objectPool[_name].Enqueue(prefab);
        prefab.SetActive(false);
    }
    public void ResetObject()
    {
        objectPool.Clear();
    }



}
