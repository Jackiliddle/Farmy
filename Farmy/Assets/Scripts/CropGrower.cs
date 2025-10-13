using UnityEngine;
using System.Collections;

public class CropGrower : MonoBehaviour
{
    [Header("Growth Settings")]
    public string cropTag = "Veggie";  // All crops share this tag
    public float growDuration = 15f;   // Time to fully grow
    public float finalScale = 3f;      // Target scale size

    private void Start()
    {
        StartCoroutine(GrowAllCrops());
    }

    private IEnumerator GrowAllCrops()
    {
        // Find all crops in the scene with the tag
        GameObject[] crops = GameObject.FindGameObjectsWithTag(cropTag);

        foreach (GameObject crop in crops)
        {
            // Start each crop growing independently
            StartCoroutine(GrowCrop(crop.transform));
        }

        yield return null;
    }

    private IEnumerator GrowCrop(Transform crop)
    {
        Vector3 startScale = Vector3.zero;
        Vector3 targetScale = Vector3.one * finalScale;
        float elapsed = 0f;

        // Optional: reset initial scale to 0 (seed)
        crop.localScale = startScale;

        while (elapsed < growDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / growDuration;
            crop.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }

        // Ensure final scale is exact
        crop.localScale = targetScale;
    }
}
