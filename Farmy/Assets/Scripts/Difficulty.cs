using UnityEngine;
using UnityEngine.UI;

public class Difficulty : MonoBehaviour
{
    private Button button;
    public int difficulty;
    private GameManager gameManager;

    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(SetDifficulty);

        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    void Update()
    {
        //Empty. Can this be removed?
    }

    void SetDifficulty()
    {
        Debug.Log(gameObject.name + " button clicked!");

        if (gameManager != null)
        {
            Debug.Log("Found GameManager, starting game with difficulty " + difficulty);
            gameManager.StartGame(difficulty);
        }
        else
        {
            Debug.LogError("GameManager not found!");
        }
    }
}
