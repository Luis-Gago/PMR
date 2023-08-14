using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    [Header("Component")]
    public TextMeshProUGUI timerText;

    [Header("Timer Settings")]
    public float currentTime;
    public bool countDown;

    [Header("Limit Settings")]
    public bool hasLimit;
    public float timerLimit;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (countDown)
        {
            currentTime -= Time.deltaTime;
        }
        else
        {
            currentTime += Time.deltaTime;
        }

        if (hasLimit && ((countDown && currentTime <= 0) || (!countDown && currentTime >= timerLimit)))
        {
            currentTime = timerLimit;
            timerText.color = Color.red;
            enabled = false;
        }
        else
        {
            setTimerText();
        }
    }



    private void setTimerText()
    {
        // Assuming currentTime is in seconds
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);

        string formattedTime = string.Format("{0:00}:{1:00}", minutes, seconds);
        timerText.text = formattedTime;

    }
}
