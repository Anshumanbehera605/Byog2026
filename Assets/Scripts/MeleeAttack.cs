using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    [SerializeField] private int damage = 25;

    private SphereCollider attackCollider;

    // Enemies currently inside attack range
    private HashSet<EnemyHealth> enemiesInRange = new HashSet<EnemyHealth>();

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
            Attack();
        }
    }

    private void Attack()
    {
        EnemyHealth nearestEnemy = GetNearestEnemy();

        if (nearestEnemy != null)
        {
            nearestEnemy.TakeDamage(damage, transform);
            Debug.Log("Hit enemy for " + damage + " damage!");
        }
        else
        {
            Debug.Log("No enemy in range.");
        }
    }

    private EnemyHealth GetNearestEnemy()
    {
        EnemyHealth nearestEnemy = null;
        float nearestDistance = Mathf.Infinity;

        foreach (EnemyHealth enemy in enemiesInRange)
        {
            // Safety check in case enemy was destroyed
            if (enemy == null)
                continue;

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