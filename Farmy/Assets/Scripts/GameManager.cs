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

    //Keep highscore here
    private void Start()
    {
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        highScoreText.text = "High Score: " + highScore;
    }

    //Update bunny killscore
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

    //Start game
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


    //Check there are veggies still on scene
    public void CheckGameOver()
    {
        GameObject[] veggies = GameObject.FindGameObjectsWithTag("Veggie");

        if (veggies.Length == 0)
        {
            GameOver();
        }
    }

    //Stop any routines if game is over
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

    //Restart :)
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Debug.Log("Restart Clicked!");
    }
}
