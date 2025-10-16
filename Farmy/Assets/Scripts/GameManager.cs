using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI cashText;
    public Button restartButton;
    public GameObject titleScreen;

    [Header("Gameplay")]
    public PestSpawner pestSpawner;

    private int highScore;
    private int score;
    public bool isGameActive;

    // Keep highscore here
    private void Start()
    {
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        highScoreText.text = "High Score: " + highScore;

        // Hide all burrows at start
        pestSpawner.HideAllBurrows();
    }

    // Update bunny killscore
    public void UpdateScore(int scoreToAdd)
    {
        score += scoreToAdd;
        scoreText.text = "Current score: " + score;

        // Update high score if beaten
        if (score > highScore)
        {
            highScore = score;
            highScoreText.text = "High Score: " + highScore;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
        }
    }

    // Start game
    public void StartGame(int difficulty)
    {
        isGameActive = true;
        score = 0;
        UpdateScore(0);

        // Hide title screen
        titleScreen.SetActive(false);

        // Adjust difficulty of pests
        if (pestSpawner != null)
            pestSpawner.AdjustDifficulty(difficulty);

        // Start growing all crops
        CropGrower.StartGrowingCrops();
    }

    // Check if there are veggies still on scene
    public void CheckGameOver()
    {
        GameObject[] veggies = GameObject.FindGameObjectsWithTag("Veggie");
        Debug.Log($"Veggies remaining: {veggies.Length}");

        if (veggies.Length == 0)
        {
            GameOver();
        }
    }

    // Stop any routines if game is over
    public void GameOver()
    {
        if (!isGameActive) return;

        gameOverText.gameObject.SetActive(true);
        isGameActive = false;

        // Stop pest spawning
        if (pestSpawner != null)
            pestSpawner.StopAllCoroutines();

        // Show restart button
        restartButton.gameObject.SetActive(true);

        Debug.Log("GAME OVER! All veggies have been eaten!");
        Debug.Log("Score: " + score);
    }

    // Restart :)
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Debug.Log("Restart Clicked!");
    }

    //Instructions Panel stuff
    public GameObject instructionsPanel; 

    //Instructions Button
    public void ShowInstructions()
    {
        if (instructionsPanel != null)
            instructionsPanel.SetActive(true);
    }

    //Exit Button inside the panel
    public void HideInstructions()
    {
        if (instructionsPanel != null)
            instructionsPanel.SetActive(false);
    }
}

