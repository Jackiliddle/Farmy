using UnityEngine;

public class RabbitMover : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 3f;
    public float wanderRadius = 2f;
    public float wanderDelay = 2f;

    [Header("Points")]
    public int scoreValue = 5;

    [Header("Movement Boundaries")] //NB: DON'T CHANGE!
    public float minX = -10f;
    public float maxX = 10f;
    public float minZ = -10f;
    public float maxZ = 10f;

    [Header("Eating Settings")]
    public int lapsToEat = 5;

    [Header("Eating Auio")]
    private AudioSource monchFXAudioSource;
    public AudioClip monchAudioFX;

    // * * * * New * * * *
    [Header("Player Awareness")]
    public float fleeRadius = 0.5f;          // How close the player must be for rabbit to flee
    public float fleeSpeedMultiplier = 1f; // How much faster rabbit runs when fleeing
    public float panicDuration = 0.5f;     // How long rabbit stays in flee mode after player leaves
    private Transform player;
    private bool isFleeing = false;
    private float panicTimer = 0f;
    // * * * * New * * * *

    [Header("Garden Seeking")]
    private Transform targetGarden;
    private Vector3 wanderTarget;
    private float wanderTimer;

    [Header("Running Mechs")]
    private int lapCount = 0;
    private float lastAngle = 0f;
    private float accumulatedRotation = 0f;

    [HideInInspector]
    public PestSpawner spawner;
    public GameManager gameManager;

    void Start()
    {
        monchFXAudioSource = gameObject.AddComponent<AudioSource>();
        monchFXAudioSource.volume = 0.3f; 
        PickRandomGarden();
        PickWanderTarget();
        wanderTimer = wanderDelay;

        // * * * * New * * * *
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        // * * * * New * * * *

        if (targetGarden != null)
            lastAngle = GetAngleToTarget();

    }

    void Update()
    {
        if (targetGarden != null)
        {
            // * * * * New * * * *
            DetectPlayer();
            // * * * * New * * * *

            MoveAndWander();
            TrackLaps();

            if (lapCount >= lapsToEat)
                EatVeggie();
        }

        //Added to keep bunnies in xz 10 x 10 area, destroy on reaching x 10 or -10, z 10 or -10:
        ClampPositionToBounds();

        // HARD boundary destruction. NB: DO NOT TOUCH! THIS WORKS!
        if (IsOutOfBounds())
        {
            Debug.Log($"{gameObject.name} left bounds at {transform.position} → Destroying!");
            Destroy(gameObject);
        }
    }
    
    // Keeps rabbits inside the play area bounds
    private void ClampPositionToBounds()
    {
        Vector3 pos = transform.position;

        bool hitBoundary = false;

        if (pos.x < minX) { pos.x = minX; hitBoundary = true; }
        if (pos.x > maxX) { pos.x = maxX; hitBoundary = true; }
        if (pos.z < minZ) { pos.z = minZ; hitBoundary = true; }
        if (pos.z > maxZ) { pos.z = maxZ; hitBoundary = true; }

        //Stops rabbits flying away!!!!
        pos.y = 0f;

        if (hitBoundary)
        {
         // Turn them around when they hit a boundary
         PickWanderTarget();
        }

        transform.position = pos;
    }

    //SAFEGAURD for rogue wandering rabbits DO NOT REMOVE! GAME DOESNT END OTHERWISE!
    private bool IsOutOfBounds()
    {
        Vector3 pos = transform.position;
        return pos.x < minX || pos.x > maxX || pos.z < minZ || pos.z > maxZ;
    }

    private void MoveAndWander()
    {
        Vector3 direction;

        if (isFleeing && player != null)
        {
            // Move away from the player
            direction = (transform.position - player.position).normalized;
            float fleeSpeed = speed * fleeSpeedMultiplier;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 8f * Time.deltaTime);
            transform.Translate(Vector3.forward * fleeSpeed * Time.deltaTime);
            return; // Skip wandering this frame
        }

        // Normal wandering
        direction = (wanderTarget - transform.position).normalized;
        if (direction != Vector3.zero)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 5f * Time.deltaTime);

        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        wanderTimer -= Time.deltaTime;
        if (Vector3.Distance(transform.position, wanderTarget) < 0.5f || wanderTimer <= 0f)
        {
            PickWanderTarget();
            wanderTimer = wanderDelay;
        }
    }
    // * * * * New * * * * 

    private void TrackLaps()
    {
        if (targetGarden == null) return;

        float currentAngle = GetAngleToTarget();
        float deltaAngle = Mathf.DeltaAngle(lastAngle, currentAngle);
        accumulatedRotation += Mathf.Abs(deltaAngle);
        lastAngle = currentAngle;

        if (accumulatedRotation >= 360f)
        {
            lapCount++;
            accumulatedRotation = 0f;
        }
    }

    // * * * * New * * * * 
    private void DetectPlayer()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance < fleeRadius)
        {
            // Start fleeing if not already
            isFleeing = true;
            panicTimer = panicDuration;
        }
        else if (isFleeing)
        {
            // Countdown panic timer once player leaves radius
            panicTimer -= Time.deltaTime;
            if (panicTimer <= 0f)
            {
                isFleeing = false;
            }
        }
    }
    // * * * * New * * * * 

    private float GetAngleToTarget()
    {
        Vector3 dir = transform.position - targetGarden.position;
        return Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;
    }
    
    private void EatVeggie()
    {
    if (targetGarden != null)
    {
            Destroy(targetGarden.gameObject);
            monchFXAudioSource.PlayOneShot(monchAudioFX, 1.0f);

        if (gameManager != null)
        {
            gameManager.CheckGameOver();
        }
    }

    lapCount = 0;
    accumulatedRotation = 0f;
    PickRandomGarden();

        if (targetGarden != null)
            PickWanderTarget();
        }



    private void PickRandomGarden()
    {
        GameObject[] gardens = GameObject.FindGameObjectsWithTag("Veggie");
        gardens = System.Array.FindAll(gardens, g => g != null);
        targetGarden = gardens.Length > 0 ? gardens[Random.Range(0, gardens.Length)].transform : null;
    }

    private void PickWanderTarget()
    {
        if (targetGarden == null) return;
        Vector2 circle = Random.insideUnitCircle * wanderRadius;
        wanderTarget = targetGarden.position + new Vector3(circle.x, 0, circle.y);
    }

    private void OnDestroy()
    {
        if (spawner != null)
        {
            spawner.RabbitDestroyed(this.gameObject);
        }

        if (gameManager != null)
        {
            gameManager.CheckGameOver();
        }
    }

}
