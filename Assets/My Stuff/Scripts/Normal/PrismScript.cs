using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrismScript : MonoBehaviour
{

    public CurrencyObject Prisms;

    public GameObject prismParent;
    public PrismParentScript prismParentScript;

    // Start is called before the first frame update
    void Start()
    {

        // Create mesh filter using GetComponent<meshfilter>
        MeshFilter mf = GetComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        mf.mesh = mesh;

        // Vertices
        Vector3[] vertices = new Vector3[5] {new Vector3(0, 1, 0), new Vector3(1, 0, 1), new Vector3(-1, 0, 1), new Vector3(1, 0, -1), new Vector3(-1, 0, -1)};

        // Triangles
        int[] triangles = new int[21];
        triangles[0] = 0;
        triangles[1] = 2;
        triangles[2] = 1;

        triangles[3] = 0;
        triangles[4] = 1;
        triangles[5] = 3;

        triangles[6] = 0;
        triangles[7] = 4;
        triangles[8] = 2;

        triangles[9] = 0;
        triangles[10] = 3;
        triangles[11] = 4;

        triangles[12] = 0;
        triangles[13] = 3;
        triangles[14] = 1;

        triangles[15] = 1;
        triangles[16] = 2;
        triangles[17] = 3;

        triangles[18] = 2;
        triangles[19] = 4;
        triangles[20] = 3;

        Vector3[] normals = mesh.normals;

        //normals[0] *= -1;
        //normals[2] *= -1;
        //normals[4] *= -1;

        // Update mesh with vertices, triangles and normals
        mesh.vertices = vertices;
        mesh.triangles = triangles;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider collision)
    {
        if (collision.gameObject.tag == "Player")
        {

            Prisms.value++;

            prismParentScript.Hide();

        }

    }

}
