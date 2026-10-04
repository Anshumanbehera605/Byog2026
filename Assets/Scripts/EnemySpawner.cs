using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private GameObject[] enemyPrefabs; // Array for multiple enemy types
    [SerializeField] private GameObject spawnEffect;

    [SerializeField] private float minSpawnDistance = 4f;
    [SerializeField] private float maxSpawnDistance = 8f;
    [SerializeField] private float groundLevel = 0f;
    [SerializeField] private float spawnInterval = 2f;
    
    [SerializeField] private int minEnemiesPerWave = 2;
    [SerializeField] private int maxEnemiesPerWave = 5;

    private void Start()
    {
        InvokeRepeating(nameof(SpawnEnemyWave), 1f, spawnInterval);
    }

    public void SpawnEnemyWave()
    {
        if (player == null || enemyPrefabs.Length == 0)
            return;

        int enemiesToSpawn = Random.Range(minEnemiesPerWave, maxEnemiesPerWave + 1);

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            Vector2 randomDirection = Random.insideUnitCircle.normalized;
            // Pick a random distance for each individual enemy
            float randomDistance = Random.Range(minSpawnDistance, maxSpawnDistance);

            Vector3 spawnPosition = player.position + 
                                    new Vector3(randomDirection.x, 0f, randomDirection.y) * randomDistance;
            spawnPosition.y = groundLevel;

            // Spawn magic effect
            if (spawnEffect != null)
            {
                Instantiate(spawnEffect, spawnPosition, Quaternion.identity);
            }

            // Spawn random enemy type from the array
            GameObject randomEnemy = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            Instantiate(randomEnemy, spawnPosition, Quaternion.identity);
        }
    }
}