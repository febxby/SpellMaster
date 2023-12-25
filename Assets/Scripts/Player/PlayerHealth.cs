using UnityEngine;
public interface IDamageable
{
    void TakeDamage(int damageAmount);
}

public class PlayerHealth : MonoBehaviour, IDamageable
{
    public int maxHealth = 100;
    public int currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // TODO: Implement death logic here
        Debug.Log("Player has died");
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Health"))
        {
            currentHealth += 10;
            GameObjectPool.Instance.PushObject(other.gameObject);
        }
    }
}
