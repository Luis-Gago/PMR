using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BirdScript : MonoBehaviour
{
    public Rigidbody2D myRigidbody;
    public float flapStrength;
    public LogicScript logic;
    public bool birdIsAlive = true;
    public Slider slider; // Reference to the slider

    private float sliderValue; // Stores the current slider value

    // Start is called before the first frame update
    void Start()
    {
        logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicScript>();
        slider.onValueChanged.AddListener(OnSliderValueChanged); // Register the event listener
    }

    // Update is called once per frame
    void Update()
    {
        if (sliderValue != 0 && birdIsAlive)
        {
            myRigidbody.velocity = Vector2.up * ((flapStrength * sliderValue)); // Calculate velocity with slider value
            sliderValue = 0f; // Reset the slider value to zero
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        logic.gameOver();
        birdIsAlive = false;
    }

    private void OnSliderValueChanged(float value)
    {
        sliderValue = value; // Update the slider value
    }
}




