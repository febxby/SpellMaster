using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface IInit
{
    void Init();
}
public class Singleton<T> where T : new()
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new T();
                (instance as IInit)?.Init();
            }
            return instance;
        }
    }
}
