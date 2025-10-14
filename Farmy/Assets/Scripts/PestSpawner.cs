using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PestSpawner : MonoBehaviour
{
    [Header("Rabbit Prefabs")]
    public GameObject[] rabbitPrefabs;

    [Header("Spawn Settings")]
    public int rabbitsPerWave = 10;
    public float spawnInterval = 3f;
    public float delayBetweenWaves = 30f;

    [Tooltip("X/Z spawn area boundaries")]
    public float spawnRangeX = 10f;
    public float spawnRangeZ = 10f;

    private List<GameObject> activeRabbits = new List<GameObject>();
    private bool gameStarted = false;

    public void AdjustDifficulty(int difficulty)
    {
        rabbitsPerWave = 10 * difficulty;
        spawnInterval = Mathf.Max(0.5f, 3f / difficulty);
        delayBetweenWaves = Mathf.Max(10f, 30f / difficulty);

        Debug.Log($"Difficulty {difficulty}: {rabbitsPerWave} rabbits per wave, interval {spawnInterval}s");
        StartSpawning();
    }

    private void StartSpawning()
    {
        if (gameStarted) return;
        gameStarted = true;
        StartCoroutine(SpawnWaves());
    }

    private IEnumerator SpawnWaves()
    {
        while (true)
        {
            yield return StartCoroutine(SpawnWave());

            // wait until all rabbits destroyed before next wave
            yield return new WaitUntil(() => activeRabbits.Count == 0);

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

        RabbitMover mover = rabbit.GetComponent<RabbitMover>();
        if (mover != null)
            mover.spawner = this;

        Debug.Log($"Spawned rabbit at {spawnPos}");
    }

    public void RabbitDestroyed(GameObject rabbit)
    {
        if (activeRabbits.Contains(rabbit))
            activeRabbits.Remove(rabbit);
    }
}
