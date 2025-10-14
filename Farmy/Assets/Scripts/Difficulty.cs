using UnityEngine;
using UnityEngine.UI;

public class Difficulty : MonoBehaviour
{
    private Button button;
    public int difficulty;
    private GameManager gameManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(SetDifficulty);
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {

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
