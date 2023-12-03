using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManger : MonoBehaviour
{
    bool isClicking;
    [Header("Actions")]
    public static Action<Vector3> OnTouching;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
            isClicking = true;
        else if(isClicking && Mouse.current.leftButton.isPressed)
        {
            Clicking();
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame&&isClicking)
            isClicking = false;
    }

    private void Clicking()
    {
        RaycastHit hit;
        Physics.Raycast(Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()), out hit,50);
        if (hit.collider is null)
            return;
        OnTouching?.Invoke(hit.point);
    }
}
