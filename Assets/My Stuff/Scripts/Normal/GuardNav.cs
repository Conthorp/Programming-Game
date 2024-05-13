using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public struct GuardData
{

    public GameObject targetRecord;
    public Vector3 sourceRecord;

    public GameObject currentTarget;
    public Vector3 source;
    //public int health;
}

public class GuardNav : MonoBehaviour
{

    GuardData guardData;
    string filePath;
    const string FILE_NAME = "GuardData.json";

    //public GameObject targetRecord;

    //public GameObject currentTarget;

    // Declare a public variable to reference the Main Camera
    public GameObject WaypointPlat;
    UnityEngine.AI.NavMeshAgent navAgent;
    public const float PROXIMITY_DISTANCE = 1.0f;
    public GameObject player;
    private WaypointManager waypointManager;
    const float DECELERATION_FACTOR = 1.5f;
    // Now variables needed by FixedUpdate

    public Vector3 target;
    public Vector3 startPoint;

    Vector3 outputVelocity;
    // And arrive
    Vector3 directionToTarget;
    Vector3 velocityToTarget;
    Vector3 dirDiff, dirChange;

    public CharacterControls playerScript;

    public float fieldOfViewAngle = 360.0f;

    float distanceToTarget;
    public float speed;

    public int randomIdle;

    public bool isIdle, isPatrolling, isDistracted, isSeeking;

    public bool isVisible, isAudible, isClose;

    public float idleWait;

    float idleWaitRemaining;

    // Use this for initialisation
    void Awake()
    {

        filePath = Application.persistentDataPath;
        guardData = new GuardData();

        //SaveData();
        LoadData();

        // Get the WaypointManager from the camera and then the first object
        waypointManager = WaypointPlat.GetComponent<WaypointManager>();
        navAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();

        isPatrolling = true;

        //guardData.currentTarget = waypointManager.NextWaypoint(null);

        playerScript = player.GetComponent<CharacterControls>();

        transform.position = guardData.source;
    }

    void FixedUpdate()
    {
        if (guardData.currentTarget == null)
        {

            guardData.currentTarget = waypointManager.NextWaypoint(null);
            guardData.targetRecord = guardData.currentTarget;

        }

        guardData.source = transform.position;
        target = guardData.currentTarget.transform.position;



        isPlayerVisible();

        isPlayerClose();

        outputVelocity = Arrive(guardData.source, target);
        GetComponent<Rigidbody>().AddForce(outputVelocity, ForceMode.VelocityChange);
        // Check the distance from object to target, and make query
        // When it moves within the PROXIMITY_DISTANCE

        if (!isVisible && !isClose && !isAudible)
        {

            setPatrolling();


        }

        if (isIdle)
        {

            idleWaitRemaining -= Time.fixedDeltaTime;

            if (idleWaitRemaining <= 0 )
            {

                setPatrolling();

            }

        }

        if (Vector3.Distance(guardData.source, target) < PROXIMITY_DISTANCE && (isPatrolling || isDistracted))
        {

            randomIdle = (UnityEngine.Random.Range(0, 2));

            if (randomIdle == 0)
            {

                guardData.currentTarget = waypointManager.NextWaypoint(guardData.currentTarget);

            }
            else
            {

                setIdle();

                idleWaitRemaining = idleWait;

                guardData.currentTarget = waypointManager.NextWaypoint(guardData.currentTarget);

            }

        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {

            playerScript.KillPlayer();

        }

    }

    public void setIdle()
    {

        isIdle = true;

        isPatrolling = false;

        isDistracted = false;

        isSeeking = false;

    }

    public void setPatrolling()
    {

        if (isSeeking || isDistracted)
        {

            guardData.currentTarget = waypointManager.NextWaypoint(null);

        }

        isIdle = false;

        isPatrolling = true;

        isDistracted = false;

        isSeeking = false;

    }

    public void setDistracted()
    {

        isIdle = false;

        isPatrolling = false;

        isDistracted = true;

        isSeeking = false;

    }

    public void setSeeking()
    {
        guardData.currentTarget = player;

        isIdle = false;

        isPatrolling = false;

        isDistracted = false;

        isSeeking = true;

    }

    public void isPlayerVisible()
    {

        // Create a vector from the enemy to the player and store the angle between it and forward.
        Vector3 direction = player.transform.position - transform.position;
        float angle = Vector3.Angle(direction, transform.forward);

        // Create NavMesh hit var
        UnityEngine.AI.NavMeshHit hit;

        // If the Ray cast hits something other than the target, then true is returned, if not false
        // So !hit is used to specify visibility and...
        // If the angle between forward and where the player is, is less than half the angle of view...
        if (!navAgent.Raycast(player.transform.position, out hit) && angle < fieldOfViewAngle * 0.25f)
        {
            // ... the player is Visible
            isVisible = true;

            setSeeking();

        }
        else
        {

            if (isVisible)
            {

                setPatrolling();

                guardData.currentTarget = waypointManager.NextWaypoint(null);

            }
            // ... the player is Not Visible
            isVisible = false;

            // Update Close Text on Canvas
        }
    }

    public void isPlayerClose()
    {
        // If direct distance < 20, then audible
        if (Vector3.Distance(transform.position, player.transform.position) < 10.0f)
        {

            isClose = true;

            setSeeking();

        }
        else
        {
            // Is not Audible
            isClose = false;

        }
    }

    // Arrive function
    private Vector3 Arrive(Vector3 source, Vector3 target)
    {
        
        distanceToTarget = Vector3.Distance(source, target);
        directionToTarget = Vector3.Normalize(target - source);


        if (isPatrolling || isDistracted)
        {

            transform.LookAt(target);

            speed = Mathf.Clamp((distanceToTarget / DECELERATION_FACTOR), 3, 5);

            velocityToTarget = speed * transform.forward;

        }
        else if (isSeeking)
        {

            transform.LookAt(target);

            speed = 6;

            velocityToTarget = speed * transform.forward;

        }
        else if (isIdle)
        {

            speed = 0;

            velocityToTarget = speed * transform.forward;

            transform.Rotate(0.0f, 1.0f, 0.0f, Space.Self);

        }

        return velocityToTarget - GetComponent<Rigidbody>().velocity;

    }

    public void ResetPos(bool newRun)
    {

        if (newRun)
        {

            guardData.source = startPoint;
            guardData.sourceRecord = startPoint;

            transform.position = startPoint;

            guardData.targetRecord = waypointManager.NextWaypoint(null);
            guardData.currentTarget = waypointManager.NextWaypoint(null);

        }
        else
        {

            transform.position = guardData.sourceRecord;
            guardData.currentTarget = guardData.targetRecord;

        }

    }

    public void SaveData()
    {

        guardData.sourceRecord = guardData.source;
        guardData.targetRecord = guardData.currentTarget;

        string guardDataJson = JsonUtility.ToJson(guardData);

        File.WriteAllText(filePath + "/" + FILE_NAME, guardDataJson);

        Debug.Log("File created and saved");

    }

    public void LoadData()
    {

        if (File.Exists(filePath + "/" + FILE_NAME))
        {
            string loadedJson = File.ReadAllText(filePath + "/" + FILE_NAME);
            guardData = JsonUtility.FromJson<GuardData>(loadedJson);
            Debug.Log("File loaded successfully");
        }
        else
        {
            ResetPos(true);
            Debug.Log("File not found");
        }

    }

    void OnApplicationQuit()
    {

        SaveData();

    }
}
