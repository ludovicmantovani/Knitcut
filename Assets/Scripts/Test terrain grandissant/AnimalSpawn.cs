using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalSpawn : MonoBehaviour
{
    public float TimerSpawn = 40;
    public GameObject[] Animals;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        TimerSpawn = TimerSpawn - Time.deltaTime;
        Spawn();
    }
    void Spawn()
    {
        if(TimerSpawn <= 0)
        {
            (Instantiate(Animals[0]) as GameObject).transform.parent = this.transform;
            TimerSpawn = 25;
        }
    }
}
