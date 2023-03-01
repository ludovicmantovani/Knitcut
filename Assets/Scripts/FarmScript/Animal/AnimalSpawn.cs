using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalSpawn : MonoBehaviour
{
    public float TimerSpawn = 25;
    public GameObject[] Animals;
    public GameObject[] Spawns;
    private int animalNumber;
    private int spawnNumber;
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
        

        if (TimerSpawn <= 0)
        {
            AnimalRandomSpawn();
            Instantiate(Animals[animalNumber], Spawns[spawnNumber].transform);
            TimerSpawn = 25;
        }
    }
    void AnimalRandomSpawn()
    {
        animalNumber = Random.Range(0, 5);
        spawnNumber = Random.Range(0, 6);
    }
}
