using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EMGDeviceManager : MonoBehaviour
{
    public static EMGDeviceManager instance;

    private string selectedEMGDevice;
    private int selectedEmgDeviceChannel;
    private SliderController sliderController;
    private float emgPwr;

    private void Start()
    {
        sliderController = FindObjectOfType<SliderController>();
    }

    private void Update()
    {
        // // Use emgPwr to determine slider value
        // float sliderValue = emgPwr; // Modify this line based on your desired logic
        // sliderController.SetValue(sliderValue);
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



