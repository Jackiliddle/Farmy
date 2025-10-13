using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PestSpawner : MonoBehaviour
{
    [Header("Rabbit Prefabs")]
    public GameObject[] rabbitPrefabs;  // Assign your rabbit prefabs

    [Header("Spawn Settings")]
    public int rabbitsPerWave = 10;
    public float spawnInterval = 3f;
    public float delayBetweenWaves = 30f;

    [Tooltip("X/Z spawn area boundaries")]
    public float spawnRangeX = 10f;
    public float spawnRangeZ = 10f;

    private List<GameObject> activeRabbits = new List<GameObject>();
    private bool spawning = false;

    private void Start()
    {
        StartCoroutine(SpawnWaves());
    }

    private IEnumerator SpawnWaves()
    {
        while (true)
        {
            // Start a new wave
            spawning = true;
            yield return StartCoroutine(SpawnWave());
            spawning = false;

            // Wait until all rabbits from this wave are destroyed
            yield return new WaitUntil(() => activeRabbits.Count == 0);

            // Wait additional time before the next wave
            yield return new WaitForSeconds(delayBetweenWaves);
        }
    }

    private IEnumerator SpawnWave()
    {
        for (int i = 0; i < rabbitsPerWave; i++)
        {
            SpawnRabbit();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnRabbit()
    {
        if (rabbitPrefabs.Length == 0) return;

        GameObject prefab = rabbitPrefabs[Random.Range(0, rabbitPrefabs.Length)];
        Vector3 spawnPos = new Vector3(
            Random.Range(-spawnRangeX, spawnRangeX),
            0f,
            Random.Range(-spawnRangeZ, spawnRangeZ)
        );
        Quaternion rot = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

        GameObject rabbit = Instantiate(prefab, spawnPos, rot);
        activeRabbits.Add(rabbit);

        // Remove from list when destroyed
        RabbitDespawn despawn = rabbit.AddComponent<RabbitDespawn>();
        despawn.onDestroyed += () => activeRabbits.Remove(rabbit);
    }

    // Small helper class to detect destruction of spawned rabbits
    private class RabbitDespawn : MonoBehaviour
    {
        public System.Action onDestroyed;
        private void OnDestroy() => onDestroyed?.Invoke();
    }
}
