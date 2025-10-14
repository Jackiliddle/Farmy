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
    public float delayBetweenWaves = 15f;

    [Header("Burrow FX")]
    public GameObject burrowSpawnFX; 
    private AudioSource burrowFXAudioSource;
    public AudioClip burrowAudioFX;

    private List<GameObject> activeRabbits = new List<GameObject>();
    private List<Transform> burrows = new List<Transform>();

    private bool gameStarted = false;
    private int waveNumber = 0;

    public void Start()
    {
        burrowFXAudioSource = gameObject.AddComponent<AudioSource>();
        //sfxAudioSource.volume = 0.3f;
    }

    public void AdjustDifficulty(int difficulty)
    {
        rabbitsPerWave = 10 * difficulty;
        spawnInterval = Mathf.Max(0.5f, 3f / difficulty);
        delayBetweenWaves = Mathf.Max(10f, 15f / difficulty);

        Debug.Log($"Difficulty {difficulty}: {rabbitsPerWave} rabbits per wave, interval {spawnInterval}s");
        StartSpawning();
    }

    private void StartSpawning()
    {
        if (gameStarted) return;

        // Find all burrows (tagged "Burrow")
        GameObject[] burrowObjects = GameObject.FindGameObjectsWithTag("Burrow");
        foreach (GameObject b in burrowObjects)
        {
            burrows.Add(b.transform);
            b.SetActive(false); // start with all burrows inactive
        }

        if (burrows.Count == 0)
        {
            Debug.LogWarning("No burrows found! Make sure your burrow objects are tagged 'Burrow'.");
            return;
        }

        // ✅ Start with one active burrow
        burrows[0].gameObject.SetActive(true);

        gameStarted = true;
        StartCoroutine(SpawnWaves());
    }

    private IEnumerator SpawnWaves()
    {
        while (true)
        {
            waveNumber++;
            Debug.Log($"Starting wave {waveNumber}");

            // ✅ Activate one more burrow each new wave
            ActivateBurrows(waveNumber);

            // ✅ Scale difficulty: +5 rabbits, spawn interval -1s (down to min 1f)
            rabbitsPerWave += 5;
            spawnInterval = Mathf.Max(1f, spawnInterval - 1f);

            Debug.Log($"Wave {waveNumber} — {rabbitsPerWave} rabbits, spawn interval {spawnInterval}s");

            yield return StartCoroutine(SpawnWave());

            // Wait until all rabbits destroyed before next wave
            yield return new WaitUntil(() => activeRabbits.Count == 0);

            yield return new WaitForSeconds(delayBetweenWaves);
        }
    }

    private void ActivateBurrows(int wave)
    {
        int burrowsToActivate = Mathf.Min(wave, burrows.Count);

        for (int i = 0; i < burrowsToActivate; i++)
        {
            if (!burrows[i].gameObject.activeSelf)
            {
                StartCoroutine(AnimateBurrowAppearance(burrows[i]));
                
                //Poof burrow appears noise
                if (burrowAudioFX != null && burrowFXAudioSource != null)
                {
                    burrowFXAudioSource.PlayOneShot(burrowAudioFX, 0.5f); // 1f = full volume
                }
            }
        }

        Debug.Log($"{burrowsToActivate} burrows active this wave.");
    }

    private IEnumerator AnimateBurrowAppearance(Transform burrow)
    {
        if (burrowSpawnFX != null)
            Instantiate(burrowSpawnFX, burrow.position, Quaternion.identity);

        burrow.localScale = Vector3.zero;
        burrow.gameObject.SetActive(true);

        float duration = 1f;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float scale = Mathf.SmoothStep(0f, 1f, t / duration);
            burrow.localScale = Vector3.one * scale;
            yield return null;
        }

        burrow.localScale = Vector3.one;
    }

    private IEnumerator SpawnWave()
    {
        // Get all currently active burrows
        List<Transform> activeBurrows = burrows.FindAll(b => b.gameObject.activeSelf);

        for (int i = 0; i < rabbitsPerWave; i++)
        {
            Transform burrow = activeBurrows[Random.Range(0, activeBurrows.Count)];
            SpawnRabbit(burrow.position);
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnRabbit(Vector3 position)
    {
        if (rabbitPrefabs.Length == 0) return;

        GameObject prefab = rabbitPrefabs[Random.Range(0, rabbitPrefabs.Length)];
        Quaternion rot = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

        GameObject rabbit = Instantiate(prefab, position, rot);
        activeRabbits.Add(rabbit);

        RabbitMover mover = rabbit.GetComponent<RabbitMover>();
        if (mover != null)
        {
            mover.spawner = this;
            mover.gameManager = FindObjectOfType<GameManager>();
        }

        Debug.Log($"Spawned rabbit at burrow position {position}");
    }

    public void RabbitDestroyed(GameObject rabbit)
    {
        if (activeRabbits.Contains(rabbit))
            activeRabbits.Remove(rabbit);
    }
}
