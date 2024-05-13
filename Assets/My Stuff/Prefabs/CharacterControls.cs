﻿using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public struct GameData
{

	public int failCount;

    public Vector3 checkPoint;
    //public int health;
}

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]

public class CharacterControls : MonoBehaviour {

	GameData gameData;
	string filePath;
	const string FILE_NAME = "GameData.json";

	public CurrencyObject Prisms;

	public CanvasScript canvasScript;

    public Vector3 startPoint;

    public GuardNav guardScript;
    public GameObject guard;

    public float speed = 10.0f;
	public float airVelocity = 8f;
	public float gravity = 10.0f;
	public float maxVelocityChange = 10.0f;
	public float jumpHeight = 2.0f;
	public float maxFallSpeed = 20.0f;
	public float rotateSpeed = 25f; //Speed the player rotate
	private Vector3 moveDir;
	public GameObject cam;
	private Rigidbody rb;

	private float distToGround;

	private bool canMove = true; //If player is not hitted
	private bool isStuned = false;
	private bool wasStuned = false; //If player was stunned before get stunned another time
	private float pushForce;
	private Vector3 pushDir;


	public GameObject blockPrefab;

    GameObject[] blocks = new GameObject[2];

    //public Vector3 checkPoint;

    private bool slide = false;

	void  Start (){

        guard = GameObject.FindGameObjectWithTag("Guard");
        guardScript = guard.GetComponent<GuardNav>();

        distToGround = GetComponent<Collider>().bounds.extents.y;

		startPoint = transform.position;

        LoadCheckPoint();

    }

    bool IsGrounded (){
		return Physics.Raycast(transform.position, -Vector3.up, distToGround);
	}
	
	void Awake () {

        filePath = Application.persistentDataPath;

		//Debug.Log(filePath);

        gameData = new GameData();

        LoadData();

        rb = GetComponent<Rigidbody>();
		rb.freezeRotation = true;
		rb.useGravity = false;
	}
	
	void FixedUpdate () {
        Cursor.visible = false;

        if (canMove)
		{
			if (moveDir.x != 0 || moveDir.z != 0)
			{
				Vector3 targetDir = moveDir; //Direction of the character

				targetDir.y = 0;
				if (targetDir == Vector3.zero)
					targetDir = transform.forward;
				Quaternion tr = Quaternion.LookRotation(targetDir); //Rotation of the character to where it moves
				Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, Time.deltaTime * rotateSpeed); //Rotate the character little by little
				transform.rotation = targetRotation;
			}
            if (IsGrounded())
			{
			 // Calculate how fast we should be moving
				Vector3 targetVelocity = moveDir;
				targetVelocity *= speed;

				// Apply a force that attempts to reach our target velocity
				Vector3 velocity = rb.velocity;
				if (targetVelocity.magnitude < velocity.magnitude) //If I'm slowing down the character
				{
					targetVelocity = velocity;
					rb.velocity /= 1.1f;
				}
				Vector3 velocityChange = (targetVelocity - velocity);
				velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
				velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
				velocityChange.y = 0;
				if (!slide)
				{
					if (Mathf.Abs(rb.velocity.magnitude) < speed * 1.0f)
						rb.AddForce(velocityChange, ForceMode.VelocityChange);
				}
				else if (Mathf.Abs(rb.velocity.magnitude) < speed * 1.0f)
				{
					rb.AddForce(moveDir * 0.15f, ForceMode.VelocityChange);
					//Debug.Log(rb.velocity.magnitude);
				}

				// Jump
				if (IsGrounded() && Input.GetButton("Jump"))
				{
					rb.velocity = new Vector3(velocity.x, CalculateJumpVerticalSpeed(), velocity.z);
				}
            }
			else
			{
				if (!slide)
				{
					Vector3 targetVelocity = new Vector3(moveDir.x * airVelocity, rb.velocity.y, moveDir.z * airVelocity);
					Vector3 velocity = rb.velocity;
					Vector3 velocityChange = (targetVelocity - velocity);
					velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
					velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
					rb.AddForce(velocityChange, ForceMode.VelocityChange);
					if (velocity.y < -maxFallSpeed)
						rb.velocity = new Vector3(velocity.x, -maxFallSpeed, velocity.z);
                }
				else if (Mathf.Abs(rb.velocity.magnitude) < speed * 1.0f)
				{
					rb.AddForce(moveDir * 0.15f, ForceMode.VelocityChange);
				}

            }
		}
		else
		{
			rb.velocity = pushDir * pushForce;
		}
		// We apply gravity manually for more tuning control
		rb.AddForce(new Vector3(0, -gravity * GetComponent<Rigidbody>().mass, 0));
	}

	private void Update()
	{

		if (Input.GetKeyDown(KeyCode.R))
		{

			NewRun();

		}

		float h = Input.GetAxis("Horizontal");
		float v = Input.GetAxis("Vertical");

		Vector3 v2 = v * cam.transform.forward; //Vertical axis to which I want to move with respect to the camera
		Vector3 h2 = h * cam.transform.right; //Horizontal axis to which I want to move with respect to the camera
		moveDir = (v2 + h2).normalized; //Global position to which I want to move in magnitude 1

		RaycastHit hit;
		if (Physics.Raycast(transform.position, -Vector3.up, out hit, distToGround + 0.1f))
		{
			if (hit.transform.tag == "Slide")
			{
				slide = true;
			}
			else
			{
				slide = false;
			}
		}
	}

	float CalculateJumpVerticalSpeed () {
		// From the jump height and gravity we deduce the upwards speed 
		// for the character to reach at the apex.
		return Mathf.Sqrt(2 * jumpHeight * gravity);
	}

	public void HitPlayer(Vector3 velocityF, float time)
	{
		rb.velocity = velocityF;

		pushForce = velocityF.magnitude;
		pushDir = Vector3.Normalize(velocityF);
		StartCoroutine(Decrease(velocityF.magnitude, time));
	}

	public void SetCheckPoint()
	{

		gameData.checkPoint = transform.position;

		guardScript.SaveData();

	}

	public void LoadCheckPoint()
	{
		transform.position = gameData.checkPoint;

		guardScript.ResetPos(false);

	}

	private IEnumerator Decrease(float value, float duration)
	{
		if (isStuned)
			wasStuned = true;
		isStuned = true;
		canMove = false;

		float delta = 0;
		delta = value / duration;

		for (float t = 0; t < duration; t += Time.deltaTime)
		{
			yield return null;
			if (!slide) //Reduce the force if the ground isnt slide
			{
				pushForce = pushForce - Time.deltaTime * delta;
				pushForce = pushForce < 0 ? 0 : pushForce;
				//Debug.Log(pushForce);
			}
			rb.AddForce(new Vector3(0, -gravity * GetComponent<Rigidbody>().mass, 0)); //Add gravity
		}

		if (wasStuned)
		{
			wasStuned = false;
		}
		else
		{
			isStuned = false;
			canMove = true;
		}
	}

    public void NewRun()
	{

        for (int i = 0; i < blocks.Length; i++)
        {

			if (blocks[i] != null)
			{

                blocks[i].GetComponent<ShaderScript>().Nuke();

            }

        }


        blocks[0] = (GameObject)Instantiate(blockPrefab, new Vector3(10.0f, 2.5f, 1.1f), Quaternion.identity);
        blocks[1] = (GameObject)Instantiate(blockPrefab, new Vector3(10.0f, 2.5f, -1.1f), Quaternion.identity);

        blocks[0].GetComponent<ShaderScript>().Spawn();
        blocks[1].GetComponent<ShaderScript>().Spawn();

        gameData.failCount++;

		Prisms.value = 0;

        gameData.checkPoint = startPoint;

		guardScript.ResetPos(true);

		canvasScript.ResetTimer();

		LoadCheckPoint();

    }

	public void KillPlayer()
	{
		if (Prisms.value >= 1)
		{

            Prisms.value -= 1;

        }

        LoadCheckPoint();

    }

    public void SaveData()
	{

		string gameDataJson = JsonUtility.ToJson(gameData);

        File.WriteAllText(filePath + "/" + FILE_NAME, gameDataJson);

        Debug.Log("File created and saved");

    }

	public void LoadData()
	{

        if (File.Exists(filePath + "/" + FILE_NAME))
        {
            string loadedJson = File.ReadAllText(filePath+ "/" + FILE_NAME);
			gameData = JsonUtility.FromJson<GameData>(loadedJson);
            Debug.Log("File loaded successfully");
        }
        else
        {
			NewRun();
            Debug.Log("File not found");
        }

    }

    void OnApplicationQuit()
	{

		SaveData();

	}
}