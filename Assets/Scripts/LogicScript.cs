// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;
// using UnityEngine.SceneManagement;

// public class LogicScript : MonoBehaviour
// {
//     public int playerScore;
//     public Text scoreText;
//     public GameObject gameOverScreen;
//     public AudioSource ding;
    
//     public void addScore(int scoreToAdd)
//     {
//         playerScore += scoreToAdd;
//         scoreText.text = playerScore.ToString();
//         ding.Play();
//     }

//     public void restartGame()
//     {
//         Scene currentScene = SceneManager.GetActiveScene();

//         Debug.Log("Current Scene: " + currentScene.name);

//         if (currentScene.name == "Level1")
//         {
//             SceneManager.UnloadSceneAsync(1);
//             SceneManager.LoadScene(1, LoadSceneMode.Additive);
//         }
//         else if (currentScene.name == "Level2")
//         {
//             SceneManager.UnloadSceneAsync(2);
//             SceneManager.LoadScene(2, LoadSceneMode.Additive);
//         }
//         else if (currentScene.name == "Level3")
//         {
//             SceneManager.UnloadSceneAsync(3);
//             SceneManager.LoadScene(3, LoadSceneMode.Additive);
//         }

//         // SceneManager.LoadScene(SceneManager.GetActiveScene().name);
//     }

//     public void gameOver()
//     {
//         gameOverScreen.SetActive(true);
//     }

//     public void playGame()
//     {
//         SceneManager.LoadScene(1, LoadSceneMode.Additive);
//     }

//     public void levelTwo()
//     {
//         SceneManager.UnloadSceneAsync(1);
//         SceneManager.LoadScene(2, LoadSceneMode.Additive);
//     }

//     public void levelThree()
//     {
//         SceneManager.UnloadSceneAsync(2);
//         SceneManager.LoadScene(3, LoadSceneMode.Additive);
//     }

//     public void quitGame()
//     {
//         Application.Quit();
//     }
// }

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

    public void addScore(int scoreToAdd)
    {
        playerScore += scoreToAdd;
        scoreText.text = playerScore.ToString();
        ding.Play();
         // Set the player's normal score to playerScore
        AndroidBinding.Instance.SetPlayerNormalScore(playerScore);
    }

    public void restartGame()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        Debug.Log("Current Scene: " + currentScene.name);

        // Get the build index of the current scene and restart it
        int sceneIndex = currentScene.buildIndex;
        StartCoroutine(SwitchToScene(sceneIndex, sceneIndex));
    }

    public void gameOver()
    {
        gameOverScreen.SetActive(true);
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
}
