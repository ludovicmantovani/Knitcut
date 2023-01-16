using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;

public class StartBacking : MonoBehaviour
{
    [SerializeField] private NavMeshSurface[] Path;
    [SerializeField] private GameObject boutonStart;
    
    public void BackingPath()
    {
        for (int i = 0; i < Path.Length; i++)
        {
            Path[i].BuildNavMesh();
        }
        boutonStart.SetActive(false);
    }
}
