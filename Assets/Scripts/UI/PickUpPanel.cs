using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PickUpPanel : MonoBehaviour
{
    // Start is called before the first frame update
    //TODO:完善拾取功能
    private void OnEnable()
    {
        MEventSystem.Instance.Send<SwitchActionMap>(new SwitchActionMap()
        {
            actionMapName = "Editor"
        });
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
