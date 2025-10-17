using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PestSpawner : MonoBehaviour
{
    [Header("Rabbit Prefabs")]
    public GameObject[] rabbitPrefabs;

    [Header("Spawn Settings")]
    [SerializeField, HideInInspector] private int rabbitsPerWave = 2; 
    [SerializeField, HideInInspector] private float spawnInterval = 3f; 

    [Header("Burrow FX")]
    public GameObject burrowSpawnFX; 
    private AudioSource burrowFXAudioSource;
    public AudioClip burrowAudioFX;

    [Header("Spawning Stuff")]
    private List<GameObject> activeRabbits = new List<GameObject>();
    private List<Transform> burrows = new List<Transform>();

    [Header("Spawn Mechanics")]
    private bool gameStarted = false;
    private float burrowSpawnInterval; 

    private void Start()
    {
        burrowFXAudioSource = gameObject.AddComponent<AudioSource>();
    }

    // difficulty: 1 = easy, 2 = medium, 3 = hard
    public void AdjustDifficulty(int difficulty)
    {
        rabbitsPerWave = 5 * difficulty;
        spawnInterval = Mathf.Max(0.5f, 3f / difficulty);

        switch (difficulty)
        {
            case 1: burrowSpawnInterval = 15f; break; // easy
            case 2: burrowSpawnInterval = 10f; break; // medium
            case 3: burrowSpawnInterval = 5f; break;  // hard
            default: burrowSpawnInterval = 15f; break;
        }

        StartSpawning();
    }

    private void StartSpawning()
    {
        if (gameStarted) return;

        GameObject[] burrowObjects = GameObject.FindGameObjectsWithTag("Burrow");
        foreach (GameObject b in burrowObjects)
        {
            burrows.Add(b.transform);
            b.SetActive(false);
        }

        if (burrows.Count == 0)
        {
            Debug.LogWarning("No burrows found!");
            return;
        }

        gameStarted = true;
        StartCoroutine(SpawnBurrowsWithInitialDelay(1f, burrowSpawnInterval));
    }

    //Do not delete! Fixes the weird Burrow spawn start delay issue
    private IEnumerator SpawnBurrowsWithInitialDelay(float initialDelay, float interval)
    {
        yield return new WaitForSeconds(initialDelay);
        yield return StartCoroutine(SpawnBurrow());

        while (true)
        {
            yield return new WaitForSeconds(interval);
            yield return StartCoroutine(SpawnBurrow());
        }
    }
    private IEnumerator SpawnBurrow()
    {
        List<Transform> inactiveBurrows = burrows.FindAll(b => !b.gameObject.activeSelf);
        if (inactiveBurrows.Count == 0)
            yield break;

        Transform burrow = inactiveBurrows[Random.Range(0, inactiveBurrows.Count)];
        StartCoroutine(AnimateBurrowAppearance(burrow));

        if (burrowAudioFX != null && burrowFXAudioSource != null)
            burrowFXAudioSource.PlayOneShot(burrowAudioFX, 0.5f);

        Debug.Log($"Burrow popped up at {burrow.position}");

        StartCoroutine(SpawnWaveFromBurrow(burrow));
        yield return null;
    }

    // Animate burrow popping up via scaling!!
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

    private IEnumerator SpawnWaveFromBurrow(Transform burrow)
    {
        for (int i = 0; i < rabbitsPerWave; i++)
        {
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
        Destroy(rabbit, 20f);

        // Assign references to spawner and GM (What it needs to know about the scene)
        RabbitMover mover = rabbit.GetComponent<RabbitMover>();
        if (mover != null)
        {
            mover.spawner = this;
            mover.gameManager = FindObjectOfType<GameManager>();
        }

        Debug.Log($"Spawned rabbit at {position}");
    }

    public void RabbitDestroyed(GameObject rabbit)
    {
        if (activeRabbits.Contains(rabbit))
            activeRabbits.Remove(rabbit);
    }

    // Hide all burrows at start - Can't let theplayers see where the bunnies are coming from!
    public void HideAllBurrows()
    {
        Transform burrowParent = GameObject.Find("Burrows").transform;
        foreach (Transform b in burrowParent)
        {
            burrows.Add(b);
            b.gameObject.SetActive(false);
        }
    }
}
