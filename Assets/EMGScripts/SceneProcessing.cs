using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Linq;

public class SceneProcessing : MonoBehaviour
{
    private float filtered_power = 0.0f;
    private float lpf = 0.05f;
    public float min_filtered = 5000.0f;
    public float max_filtered = 1.0f;
    private float maxDecay = 0.9999f;
    private float boost = 1.0f;
    public float active_max_filtered;
    private float last_pwr;
    private float emgScaled;
    private List<float> maxFilteredList = new List<float>();
    private int warmupSamples = 200;
    private int counterForInitialization = 0;
    private bool isPlaying = false;
    private bool receivedActiveMax = false;
    private bool isTrialBased = true;
    private bool isSensorConnected = false;
    private bool hasFlexed = true;

    [Serializable]
    public class FloatEvent : UnityEvent<float> { };
    public FloatEvent SendScaledEmgSignal;
    public FloatEvent updateEmgBoostFactorLog;
    public FloatEvent updateEMGRawLog;
    public FloatEvent updateEMGFilteredLog;
    public FloatEvent updateEMGMinFilteredLog;
    public FloatEvent updateEMGMaxFilteredLog;
    public FloatEvent updateEMGScaledLog;
    public FloatEvent updateLPFLog;

    public FloatEvent UpdateRawEmgUI;
    public FloatEvent UpdateFilteredEmgUI;
    public FloatEvent UpdateMaxFilteredUI;
    public FloatEvent UpdateMinFilteredUI;
    public FloatEvent UpdateEmgScaledUI;
    public FloatEvent UpdateLpfDisplayUI;
    public FloatEvent UpdateBoostTextUI;
    public FloatEvent UpdateMaxDecayToDisplayUI;
    public FloatEvent UpdateActiveMaxFilteredEmgUI;

    [Serializable]
    public class BoolEvent : UnityEvent<bool> { };
    public BoolEvent UpdateIsActiveMaxTrialBasedUI;

    public GameObject CalibrationPanel;

    enum SignalState { REMOVE_FIRST_SAMPLES, STREAM };
    SignalState signalState = SignalState.REMOVE_FIRST_SAMPLES;

    private bool isSetFilteredMinToMax = false;
    public GameObject flex;

    void Start()
    {
        StartGame();
        SetActiveMaxMode();
        updateEmgBoostFactorLog.Invoke(boost);
        Debug.Log("SceneProcessing.cs boost: " + boost);
        updateLPFLog.Invoke(lpf);
        Debug.Log("SceneProcessing.cs lpf: " + lpf);
        UpdateLpfDisplayUI.Invoke(lpf);
        UpdateBoostTextUI.Invoke(boost);
        UpdateMaxDecayToDisplayUI.Invoke(maxDecay);
        Debug.Log("SceneProcessing.cs maxDecay: " + maxDecay);
        UpdateIsActiveMaxTrialBasedUI.Invoke(isTrialBased);
        Debug.Log("SceneProcessing.cs isTrialBased: " + isTrialBased);
        flex.SetActive(false);
    }

    void FixedUpdate()
    {
        Debug.Log("SceneProcessing.cs isSensorConnected: " + isSensorConnected);
        if (isSensorConnected)
        {
            switch (signalState)
            {
                case SignalState.REMOVE_FIRST_SAMPLES:
                    if (counterForInitialization < warmupSamples) // throws away the first 100 vals to account for the initial power spike
                    {
                        counterForInitialization++;
                        last_pwr = 0;
                        Debug.Log("DEBUG LOG: SignalState.REMOVE_FIRST_SAMPLES" + counterForInitialization);
                    }
                    else
                    {
                        SetSignalStateToStream();
                        Debug.Log("DEBUG LOG: SetSignalState.STREAM");
                    }
                    break;

                case SignalState.STREAM:
                    Debug.Log("DEBUG LOG: SignalState.STREAM");
                    filtered_power = filtered_power * (1 - lpf) + last_pwr * lpf;
                    Debug.Log("SceneProcessing.cs filtered_power: " + filtered_power);
                    Debug.Log("SceneProcessing.cs last_pwr: " + last_pwr);
                    max_filtered = Mathf.Max(max_filtered, filtered_power) * maxDecay;

                    maxFilteredList.Add(max_filtered);
                    min_filtered = Mathf.Min(filtered_power, min_filtered) * 0.9999f + 0.0001f * (filtered_power - min_filtered);

                    // if the user hasn't started playing the game, then the user can still see the biofeedback of the spaceship
                    // once the user starts playing the game, then we use the active_max_filtered, which is updated every trial
                    if (isPlaying && receivedActiveMax)
                    {
                        if (isTrialBased)
                        {
                            emgScaled = ((filtered_power - min_filtered) / (active_max_filtered - min_filtered)) * boost;
                            Debug.Log("SceneProcessing.cs emgScaled in the if statement: " + emgScaled);
                        }
                        else
                        {
                            emgScaled = ((filtered_power - min_filtered) / (max_filtered - min_filtered)) * boost;
                            Debug.Log("SceneProcessing.cs emgScaled in the nested else statement: " + emgScaled);
                        }
                    }
                    else
                    {
                        emgScaled = Mathf.Clamp(((filtered_power - min_filtered) / (max_filtered - min_filtered)) * boost, 0.0f, 1.0f);
                        Debug.Log("SceneProcessing.cs emgScaled in the else statement: " + emgScaled);
                    }
                                  
                    flex.SetActive(hasFlexed);
                    if ((maxFilteredList.Count > 100) && !isSetFilteredMinToMax)
                    {
                        isSetFilteredMinToMax = true;
                        SetEmgFilteredMinToMax();
                        hasFlexed = false;
                    }
                    break;
            }
        }

        SendEmgSignalToGame(emgScaled);
        Debug.Log("SceneProcessing.cs emgScaled: " + emgScaled);
        UpdateRawEmgUI.Invoke(last_pwr);
        UpdateFilteredEmgUI.Invoke(filtered_power);
        UpdateMinFilteredUI.Invoke(min_filtered);
        Debug.Log("SceneProcessing.cs min_filtered: " + min_filtered);
        UpdateMaxFilteredUI.Invoke(max_filtered);
        Debug.Log("SceneProcessing.cs max_filtered: " + max_filtered);
        UpdateEmgScaledUI.Invoke(emgScaled);
        UpdateActiveMaxFilteredEmgUI.Invoke(active_max_filtered);
    }

    public void EmgRawPowerReceived(float pwr)
    {
        last_pwr = pwr;
        
    }

    public float ScaleInput(float x, float x_min, float x_max, float a, float b)
    {
        float result = a + ((x - x_min) * (b - a)) / (x_max - x_min);
        return result;
    }

    public void IncreaseLPF()
    {
        lpf += 0.01f;
        updateLPFLog.Invoke(lpf);
        UpdateLpfDisplayUI.Invoke(lpf);
    }

    public void DecreaseLPF()
    {
        lpf -= 0.01f;
        updateLPFLog.Invoke(lpf);
        UpdateLpfDisplayUI.Invoke(lpf);
    }

    public void IncreaseBoost()
    {
        boost += 0.1f;
        updateEmgBoostFactorLog.Invoke(boost);
        UpdateBoostTextUI.Invoke(boost);
    }

    public void DecreaseBoost()
    {
        if (boost > 0)
        {
            boost -= 0.1f;
        }
        updateEmgBoostFactorLog.Invoke(boost);
        UpdateBoostTextUI.Invoke(boost);
    }

    void SendEmgSignalToGame(float emgSignal)
    {
        SendScaledEmgSignal.Invoke(emgSignal);
        Debug.Log("SceneProcessing.cs emgSignal: " + emgSignal);
    }

    public void UpdateMaxFiltered()
    {
        StartCoroutine(GetLastMax());
    }
    
    IEnumerator GetLastMax()
    {
        if (maxFilteredList.Count > 0)
        {
            active_max_filtered = maxFilteredList.Max();
            maxFilteredList.Clear();
            receivedActiveMax = true;
        }
        yield return null;
    }

    public void IncreaseMaxDecay()
    {
        if (maxDecay < 1.0f)
        {
            maxDecay += 0.001f;
        }
        UpdateMaxDecayToDisplayUI.Invoke(maxDecay);
    }

    public void DecreaseMaxDecay()
    {
        maxDecay -= 0.001f;
        UpdateMaxDecayToDisplayUI.Invoke(maxDecay);
    }

    public void SetMaxDecayValue(float value)
    {
        maxDecay = value;
        UpdateMaxDecayToDisplayUI.Invoke(maxDecay);
    }

    public void StartGame()
    {
        isPlaying = true;
    }

    public void SetActiveMaxMode()
    {
        isTrialBased = !isTrialBased;
        UpdateIsActiveMaxTrialBasedUI.Invoke(isTrialBased);
    }

    public void UpdateEMGSignalTrialLog()
    {
        updateEMGRawLog.Invoke(last_pwr);
        updateEMGFilteredLog.Invoke(filtered_power);
        updateEMGMinFilteredLog.Invoke(min_filtered);
        updateEMGMaxFilteredLog.Invoke(max_filtered);
        updateEMGScaledLog.Invoke(emgScaled);
    }

    public void CalibrationPanelOff()
    {
        CalibrationPanel.SetActive(false);
    }

    public void CalibrationPanelOn()
    {
        CalibrationPanel.SetActive(true);
        CalibrationPanel.GetComponent<Image>().color = new Color32(43, 33, 240, 159);
    }

    public void CalibrationPanelReady()
    {
        CalibrationPanel.GetComponent<Image>().color = new Color32(43, 229, 33, 159);
    }

    public void SetEmgFilteredMinToMax()
    {
        min_filtered = max_filtered;
    }

    public void SetSensorStatusToConnected()
    {
        isSensorConnected = true;
        Debug.Log("SceneProcessing.cs isSensorConnected: " + isSensorConnected);
    }

    private void SetSignalStateToStream()
    {
        signalState = SignalState.STREAM;
    }

    public void ReduceFilteredMaxByAFactorOfTwo()
    {
        max_filtered = max_filtered / 2.0f;
    }
}

