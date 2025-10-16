using UnityEngine;
using System.Collections;

public class CropGrower : MonoBehaviour
{
    [Header("Growth Settings")]
    private float finalScale = 3f;
    private const float GrowDuration = 15f;

    // Starts the growth process for all GameObjects tagged as "Veggie".

    public static void StartGrowingCrops()
    {
        foreach (var crop in GameObject.FindGameObjectsWithTag("Veggie"))
        {
            if (crop == null) continue;

            var grower = crop.GetComponent<CropGrower>();
            if (grower != null)
            {
                crop.transform.localScale = Vector3.zero;
                grower.StartCoroutine(grower.GrowCropCoroutine());
            }
        }
    }

    //Scaling up the crops.
    private IEnumerator GrowCropCoroutine()
    {
        Vector3 startScale = Vector3.zero;
        Vector3 targetScale = Vector3.one * finalScale;

        float elapsed = 0f;

        while (elapsed < GrowDuration)
        {
            elapsed += Time.deltaTime;
            transform.localScale = Vector3.Lerp(startScale, targetScale, elapsed / GrowDuration);
            yield return null;
        }

        //Is it 3f?
        transform.localScale = targetScale;
    }
}
