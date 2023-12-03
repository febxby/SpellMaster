using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCollider : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter(Collision other) {
        Debug.Log("1"+other.gameObject.name);
    }
    private void OnTriggerEnter(Collider other) {
        Debug.Log("2"+other.gameObject.name);
        
    }
}
