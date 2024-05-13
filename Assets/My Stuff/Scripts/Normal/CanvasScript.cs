using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using TMPro;

[Serializable]
public struct TimerData
{
    public float timer;
    //public int health;
}

public class CanvasScript : MonoBehaviour
{
    TimerData timerData;
    string filePath;
    const string FILE_NAME = "TimerData.json";

    public CurrencyObject Prisms;
    public CharacterControls characterControls;

    public TMP_Text prismCount, timer;

    // Start is called before the first frame update
    void Awake()
    {

        filePath = Application.persistentDataPath;
        timerData = new TimerData();

        LoadData();

    }

    // Update is called once per frame
    void Update()
    {

        timerData.timer -= Time.unscaledDeltaTime;

        if (timerData.timer <= 0.0f)
        {

            ResetTimer();
            characterControls.NewRun();

        }

        prismCount.text = Prisms.value.ToString();
        timer.text = timerData.timer.ToString();

    }

    public void SaveData()
    {

        string TimerDataJson = JsonUtility.ToJson(timerData);

        File.WriteAllText(filePath + "/" + FILE_NAME, TimerDataJson);

        Debug.Log("File created and saved");

    }

    public void LoadData()
    {

        if (File.Exists(filePath + "/" + FILE_NAME))
        {
            string loadedJson = File.ReadAllText(filePath + "/" + FILE_NAME);
            timerData = JsonUtility.FromJson<TimerData>(loadedJson);
            Debug.Log("File loaded successfully");
        }
        else
        {
            ResetTimer();
            Debug.Log("File not found");
        }

    }

    public void ResetTimer()
    {

        timerData.timer = 30.0f;

    }

    void OnApplicationQuit()
    {

        SaveData();

    }
}
