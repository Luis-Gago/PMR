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

    
    private void Start()
    {
       
    }

    private void Update()
    {

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



