using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{

    private void Update()
    {
        // Check for user input to toggle pause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            MainMenu();
        }
    }

    public void MainMenu()
    {
        // Load the main menu scene
        SceneManager.LoadScene("Main Menu");
    }
}
