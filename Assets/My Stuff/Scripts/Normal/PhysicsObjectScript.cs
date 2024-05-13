using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public struct ObjectData
{

    public Vector3 position;
    public Quaternion rotation;

}

public class PhysicsObjectScript : MonoBehaviour
{

    ObjectData objectData;
    string filePath;
    const string FILE_NAME = "ObjectData.json";

    public Vector3 startPosition;
    public Quaternion startRotation;

    // Start is called before the first frame update
    void Awake()
    {

        filePath = Application.persistentDataPath;
        objectData = new ObjectData();

        LoadData();

        transform.position = objectData.position;
        transform.rotation = objectData.rotation;

    }

    // Update is called once per frame
    void Update()
    {
        


    }

    public void LoadPosition()
    {

        transform.position = objectData.position;
        transform.rotation = objectData.rotation;

    }

    public void ResetPosition()
    {

        transform.position = startPosition;
        transform.rotation = startRotation;

        objectData.position = startPosition;
        objectData.rotation = startRotation;

        SaveData();

    }

    public void SaveData()
    {

        string objectDataJson = JsonUtility.ToJson(objectData);

        File.WriteAllText(filePath + "/" + FILE_NAME, objectDataJson);

        Debug.Log("File created and saved");

    }

    public void LoadData()
    {

        if (File.Exists(filePath + "/" + FILE_NAME))
        {
            string loadedJson = File.ReadAllText(filePath + "/" + FILE_NAME);
            objectData = JsonUtility.FromJson<ObjectData>(loadedJson);
            Debug.Log("File loaded successfully");
        }
        else
        {
            ResetPosition();
            Debug.Log("File not found");
        }

    }

    void OnApplicationQuit()
    {

        SaveData();

    }

}
