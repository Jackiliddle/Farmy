using UnityEngine;

public class Veggie : MonoBehaviour
{
    public GameManager gameManager;

    private void Start()
    {
        if (gameManager == null)
            gameManager = FindObjectOfType<GameManager>();
    }

    private void OnDestroy()
    {
        if (gameManager != null)
            gameManager.CheckGameOver();
    }

}
