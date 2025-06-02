using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    public GameObject enemyTypeA;
    public GameObject enemyTypeB;

    [Header("Spawner Settings")]
    public Transform[] spawners;           // 4 spawners
    public int enemiesPerSpawner = 4;
    public float spawnInterval = 90f;      // 1.5 minutes
    public int maxTotalEnemies = 100;

    private float timer = 0f;
    private int currentEnemyCount = 0;

    void Start()
    {
        SpawnEnemies(); // Immediate first wave
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnEnemies();
        }
    }

    void SpawnEnemies()
    {
        foreach (Transform spawner in spawners)
        {
            for (int i = 0; i < enemiesPerSpawner; i++)
            {
                if (currentEnemyCount >= maxTotalEnemies) return;

                GameObject selectedPrefab = (Random.value < 0.5f) ? enemyTypeA : enemyTypeB;

                GameObject enemy = Instantiate(selectedPrefab, spawner.position, Quaternion.identity);
                currentEnemyCount++;

                // Optional: decrement when enemy dies
                Targetable target = enemy.GetComponent<Targetable>();
                if (target != null)
                    Targetable.OnTargetableDeath += OnEnemyDeath;
            }
        }
    }

    void OnEnemyDeath(Targetable target)
    {
        currentEnemyCount--;
        Targetable.OnTargetableDeath -= OnEnemyDeath;
    }
}
