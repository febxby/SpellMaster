using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface ISaleable
{
    int Sell(int price);
}
public class Saleable : MonoBehaviour, ISaleable
{
    public int price;
    public int Sell(int price)
    {
        if (price > this.price)
            return price - this.price;
        return -1;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
