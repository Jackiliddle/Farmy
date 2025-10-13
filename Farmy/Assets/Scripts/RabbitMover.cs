using UnityEngine;

public class RabbitMover : MonoBehaviour
{
    public float speed = 3f;
    public float wanderRadius = 2f;
    public float wanderDelay = 2f;

    private Transform targetGarden;
    private Vector3 wanderTarget;
    private float wanderTimer;


    void Start()
    {
        PickRandomGarden();
        PickWanderTarget();
        wanderTimer = wanderDelay;
    }

    void Update()
    {
        // Movement
        if (targetGarden != null)
        {
            Vector3 direction = (wanderTarget - transform.position).normalized;
            if (direction != Vector3.zero)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 5f * Time.deltaTime);

            transform.Translate(Vector3.forward * speed * Time.deltaTime);

            float distance = Vector3.Distance(transform.position, wanderTarget);
            wanderTimer -= Time.deltaTime;
            if (distance < 0.5f || wanderTimer <= 0f)
            {
                PickWanderTarget();
                wanderTimer = wanderDelay;
            }
        }
    }

    void PickRandomGarden()
    {
        GameObject[] gardens = GameObject.FindGameObjectsWithTag("Patch"); // or "GardenSection"
        if (gardens.Length > 0)
            targetGarden = gardens[Random.Range(0, gardens.Length)].transform;
    }

    void PickWanderTarget()
    {
        if (targetGarden == null) return;
        Vector2 randomCircle = Random.insideUnitCircle * wanderRadius;
        wanderTarget = targetGarden.position + new Vector3(randomCircle.x, 0, randomCircle.y);
    }
}
