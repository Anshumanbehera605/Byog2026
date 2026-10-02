using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 100;

    private int currentHealth;

    private Enemy enemy;

    void Start()
    {
        currentHealth = maxHealth;

        enemy = GetComponent<Enemy>();

        if (enemy == null)
        {
            Debug.LogError("EnemyHealth requires an Enemy component!");
        }
    }

    public void TakeDamage(int damage, Transform attacker = null)
    {
        currentHealth -= damage;

        Debug.Log(
            gameObject.name + " took " + damage +
            " damage! Health: " + currentHealth + "/" + maxHealth
        );

        // Apply knockback
        if (enemy != null && attacker != null)
        {
            Vector3 knockbackDirection =
                transform.position - attacker.position;

            knockbackDirection.y = 0f;

            if (knockbackDirection.sqrMagnitude > 0.001f)
            {
                knockbackDirection.Normalize();
                enemy.ApplyKnockback(knockbackDirection);
            }
        }

        // Die
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log(gameObject.name + " died!");

        Destroy(gameObject);
    }
}