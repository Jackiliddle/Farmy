using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI titleText;
    public Button restartButton;
    public GameObject titleScreen;

    [Header("Gameplay")]
    public PestSpawner pestSpawner;

    private int highScore;
    private int score;
    public bool isGameActive;

    [Header("Countdown")]
    public TextMeshProUGUI startText; 
    public float countdownTime = 3f;

    [Header("Pause")]
    public GameObject pauseMenu; 
    private bool isPaused = false;

    [Header("Instructions Panel")]
    public GameObject instructionsPanel;

    private void Start()
    {
        Time.timeScale = 1f;

        highScore = PlayerPrefs.GetInt("HighScore", 0);
        highScoreText.text = "High Score: " + highScore;

        pestSpawner.HideAllBurrows();
    }

    private void Update()
    {
        if (isGameActive && Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void UpdateScore(int scoreToAdd)
    {
        score += scoreToAdd;
        scoreText.text = "Current score: " + score;

        if (score > highScore)
        {
            highScore = score;
            highScoreText.text = "High Score: " + highScore;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
        }
    }

    public void StartGame(int difficulty)
    {
        Time.timeScale = 1f; 
        titleScreen.SetActive(false);
        StartCoroutine(CountdownAndStart(difficulty));
    }

    private IEnumerator CountdownAndStart(int difficulty)
    {
        startText.gameObject.SetActive(true);

        for (int i = (int)countdownTime; i > 0; i--)
        {
            startText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }

        startText.text = "GO!";
        yield return new WaitForSeconds(1f);
        startText.gameObject.SetActive(false);

        isGameActive = true;
        score = 0;
        UpdateScore(0);

        if (pestSpawner != null)
            pestSpawner.AdjustDifficulty(difficulty);

        CropGrower.StartGrowingCrops();
    }

    public void CheckGameOver()
    {
        GameObject[] veggies = GameObject.FindGameObjectsWithTag("Veggie");
        Debug.Log($"Veggies remaining: {veggies.Length}");

        if (veggies.Length == 0)
            GameOver();
    }

    public void GameOver()
    {
        if (!isGameActive) return;

        gameOverText.gameObject.SetActive(true);
        isGameActive = false;

        if (pestSpawner != null)
            pestSpawner.StopAllCoroutines();

        restartButton.gameObject.SetActive(true);

        Debug.Log("GAME OVER! All veggies have been eaten!");
        Debug.Log("Score: " + score);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Debug.Log("Restart Clicked!");
    }

    public void ShowInstructions()
    {
        if (instructionsPanel != null)
            instructionsPanel.SetActive(true);
    }

    public void HideInstructions()
    {
        if (instructionsPanel != null)
            instructionsPanel.SetActive(false);
    }

    public void TogglePause()
    {
        if (isPaused)
        {
            // Unpause
            Time.timeScale = 1f;
            isPaused = false;
            if (pauseMenu != null) pauseMenu.SetActive(false);
        }
        else
        {
            // Pause
            Time.timeScale = 0f;
            isPaused = true;
            if (pauseMenu != null) pauseMenu.SetActive(true);
        }
    }

    //Do not delete! Fixes the game loop issue!
    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        isPaused = false;
        isGameActive = false;

        if (pauseMenu != null)
            pauseMenu.SetActive(false);

        if (pestSpawner != null)
            pestSpawner.HideAllBurrows();

        titleScreen.SetActive(true);
        gameOverText.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);

        Debug.Log("Returned to Main Menu");
    }
}
