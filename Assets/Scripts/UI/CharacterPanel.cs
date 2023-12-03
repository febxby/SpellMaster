using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterPanel : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Image chargeBar;
    void Start()
    {

    }
    public void UpdateChargeBar(float progress)
    {
        chargeBar.fillAmount = progress;
    }
    // Update is called once per frame
    void Update()
    {

    }
}
