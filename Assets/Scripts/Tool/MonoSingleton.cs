using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : Component
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<T>();
                if (instance == null)
                {
                    GameObject obj = new(typeof(T).Name);
                    DontDestroyOnLoad(obj);
                    instance = obj.AddComponent<T>();
                }
            }
            return instance;
        }
    }
}
