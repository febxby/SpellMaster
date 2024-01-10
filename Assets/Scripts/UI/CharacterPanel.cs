using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public struct HealthChange
{
    public float value;
}
public struct MagicChange
{
    public float value;
}
public struct ChargeChange
{
    public float value;
}
public struct CoinChange
{
    public int value;
}
public class CharacterPanel : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Image healthBar;
    [SerializeField] Image magicBar;
    [SerializeField] Image chargeBar;
    [SerializeField] Text coinText;
    void Awake()
    {
        MEventSystem.Instance.Register<HealthChange>(e => UpdateHealthBar(e.value)).UnRegisterWhenGameObjectDestroy(gameObject);
        MEventSystem.Instance.Register<MagicChange>(e => UpdateMagicBar(e.value)).UnRegisterWhenGameObjectDestroy(gameObject);
        MEventSystem.Instance.Register<ChargeChange>(e => UpdateChargeBar(e.value)).UnRegisterWhenGameObjectDestroy(gameObject);
        MEventSystem.Instance.Register<CoinChange>(e => UpdateCoinText(e.value)).UnRegisterWhenGameObjectDestroy(gameObject);
    }
    public void UpdateChargeBar(float progress)
    {
        chargeBar.fillAmount = progress;
    }
    public void UpdateHealthBar(float progress)
    {
        healthBar.fillAmount = progress;
    }
    public void UpdateMagicBar(float progress)
    {
        magicBar.fillAmount = progress;
    }
    public void UpdateCoinText(int coin)
    {
        coinText.text = coin.ToString();
    }
}
