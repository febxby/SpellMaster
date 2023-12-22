using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareTester : MonoBehaviour
{
    public GameObject squarePrefab;
    public int width;
    public int height;
    // Start is called before the first frame update
    void Start()
    {
        //根据宽高生成多个TerrainGenerator
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                GameObject go = Instantiate(squarePrefab, new Vector3(x * 19, y * 19, 0), Quaternion.identity);
                go.transform.parent = transform;
            }
        }

    }

    // Update is called once per frame
    void Update()
    {

    }


}
