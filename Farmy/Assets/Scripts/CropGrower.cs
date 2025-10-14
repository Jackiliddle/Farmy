using UnityEngine;
using System.Collections;

public class CropGrower : MonoBehaviour
{
    [Header("Growth Settings")]
    public float growDuration = 15f;   // Time to fully grow
    public float finalScale = 3f;      // Target scale size

    public static void StartGrowingCrops()
    {
        GameObject[] crops = GameObject.FindGameObjectsWithTag("Veggie");
        foreach (GameObject crop in crops)
        {
            if (crop != null)
            {
                CropGrower grower = crop.GetComponent<CropGrower>();
                if (grower != null)
                {
                    crop.transform.localScale = Vector3.zero;
                    grower.StartCoroutine(grower.GrowCropCoroutine());
                }
            }
        }
    }

    public IEnumerator GrowCropCoroutine()
    {
        Vector3 startScale = Vector3.zero;
        Vector3 targetScale = Vector3.one * finalScale;

        float elapsed = 0f;

        while (elapsed < growDuration)
        {
            elapsed += Time.deltaTime;
            transform.localScale = Vector3.Lerp(startScale, targetScale, elapsed / growDuration);
            yield return null;
        }

        transform.localScale = targetScale;
    }
}
