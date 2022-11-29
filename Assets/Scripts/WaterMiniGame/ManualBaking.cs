using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ManualBaking : MonoBehaviour
{
    [SerializeField] NavMeshSurface[] navMeshSurface;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ButtonBaking();
    }

    void ButtonBaking()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            for (int i = 0; i < navMeshSurface.Length; i++)
            {
                navMeshSurface[i].BuildNavMesh();
            }
            Debug.Log("Build Done");
        }
    }
}
