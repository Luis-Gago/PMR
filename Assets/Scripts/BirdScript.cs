using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BirdScript : MonoBehaviour
{
    public Rigidbody2D myRigidbody;
    public LogicScript logic;
    public bool birdIsAlive = true;
    public Slider slider; // Reference to the slider

    private float sliderValue; // Stores the current slider value

    [SerializeField] private GameObject Bird;
    
    // Variables for smoothing the movement of the bird
    private float smoothPosition; // Smoothed position variable
    private float smoothSpeed = 0.1f; // Adjust this value to control the smoothing speed


    // Start is called before the first frame update
    void Start()
    {
        sliderValue = .5f; // Initialize the slider value to 1/2
        logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicScript>();
        slider.onValueChanged.AddListener(OnSliderValueChanged); // Register the event listener
    }

    // Update is called once per frame
    void Update()
    {     
        float viewportHeight = Camera.main.orthographicSize * 2f;
        float targetPosition = Mathf.Lerp(-viewportHeight / 2f, viewportHeight / 2f, sliderValue);

        // Apply smoothing using a low-pass filter
        smoothPosition = Mathf.Lerp(smoothPosition, targetPosition, smoothSpeed);

        Vector3 newObjectPosition = Bird.transform.position;
        newObjectPosition.y = smoothPosition;
        Bird.transform.position = newObjectPosition;

        //Send bird x and y coordinates to firestore
        AndroidBinding.Instance.setPlayerXCoordinate(Bird.transform.position.x);
        AndroidBinding.Instance.setPlayerYCoordinate(Bird.transform.position.y);
    }

    public void TriggerGameOver()
    {
        logic.gameOver();
        birdIsAlive = false;
    }

    private void OnSliderValueChanged(float value)
    {   
        sliderValue = value; // Update the slider value
    }
}




