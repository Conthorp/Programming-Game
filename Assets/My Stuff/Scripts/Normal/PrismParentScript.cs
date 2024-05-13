using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrismParentScript : MonoBehaviour
{

    public GameObject prism;
    public PrismScript prismScript;

    // Start is called before the first frame update
    void Start()
    {



    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Hide()
    {

        prism.SetActive(false);

    }

    public void Show()
    {

        prism.SetActive(true);

    }
}
