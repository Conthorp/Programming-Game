using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockScript : MonoBehaviour
{
    public ShaderScript myShaderScript;

    public bool hit;

    public bool despawned;

    public float hitTimer = 0.0f;

    void Awake()
    {


    }

    void Update()
    {

        if (hit && !despawned)
        {

            if (hitTimer < 2)
            {

                hitTimer += Time.unscaledDeltaTime;

            }
            else
            {

                despawned = true;

                Despawn();
                
            }

        }

    }

    void FixedUpdate()
    {


    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "PlayerMask")
        {

            hit = true;

        }

    }

    void Despawn()
    {

        myShaderScript.Despawn();
    }

}
