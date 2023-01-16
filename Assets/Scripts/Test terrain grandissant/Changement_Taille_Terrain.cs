using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Changement_Taille_Terrain : MonoBehaviour
{
    public GameObject enclo1;
    public GameObject enclo2;
    private bool Enclos = false;
    //public NavMeshSurface[] NMS;
    void Start()
    {

        //NMS[0].BuildNavMesh();

    }

    void Update()
    {
        testTaille();
    }

    void testTaille()
    {
        if (Input.GetKeyDown(KeyCode.S) && Enclos == false)
        {
            enclo1.SetActive(false);
            enclo2.SetActive(true);
            //NMS[1].BuildNavMesh();
            Enclos = true;
        }
        /*if (Input.GetKeyDown(KeyCode.S) && Enclos == true)
        {
            enclo1.SetActive(true);
            enclo2.SetActive(false);
            NMS[0].BuildNavMesh();
            Enclos = true;
        }*/
    }
}
