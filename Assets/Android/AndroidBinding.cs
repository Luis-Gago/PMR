using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Linq;

public class AndroidBinding : MonoBehaviour
{
    AndroidJavaObject unityActivity;
    AndroidJavaObject bridge;
    
    [Serializable]
    public class FloatEvent : UnityEvent<float> { };
    public FloatEvent OnRawEMGPwrReceived;
    public FloatEvent OnBatteryVoltageReceived;

    [Serializable]
    public class Event : UnityEvent { };
    public Event SwitchToWaitForUserState;

    // private bool didPlayerCrashThisTrial = false;
    private int trialNumber;
    private int playerNormalScore;
    // private int playerBonusScore;
    // private int playerLife;
    // private float coinAltitude;
    // private float spaceshipCommandSignalAtCurvePeakPointCrossing;
    // private float curveAmplitude;
    // private float curveWidth;
    // private float curveSpeed;
    // private float spaceshipCommandSignalAtCrash;
    private float emgBoostFactor;
    // private float coinHeight;
    private float emgRaw;
    private float emgFiltered;
    private float emgMaxFiltered;
    private float emgMinFiltered;
    private float emgScaled;
    private float lpf;

    // device selection through Canvas(UI)
    private List<string> deviceList;
    private string selectedDevice;
    private string selectedEMGDevice;
    private int selectedEmgDeviceChannel;
    private double batteryVoltage;
    private string firmwareVersion;
    private string deviceToSave = "None";

    public static string _versionHash = null;

    public Text sensor;
    public GameObject bleDeviceMenu;
    public Dropdown dropdown;
    private float emgPwr;
    public static string GetVersionHash()
    {
        if (_versionHash == null)
        {
            _versionHash = "unknown";
            var gitHash = UnityEngine.Resources.Load<TextAsset>("GitHash");
            if (gitHash != null)
            {
                _versionHash = gitHash.text;
            }
        }
        return _versionHash;
    }

    void Start()
    {    
        ConnectService();
        selectedDevice = "No device is selected!";
    }

    void ConnectService() {
        // AndroidJavaClass is the Unity representation of a generic instance of java.lang.Class.
        // AndroidJavaClass Constructor - public AndroidJavaClass(string className); - Specifies the Java class name (e.g. java.lang.String).
        // Construct an AndroidJavaClass from the class name. This essentially means  
        // locate the class type and allocate a java.lang.Class object of that particular type.
        AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");

        // public FieldType GetStatic(string fieldName);
        // Parameters: fieldName --> The name of the field (e.g. <i>int counter;</i> would have fieldName = "counter").
        // Get the value of a static field in an object type. The generic parameter determines the field type.
        unityActivity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");
        //  
        bridge = new AndroidJavaObject("org.sralab.emgimu.unity_bindings.Bridge"); //
        // Triggers android to connect to the service and also pass the callback
        // object so the android bridge can pass in updates

        Debug.Log("Calling connectService");

        // Setup the parameters we want to send to our native plugin.
        object[] parameters = new object[2];
        parameters[0] = unityActivity;
        parameters[1] = new AndroidPluginCallback(this);
        if (bridge != null)
        {
            bridge.Call("connectService", parameters);
        }
    }

    IEnumerator OnResumeCoroutineWithDelay(float time)
    {
        yield return new WaitForSeconds(time);
        SendSelectedDeviceToAndroid(selectedDevice);

    }

    private void OnApplicationFocus(bool focus)
    {
        Debug.Log("OnApplicationFocus(), focus = " + focus + "deviceToSave = " + deviceToSave);
        if (deviceToSave != "None")
        {
            if (focus)
            {
                SwitchToWaitForUserState.Invoke();
                ConnectService();
                StartCoroutine(OnResumeCoroutineWithDelay(0.1f)); // wait for 100 ms for sensor to connect to service
            }
            else
            {
                object[] parameters = new object[1];
                parameters[0] = unityActivity;
                bridge.Call("disconnectService", parameters);
            }
        }

    }

    void OnApplicationQuit()
    {
        // Disconnect from service (this will also delete the callback
        // object handle)
        object[] parameters = new object[1];
        parameters[0] = unityActivity;
        if (bridge != null)
        {
            bridge.Call("disconnectService", parameters);
        }
    }

    public void Update()
    {
        if (emgPwr > 0)
        {
            OnRawEMGPwrReceived.Invoke(emgPwr);
            OnBatteryVoltageReceived.Invoke((float) batteryVoltage);
            sensor.text = selectedDevice;
        }
    }

    void emgPwrUpdated(int power)
    {
        emgPwr = power;
    }

    void batteryLifeUpdated(double voltage)
    {
        batteryVoltage = voltage;
    }

    void firmwareVersionUpdated(string version)
    {
        firmwareVersion = version;
    }

    void OnDeviceListReceived(string msg)
    {
        deviceList = CreateBleDeviceList(ParseDeviceListMsgAndClean(msg));
    }

    public void PopulateDropdown()
    {
        SetDropdownList(dropdown, deviceList);
    }


    public void SetDropdownList(Dropdown dropdown, List<string> myDeviceList)
    {
        print("called SetDropDownList");
        List<string> items = new List<string>();
        if (myDeviceList == null)
        {
            items.Add("No Devices");
        }
        else
        {
            items.Add(null);
            for (int i = 0; i < myDeviceList.Count; i++)
            {
                items.Add(myDeviceList[i]);
            }
        }
        dropdown.options.Clear();
        foreach (var item in items)
        {
            dropdown.options.Add(new Dropdown.OptionData() { text = item });
        }
        dropdown.Show(); // tells the dropdown to open 
        DropdownItemSelected(dropdown);
        dropdown.onValueChanged.AddListener(delegate { DropdownItemSelected(dropdown); });
    }

    void DropdownItemSelected(Dropdown dropdown)
    {
        int index = dropdown.value;
        if (index != 0)
        {
            SendSelectedDeviceToAndroid(dropdown.options[index].text);
            bleDeviceMenu.SetActive(false);
        }
    }

    string[] ParseDeviceListMsgAndClean(string msg)
    {
        string[] tempList = msg.Split(',');
        for (int i = 0; i < tempList.Length; i++)
        {
            tempList[i] = CleanString(tempList[i]);
        }
        return tempList;
    }

    private string CleanString(string dirtyString)
    {
        string myString;
        if (dirtyString.StartsWith("["))
        {
            myString = dirtyString.Split('[')[1];
            if (myString.EndsWith("]"))
            {
                myString = myString.Split(']')[0];
            }
        }

        else if (dirtyString.StartsWith(" "))
        {
            myString = dirtyString.Split(' ')[1];
            if (myString.EndsWith("]"))
            {
                myString = myString.Split(']')[0];
            }
        }
        else
        {
            myString = dirtyString;
        }

        return myString;
    }

    private List<string> CreateBleDeviceList(string[] myArr)
    {
        List<string> myList = new List<string>();
        foreach (string item in myArr)
        {
            for (int i = 0; i < 2; i++)
            {
                myList.Add(item + " - ch-" + (i + 1));
            }
        }
        return myList;
    }

    class AndroidPluginCallback : AndroidJavaProxy
    {
        private AndroidBinding _ab;

        public AndroidPluginCallback(AndroidBinding ab) : base("org.sralab.emgimu.unity_bindings.PluginCallback")
        {
            _ab = ab;
        }

        public void onSuccess(string msg) {
            bool success = int.TryParse(msg, out int power); 
            if (success)
                _ab.emgPwrUpdated(power);
        }
        public void onBatteryLife(string msg)
        {
            bool success = double.TryParse(msg, out double voltage); 
            if (success)
                _ab.batteryLifeUpdated(voltage);
        }

        public void onFirmwareVersion(string msg)
        {
            _ab.firmwareVersionUpdated(msg);
        }

        public void onError(string errorMessage)
        {
            Debug.Log("ENTER callback onError: " + errorMessage);
        }

        public void sendDeviceList(string msg)
        {
            _ab.OnDeviceListReceived(msg);
        }
    }

    [Serializable]
    private struct TrialInfo
    {
        public Time TrialEndTime;
        public String Timestamp;
        public int trialNumber;
        // public int playerLife;
        public int playerNormalScore;
        // public int playerBonusScore;
        // public float coinAltitude;
        // public float coinHeight;
        // public float curveAmplitude;
        // public float curveWidth;
        // public float curveSpeed;
        // public bool didPlayerCrashThisTrial;
        // public float spaceshipCommandSignalAtCrash;
        // public float spaceshipCommandSignalAtCurvePeakPointCrossing;
        // public float emgBoostFactor;
        public float lpf;
        public float emgRaw;
        public float emgFiltered;
        public float emgMaxFiltered;
        public float emgMinFiltered;
        public float emgScaled;
        public String sensorMACAddress;
        public int sensorChannel;
        public double batteryVoltage;
        public String firmwareVersion;
        public String softwareVersionHash;
    }

    public void OnTrialCompleted() {
        TrialInfo trial = new TrialInfo();
        trial.Timestamp = DateTime.Now.ToString("O");
        // trial.trialNumber = trialNumber;
        // trial.playerLife = playerLife;
        trial.playerNormalScore = playerNormalScore;
        // trial.playerBonusScore = playerBonusScore;
        // trial.coinAltitude = coinAltitude;
        // trial.coinHeight = coinHeight;
        // trial.curveAmplitude = curveAmplitude;
        // trial.curveWidth = curveWidth;
        // trial.curveSpeed = curveSpeed;
        // trial.didPlayerCrashThisTrial = didPlayerCrashThisTrial;
        // trial.spaceshipCommandSignalAtCrash = spaceshipCommandSignalAtCrash;
        // trial.emgBoostFactor = emgBoostFactor;
        trial.lpf = lpf;
        // trial.spaceshipCommandSignalAtCurvePeakPointCrossing = spaceshipCommandSignalAtCurvePeakPointCrossing;
        trial.emgRaw = emgRaw;
        trial.emgFiltered = emgFiltered;
        trial.emgMaxFiltered = emgMaxFiltered;
        trial.emgMinFiltered = emgMinFiltered;
        trial.emgScaled = emgScaled;
        trial.sensorMACAddress = selectedEMGDevice;
        trial.sensorChannel = selectedEmgDeviceChannel - 1; // to make the logs 0-indexed
        trial.batteryVoltage = batteryVoltage;
        trial.firmwareVersion = firmwareVersion;
        trial.softwareVersionHash = GetVersionHash();

        LogTrial(JsonUtility.ToJson(trial));
    }

    private void LogTrial(string trial) {
        ///Debug.Log("LogTrial: " + trial);

        object[] parameters = { trial };
        if (bridge != null)
        {
            bridge.Call("logTrial", parameters);  // put some code here to fail gracefully
        }
    }

    public void SendSelectedDeviceToAndroid(string selectedDeviceAndChannel)
    {
        object[] parameters = new object[1];
        selectedEMGDevice = selectedDeviceAndChannel.Substring(0, 17);
        var temp = selectedDeviceAndChannel.Substring(23);
        selectedEmgDeviceChannel = System.Convert.ToInt32(temp);
        SetSensorMac(selectedEMGDevice);
        SetSensorChannel(selectedEmgDeviceChannel);
        selectedDevice = selectedDeviceAndChannel;
        parameters[0] = selectedDevice;
        deviceToSave = selectedDevice;
        if (bridge != null)
        {
            bridge.Call("selectDevice", parameters);
        }
    }

    public void SetTrialNumber(int number)
    {
        trialNumber = number;
    }

    public void SetPlayerNormalScore(int normalScore)
    {
        playerNormalScore = normalScore;
    }

    // public void SetPlayerBonusScore(int bonusScore)
    // {
    //     playerBonusScore = bonusScore;
    // }

    // public void SetPlayerLife(int life)
    // {
    //     playerLife = life;
    // }

    // public void SetPlayerCrashTrialRecord(bool didCrash)
    // {
    //     didPlayerCrashThisTrial = didCrash;
    // }

    // public void SetPlayerAltitudeAtCrash(float crashAltitude)
    // {
    //     spaceshipCommandSignalAtCrash = crashAltitude;
    // }

    // public void SetCoinAltitude(float altitude)
    // {
    //     coinAltitude = altitude;
    // }

    // public void SetCoinPaddedHeight(float height)
    // {
    //     coinHeight = height;
    // }

    // public void SetSpaceshipCommandSignalAtCurvePeakPointCrossing(float verticalCommand)
    // {
    //     spaceshipCommandSignalAtCurvePeakPointCrossing = verticalCommand;
    // }

    // public void SetCurveAmplitude(float amplitude)
    // {
    //     curveAmplitude = amplitude;
    // }

    // public void SetCurveWidth(float width)
    // {
    //     curveWidth = width;
    // }

    // public void SetCurveSpeed(float speed)
    // {
    //     curveSpeed = speed;
    // }

    public void SetEMGBoost(float boost)
    {
        emgBoostFactor = boost;
    }

    public void SetLPF(float LPF)
    {
        lpf = LPF;
    }

    public void SetRawEMG(float rawEMG)
    {
        emgRaw = rawEMG;
    }
    public void SetFilteredEMG(float filteredEMG)
    {
        emgFiltered = filteredEMG;
    }
    public void SetMaxFilteredEMG(float filteredMaxEMG)
    {
        emgMaxFiltered = filteredMaxEMG;
    }
    public void SetMinFilteredEMG(float filteredMinEMG)
    {
        emgMinFiltered = filteredMinEMG;
    }

    public void SetScaledEMG(float scaledEMG)
    {
        emgScaled = scaledEMG;
    }

    private void SetSensorMac(String MAC)
    {
        selectedEMGDevice = MAC;
    }

    private void SetSensorChannel(int channel)
    {
        selectedEmgDeviceChannel = channel;
    }
}
