﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;
    public float speed;
    public Transform target;
    void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if(target != null)
            transform.position = Vector3.MoveTowards(transform.position,new Vector3(target.position.x,target.position.y,transform.position.z), speed * Time .deltaTime);
    }

    public void ChangeTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
