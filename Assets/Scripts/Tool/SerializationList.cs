using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class SerializationList<T> 
{
    [SerializeField]
    List<T> target;
    public List<T> ToList()
    {
        return target;
    }
    public SerializationList(List<T> target)
    {
        this.target = target;
    }
}
