using UnityEngine;
using System.Collections;

public class CropGrower : MonoBehaviour
{
    [Header("Growth Settings")]
    public string cropTag = "Veggie";  // All crops share this tag
    public float growDuration = 60f;   // Time to fully grow
    public float finalScale = 3f;      // Target scale size

    // Public static method to trigger growth
    public static void StartGrowingCrops()
    {
        GameObject[] crops = GameObject.FindGameObjectsWithTag("Veggie");
        foreach (GameObject crop in crops)
        {
            if (crop != null)
            {
                crop.transform.localScale = Vector3.zero;
                crop.GetComponent<MonoBehaviour>().StartCoroutine(GrowCropCoroutine(crop.transform));
            }
        }
    }

    private static IEnumerator GrowCropCoroutine(Transform crop)
    {
        Vector3 startScale = Vector3.zero;
        Vector3 targetScale = Vector3.one * 3f; // finalScale

        float elapsed = 0f;
        float duration = 15f; // growDuration

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            crop.localScale = Vector3.Lerp(startScale, targetScale, elapsed / duration);
            yield return null;
        }

        crop.localScale = targetScale;
    }
}
