using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    [SerializeField] private int damage = 25;
    [SerializeField] private float damageDelay = 0.5f; // How long to wait before damage applies

    private SphereCollider attackCollider;

    // Enemies currently inside attack range
    private HashSet<EnemyHealth> enemiesInRange = new HashSet<EnemyHealth>();

    public Animator animator;

    void Start()
    {
        attackCollider = GetComponent<SphereCollider>();

        if (attackCollider == null)
        {
            Debug.LogError("MeleeAttack requires a SphereCollider!");
        }
    }

    void Update()
    {
        // Left mouse button
        if (Input.GetMouseButtonDown(0))
        {
            // Start the delayed attack routine
            StartCoroutine(AttackRoutine());
        }
    }

    private IEnumerator AttackRoutine()
    {
        // 1. Play the animation instantly when the button is pressed
        if (animator != null)
        {
            animator.SetTrigger("attack");
        }

        // 2. Wait for the delay (e.g., wait for the sword to swing forward)
        yield return new WaitForSeconds(damageDelay);

        // 3. Apply damage after the delay finishes
        EnemyHealth nearestEnemy = GetNearestEnemy();

        if (nearestEnemy != null)
        {
            nearestEnemy.TakeDamage(damage, transform);
            Debug.Log("Hit enemy for " + damage + " damage!");
        }
        else
        {
            Debug.Log("No enemy in range when the attack landed.");
        }
    }

    private EnemyHealth GetNearestEnemy()
    {
        EnemyHealth nearestEnemy = null;
        float nearestDistance = Mathf.Infinity;

        // Safety check to remove any enemies that were destroyed
        enemiesInRange.RemoveWhere(enemy => enemy == null);

        foreach (EnemyHealth enemy in enemiesInRange)
        {
            float distance = Vector3.Distance(
                transform.position,
                enemy.transform.position
            );

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestEnemy = enemy;
            }
        }

        return nearestEnemy;
    }

    private void OnTriggerEnter(Collider other)
    {
        EnemyHealth enemy = other.GetComponentInParent<EnemyHealth>();

        if (enemy != null)
        {
            enemiesInRange.Add(enemy);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        EnemyHealth enemy = other.GetComponentInParent<EnemyHealth>();

        if (enemy != null)
        {
            enemiesInRange.Remove(enemy);
        }
    }
}