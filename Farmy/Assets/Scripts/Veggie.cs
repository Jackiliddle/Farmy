using UnityEngine;


//This code is here because my stupid game stopped understanding when to do Game Over without it. idk why. 
public class Veggie : MonoBehaviour
{
    public GameManager gameManager;

    private void Start()
    {
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
        }
    }

    private void OnDestroy()
    {
        // Called when the veggie is destroyed
        if (gameManager != null)
        {
            gameManager.CheckGameOver();
        }
    }
}
