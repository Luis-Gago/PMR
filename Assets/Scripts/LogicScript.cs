using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LogicScript : MonoBehaviour
{
    public int playerScore;
    public Text scoreText;
    public GameObject gameOverScreen;
    public AudioSource ding;
    private bool isPausedGameOver = false;

    public void addScore(int scoreToAdd)
    {
        playerScore += scoreToAdd;
        scoreText.text = playerScore.ToString();
        ding.Play();
         // Set the player's normal score to playerScore
        AndroidBinding.Instance.SetPlayerNormalScore(playerScore);

        Scene scene = SceneManager.GetActiveScene();
        AndroidBinding.Instance.SetPlayerLevel(scene.name);
    }

    public void restartGame()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        Debug.Log("Current Scene: " + currentScene.name);
        // Reset the player's score to 0 and push to Android
        playerScore = 0;
        AndroidBinding.Instance.SetPlayerNormalScore(playerScore);
        // Get the build index of the current scene and restart it
        int sceneIndex = currentScene.buildIndex;
        StartCoroutine(SwitchToScene(sceneIndex, sceneIndex));
        PauseOnLoss();
    }

    public void gameOver()
    {
        gameOverScreen.SetActive(true);
        PauseOnLoss();
    }

    public void playGame()
    {
        SceneManager.LoadScene(1);
    }

    public void levelTwo()
    {
        StartCoroutine(SwitchToScene(1, 2));
    }

    public void levelThree()
    {
        StartCoroutine(SwitchToScene(2, 3));
    }

    public void quitGame()
    {
        Application.Quit();
    }

    private IEnumerator SwitchToScene(int sceneToUnload, int sceneToLoad)
    {
        yield return SceneManager.UnloadSceneAsync(sceneToUnload);
        SceneManager.LoadScene(sceneToLoad);
    }

    public void PauseOnLoss()
    {
        isPausedGameOver = !isPausedGameOver;

        if (isPausedGameOver)
        {
            Time.timeScale = 0; // Pause the game
        }
        else
        {
            Time.timeScale = 1; // Resume the game
        }
    }
}
