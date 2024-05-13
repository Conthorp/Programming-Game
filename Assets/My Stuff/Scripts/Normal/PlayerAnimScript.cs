using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimScript : MonoBehaviour
{

    Rigidbody2D myrigidbody; //sets a variable called myrigidbody of type Rigidbody2D. not written as public so will be stored privately
    Animator anim; //sets a variable called anim of type Animator

    // Start is called before the first frame update
    void Start()
    {

        anim = GetComponentInChildren<Animator>(); //getcomponent Animator and assigns it to anim
        myrigidbody = GetComponent<Rigidbody2D>(); //getcomponent Rigidbody2D and assigns it to myrigidbody

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKey(KeyCode.W))
        {

            anim.SetBool("Move", true);

        }
        else
        {

            anim.SetBool("Move", false);

        }

        if (Input.GetKeyDown(KeyCode.Space))
        {

            anim.SetBool("Jump", true);

        }

        if (Input.GetKey(KeyCode.A))
        {

            anim.SetFloat("HAxisIdle", -1);
            anim.SetFloat("HAxisRun", -1);

        }
        else if (Input.GetKey(KeyCode.D))
        {

            anim.SetFloat("HAxisIdle", 1);
            anim.SetFloat("HAxisRun", 1);
        }
        else
        {

            anim.SetFloat("HAxisIdle", 0);
            anim.SetFloat("HAxisRun", 0);
        }


    }

    public void Jump()
    {

        anim.SetBool("Jump", false);

    }
}
