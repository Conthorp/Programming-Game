using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderScript : MonoBehaviour
{

    //Material outlineShader;

    public MeshRenderer myRenderer;

    public Material myShader;

    public bool show;

    public float spawnSpeedMultiplier = 1.0f;

    public float myCutoff = -0.6f;

    bool destroy;

    // Start is called before the first frame update
    private void Start()
    {

        myRenderer = GetComponent<MeshRenderer>();

        myShader = Instantiate(myRenderer.sharedMaterial);

        myRenderer.materials[0] = myShader;

        myShader = myRenderer.materials[0];

        myShader.SetFloat("_Cutoff_Height", myCutoff);

    }

    // Update is called once per frame
    private void FixedUpdate()
    {

        if (show)
        {

            myCutoff += (0.015f * spawnSpeedMultiplier);

        }

        if (!show)
        {

            myCutoff -= (0.015f * spawnSpeedMultiplier);

            if (destroy && myCutoff <= -0.6f)
            {

                Destroy(this.gameObject);

            }

        }

        myCutoff = Mathf.Clamp(myCutoff, -0.6f, 0.85f);

        myShader.SetFloat("_Cutoff_Height", myCutoff);

    }

    public void Spawn()
    {

        show = true;

    }

    public void Hide()
    {

        show = false;

    }

    public void Despawn()
    {

        show = false;

        destroy = true;

    }

    public void Nuke()
    {

        Destroy(this.gameObject);

    }
}
