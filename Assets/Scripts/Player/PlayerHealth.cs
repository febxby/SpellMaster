using UnityEngine;
public interface IDamageable
{
    void TakeDamage(int damageAmount);
}

public class PlayerHealth : MonoBehaviour, IDamageable
{
    // public int maxHealth = 100;

    private void Start()
    {
        // currentHealth = maxHealth;
    }

    public void TakeDamage(int damageAmount)
    {
        GameManger.Instance.DamageText(transform.position, damageAmount);

    }

}
