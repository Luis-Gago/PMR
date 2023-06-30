using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EMGDeviceManager : MonoBehaviour
{
    public static EMGDeviceManager instance;
    private SliderController sliderController;
    private SignalProcessing signalProcessing;

    private string selectedEMGDevice;
    private int selectedEmgDeviceChannel;
    public float EMGSignal;
    public float max_filtered; // Add the 'max_filtered' variable here
    public float min_filtered; // Add the 'min_filtered' variable here

    private void Start()
    {
        signalProcessing = FindObjectOfType<SignalProcessing>(); // Find the SignalProcessing script in the scene
        max_filtered = signalProcessing.max_filtered; // Assign the value of max_filtered from SignalProcessing.cs
        min_filtered = signalProcessing.min_filtered; // Assign the value of min_filtered from SignalProcessing.cs
    }

    private void Update()
    {
        // Use the 'max_filtered' variable as needed
        Debug.Log("Max Filtered Value: " + max_filtered);
        Debug.Log("Min Filtered Value: " + min_filtered);
    }

    public void EMGSignalObserver(float scaledEMGSignal) 
    {
        EMGSignal = scaledEMGSignal;
        Debug.Log("EMGSignalObserver " + EMGSignal);
    }

    public void SetSelectedDevice(string deviceAndChannel)
    {
        selectedEMGDevice = deviceAndChannel.Substring(0, 17);
        var temp = deviceAndChannel.Substring(23);
        selectedEmgDeviceChannel = System.Convert.ToInt32(temp);
    }

    public string GetSelectedDevice()
    {
        return selectedEMGDevice;
    }

    public int GetSelectedDeviceChannel()
    {
        return selectedEmgDeviceChannel;
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}




