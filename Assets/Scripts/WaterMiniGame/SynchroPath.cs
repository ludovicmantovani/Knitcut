using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SynchroPath : MonoBehaviour
{
    public NavMeshSurface[] NMS;
    MovePiece MP;

    // Start is called before the first frame update
    void Start()
    {
        MP = FindObjectOfType<MovePiece>();

        // Build navmesh path
        for (int i = 0; i < NMS.Length; i++)
        {
            NMS[i].BuildNavMesh();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        SwitchVerification();
    }
    void SwitchVerification()
    {
        if(MP.switchOn == true)
        {
            // Rebuild the navmesh path every time a switch happen
            for (int i = 0; i < NMS.Length; i++)
            {
                NMS[i].BuildNavMesh();
            }

            MP.switchOn = false;
        }
    }
}
