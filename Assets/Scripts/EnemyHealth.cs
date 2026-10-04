using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 100;

    private int currentHealth;

    private Enemy enemy;
    private Animator animator;
    
    // Tracks if THIS script knows the enemy is dead
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        enemy = GetComponent<Enemy>();

        if (enemy == null)
        {
            Debug.LogError("EnemyHealth requires an Enemy component!");
        }
    }

    public void TakeDamage(int damage, Transform attacker = null)
    {
        // Prevent taking more damage or running logic after already dead
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log(gameObject.name + " took " + damage + " damage! Health: " + currentHealth + "/" + maxHealth);

        if (enemy != null && attacker != null)
        {
            Vector3 knockbackDirection = transform.position - attacker.position;
            knockbackDirection.y = 0f;

            if (knockbackDirection.sqrMagnitude > 0.001f)
            {
                knockbackDirection.Normalize();
                enemy.ApplyKnockback(knockbackDirection);
            }
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        animator.SetTrigger("Death");
        Debug.Log(gameObject.name + " died!");

        // Disable the Enemy.cs script completely so it stops following and attacking
        if (enemy != null)
        {
            enemy.enabled = false; 
        }

        Destroy(gameObject, 3f); 
    }
}