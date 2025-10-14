using UnityEngine;

public class RabbitMover : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 3f;
    public float wanderRadius = 2f;
    public float wanderDelay = 2f;

    [Header("Movement Boundaries")]
    public float minX = -10f;
    public float maxX = 10f;
    public float minZ = -10f;
    public float maxZ = 10f;

    [Header("Eating Settings")]
    public int lapsToEat = 5;

    [HideInInspector]
    public PestSpawner spawner;

    private Transform targetGarden;
    private Vector3 wanderTarget;
    private float wanderTimer;

    private int lapCount = 0;
    private float lastAngle = 0f;
    private float accumulatedRotation = 0f;

    void Start()
    {
        PickRandomGarden();
        PickWanderTarget();
        wanderTimer = wanderDelay;

        if (targetGarden != null)
            lastAngle = GetAngleToTarget();
    }

    void Update()
    {
        if (targetGarden != null)
        {
            MoveAndWander();
            TrackLaps();

            if (lapCount >= lapsToEat)
                EatVeggie();
        }

        // HARD boundary destruction
        if (IsOutOfBounds())
        {
            Debug.Log($"{gameObject.name} left bounds at {transform.position} → Destroying!");
            Destroy(gameObject);
        }
    }

    private bool IsOutOfBounds()
    {
        Vector3 pos = transform.position;
        return pos.x < minX || pos.x > maxX || pos.z < minZ || pos.z > maxZ;
    }

    private void MoveAndWander()
    {
        Vector3 direction = (wanderTarget - transform.position).normalized;
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

    private float GetAngleToTarget()
    {
        Vector3 dir = transform.position - targetGarden.position;
        return Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;
    }

    private void EatVeggie()
    {
        if (targetGarden != null)
            Destroy(targetGarden.gameObject);

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
            spawner.RabbitDestroyed(this.gameObject);
    }
}
