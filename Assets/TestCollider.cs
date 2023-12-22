using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCollider : MonoBehaviour
{
    public float strength;
    public int radius;
    public Transform pos;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.parent.TryGetComponent<TerrainGenerator>(out var terrainGenerator))
            terrainGenerator.TouchingCallback(other.GetContact(0).point, radius, strength);

    }

}
