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
    public Button restartButton;
    public GameObject titleScreen;

    [Header("Gameplay")]
    public PestSpawner pestSpawner;

    private int highScore;
    private int score;
    public bool isGameActive;

    private void Start()
    {
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        highScoreText.text = "High Score: " + highScore;
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
        isGameActive = true;
        score = 0;
        UpdateScore(0);
        titleScreen.SetActive(false);

        if (pestSpawner != null)
            pestSpawner.AdjustDifficulty(difficulty);

        CropGrower.StartGrowingCrops(); // triggers all veggies to grow
    }


    public void CheckGameOver()
    {
        // Find all remaining veggies
        GameObject[] veggies = GameObject.FindGameObjectsWithTag("Veggie");

        if (veggies.Length == 0)
        {
            GameOver();
        }
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
        Debug.Log("Score:" + score);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Debug.Log("Restart Clicked!");
    }
}
