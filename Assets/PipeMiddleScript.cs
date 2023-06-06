using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PipeMiddleScript : MonoBehaviour
{
    public LogicScript logic;
    private Scene currentScene; // Declare currentScene as a class-level variable

    // Start is called before the first frame update
    void Start()
    {
        logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicScript>();

         // Get the currently active scene
        currentScene = SceneManager.GetActiveScene();

        // Print the name of the current scene
        Debug.Log("Current Scene: " + currentScene.name);
    }

    // Update is called once per frame
    void Update()
    {
        if (logic.playerScore == 2 && currentScene.name == "Level1")
        {
            logic.levelTwo();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 3)
        {
            logic.addScore(1);
        }
        
    }
}
