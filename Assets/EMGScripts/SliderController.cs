using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderController : MonoBehaviour
{
    private Slider slider;
    
    private void Start()
    {
        slider = GetComponent<Slider>();
    }

    private void Update()
    {
        SetValue(EMGDeviceManager.instance.EMGSignal);
        Debug.Log("EMGDeviceManager.instance.EMGSignal " + EMGDeviceManager.instance.EMGSignal);

    }

    public void SetValue(float value)
    {
        slider.value = value;
    }
}


