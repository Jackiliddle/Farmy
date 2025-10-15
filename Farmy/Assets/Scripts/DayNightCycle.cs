using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Header("Cycle Settings")]
    public float dayLength = 720f;   // Full rotation every 720 seconds
    public Transform pivotPoint;     // Optional: what the sun orbits around

    void Update()
    {
        float rotationSpeed = 720f / dayLength;

        Vector3 pivot;

        if (pivotPoint != null)
        {
            pivot = pivotPoint.position;
        }
        else
        {
            pivot = Vector3.zero;
        }
        transform.RotateAround(pivot, Vector3.right, rotationSpeed * Time.deltaTime);
    }
}
