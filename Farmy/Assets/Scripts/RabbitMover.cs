using UnityEngine;

public class RabbitMover : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 3f;
    public float wanderRadius = 2f;
    public float wanderDelay = 2f;

    [Header("Points")]
    public int scoreValue = 5;

    [Header("Movement Boundaries")] // NB: DON'T CHANGE!
    public float minX = -10f;
    public float maxX = 10f;
    public float minZ = -10f;
    public float maxZ = 10f;

    [Header("Eating Settings")]
    public int lapsToEat = 5;

    [Header("Distance-based Eating")]
    public float eatDistance = 1.0f; 
    public float lapDetectionRadius = 3f; 

    [Header("Eating Audio")]
    private AudioSource monchFXAudioSource;
    public AudioClip monchAudioFX;

    [Header("Player Awareness")]
    [SerializeField, HideInInspector] private float fleeRadius = 1.5f;          
    [SerializeField, HideInInspector] private float fleeSpeedMultiplier = 1.2f; 
    [SerializeField, HideInInspector] private float panicDuration = 2f;        

    private Transform player;
    private bool isFleeing = false;
    private float panicTimer = 0f;

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
        // Setup audio for eating
        monchFXAudioSource = gameObject.AddComponent<AudioSource>();
        monchFXAudioSource.volume = 0.3f;

        //Pick initial veggie targets and wander targets
        PickRandomGarden();  
        PickWanderTarget();  
        wanderTimer = wanderDelay;

        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (targetGarden != null)
            lastAngle = GetAngleToTarget();
    }

    void Update()
    {
        if (targetGarden != null)
        {
            DetectPlayer();   // check if player is close
            MoveAndWander();  // move toward target/wander
            TrackLaps();      // count laps around target

            if (lapCount >= lapsToEat)
                EatVeggie(); // eat veggie if laps completed
        }

        ClampPositionToBounds(); // keep rabbit inside boundaries

        if (IsOutOfBounds()) // destroy if somehow outside. DO NOT DELETE!
        {
            Debug.Log($"{gameObject.name} left bounds at {transform.position} → Destroying!");
            Destroy(gameObject);
        }
    }

    // Keep rabbit inside boundaries
    private void ClampPositionToBounds()
    {
        Vector3 pos = transform.position;
        bool hitBoundary = false;

        if (pos.x < minX) { pos.x = minX; hitBoundary = true; }
        if (pos.x > maxX) { pos.x = maxX; hitBoundary = true; }
        if (pos.z < minZ) { pos.z = minZ; hitBoundary = true; }
        if (pos.z > maxZ) { pos.z = maxZ; hitBoundary = true; }

        pos.y = 0f;

        if (hitBoundary)
            PickWanderTarget(); // pick new wander point if hit wall

        transform.position = pos;
    }

    // Check if rabbit is completely outside boundaries. The code stops working without it so DON'T DELETE!
    private bool IsOutOfBounds()
    {
        Vector3 pos = transform.position;
        return pos.x < minX || pos.x > maxX || pos.z < minZ || pos.z > maxZ;
    }

    // Move rabbit: wander or flee if player nearby
    private void MoveAndWander()
    {
        Vector3 direction;

        if (isFleeing && player != null)
        {
            // run away from player
            direction = (transform.position - player.position).normalized;
            float fleeSpeed = speed * fleeSpeedMultiplier;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 8f * Time.deltaTime);
            transform.Translate(Vector3.forward * fleeSpeed * Time.deltaTime);
            return;
        }

        // wander toward target
        direction = (wanderTarget - transform.position).normalized;
        if (direction != Vector3.zero)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 5f * Time.deltaTime);

        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        wanderTimer -= Time.deltaTime;
        if (Vector3.Distance(transform.position, wanderTarget) < 0.5f || wanderTimer <= 0f)
        {
            PickWanderTarget(); // pick new wander spot
            wanderTimer = wanderDelay;
        }
    }

    // Track laps around veggie for eating
    private void TrackLaps()
    {
        if (targetGarden == null) return;

        float distanceToGarden = Vector3.Distance(transform.position, targetGarden.position);

        if (distanceToGarden <= lapDetectionRadius)
        {
            float currentAngle = GetAngleToTarget();
            float deltaAngle = Mathf.DeltaAngle(lastAngle, currentAngle);
            accumulatedRotation += Mathf.Abs(deltaAngle);
            lastAngle = currentAngle;

            if (accumulatedRotation >= 360f)
            {
                lapCount++;
                accumulatedRotation = 0f;
                PickWanderTarget();
                wanderTimer = 0f; // force new target immediately
            }
        }
        else
        {
            lastAngle = GetAngleToTarget();
        }
    }

    // Detect player and trigger fleeing
    private void DetectPlayer()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance < fleeRadius)
        {
            isFleeing = true;
            panicTimer = panicDuration; // start panic
        }
        else if (isFleeing)
        {
            panicTimer -= Time.deltaTime;
            if (panicTimer <= 0f)
                isFleeing = false; // stop fleeing after panic duration
        }
    }

    private float GetAngleToTarget()
    {
        Vector3 dir = transform.position - targetGarden.position;
        return Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;
    }

    // Eat the veggie target
    private void EatVeggie()
    {
        if (targetGarden != null)
        {
            Destroy(targetGarden.gameObject);
            monchFXAudioSource.PlayOneShot(monchAudioFX, 1.0f);

            if (gameManager != null)
                gameManager.CheckGameOver();
        }

        lapCount = 0;
        accumulatedRotation = 0f;

        PickRandomGarden();
        if (targetGarden != null)
            PickWanderTarget();
    }

    // Pick a random veggie target
    private void PickRandomGarden()
    {
        GameObject[] gardens = GameObject.FindGameObjectsWithTag("Veggie");
        gardens = System.Array.FindAll(gardens, g => g != null);
        targetGarden = gardens.Length > 0 ? gardens[Random.Range(0, gardens.Length)].transform : null;
    }

    // Pick a random wander target around the veggie
    private void PickWanderTarget()
    {
        if (targetGarden == null) return;

        Vector2 circle = Random.insideUnitCircle * wanderRadius;
        wanderTarget = targetGarden.position + new Vector3(circle.x, 0, circle.y);

        // Clamp inside play area
        wanderTarget.x = Mathf.Clamp(wanderTarget.x, minX, maxX);
        wanderTarget.z = Mathf.Clamp(wanderTarget.z, minZ, maxZ);
    }

    private void OnDestroy()
    {
        // notify spawner rabbit is gone
        if (spawner != null)
            spawner.RabbitDestroyed(this.gameObject);

        if (gameManager != null)
            gameManager.CheckGameOver();
    }
}
