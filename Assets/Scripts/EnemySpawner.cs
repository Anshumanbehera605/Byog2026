using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject spawnEffect;

    [SerializeField] private float spawnDistance = 6f;
    [SerializeField] private float groundLevel = 0f;
    [SerializeField] private float spawnInterval = 2f;

    private void Start()
    {
        InvokeRepeating(nameof(SpawnEnemy), 1f, spawnInterval);
    }

    public void SpawnEnemy()
    {
        if (player == null || enemyPrefab == null)
            return;

        Vector2 randomDirection = Random.insideUnitCircle.normalized;

        Vector3 spawnPosition = player.position +
                                new Vector3(
                                    randomDirection.x,
                                    0f,
                                    randomDirection.y
                                ) * spawnDistance;

        spawnPosition.y = groundLevel;

        // Spawn magic effect
        if (spawnEffect != null)
        {
            Instantiate(
                spawnEffect,
                spawnPosition,
                Quaternion.identity
            );
        }

        // Spawn enemy
        Instantiate(
            enemyPrefab,
            spawnPosition,
            Quaternion.identity
        );
    }
}