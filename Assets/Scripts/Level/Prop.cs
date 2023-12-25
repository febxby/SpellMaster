using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prop : MonoBehaviour
{
    public float speed;
    public float distance;
    // Start is called before the first frame update
    GameObject player;
    private void Awake()
    {
        player = FindFirstObjectByType<PlayerController>().gameObject;
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, player.transform.position) >= distance)
        {
            Vector3 direction = player.transform.position - transform.position;
            direction.Normalize();
            transform.Translate(direction * Time.deltaTime * speed);
        }
    }
}
