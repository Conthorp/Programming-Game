using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeScript : MonoBehaviour
{

    public CharacterControls playerScript;
    public GameObject player;

    // Start is called before the first frame update
    void Start()
    {

        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<CharacterControls>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnTriggerStay(Collider collision)
    {
        if (collision.gameObject.tag == "Player")
        {

            playerScript.KillPlayer();

        }

    }
    

}
