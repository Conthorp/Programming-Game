using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointScript : MonoBehaviour
{

    public CharacterControls playerScript;
    public GameObject player;

    public GuardNav guardScript;
    public GameObject guard;

    // Start is called before the first frame update
    void Start()
    {

        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<CharacterControls>();

        guard = GameObject.FindGameObjectWithTag("Guard");
        guardScript = guard.GetComponent<GuardNav>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider collision)
    {
        if (collision.gameObject.tag == "Player")
        {

            playerScript.SetCheckPoint();

        }

    }

}
