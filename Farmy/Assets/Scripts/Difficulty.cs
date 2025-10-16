using UnityEngine;
using UnityEngine.UI;

public class Difficulty : MonoBehaviour
{
    private Button button;
    public int difficulty;
    private GameManager gameManager;

    void Start()
    {
        // Get button component and add listener
        button = GetComponent<Button>();
        button.onClick.AddListener(SetDifficulty);

        // Find GameManager in scene
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        //Empty. Can this be removed?
    }

    // Called when the difficulty button is clicked
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
