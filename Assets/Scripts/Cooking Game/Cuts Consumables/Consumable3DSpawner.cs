using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Consumable3DSpawner : MonoBehaviour
{
    public GameObject[] consumablesToSpawn;
    public Transform[] spawnPoints;

    public float minDelay = 0.1f;
    public float maxDelay = 1f;

    public float delayBeforeStartSpawning = 2f;
    public float delayBeforeDestroyingConsumableObject = 5f;

    public bool canSpawnConsumables;

    private bool spawning;

    Cooking cooking;

    void Start()
    {
        cooking = FindObjectOfType<Cooking>();

        spawning = false;
    }

    void Update()
    {
        if (!spawning && canSpawnConsumables)
        {
            spawning = true;

            StartCoroutine(SpawnConsumables());
        }
    }

    IEnumerator SpawnConsumables()
    {
        yield return new WaitForSeconds(delayBeforeStartSpawning);

        while (consumablesToSpawn.Length > 0)
        {
            // Random delay between limits
            float delay = Random.Range(minDelay, maxDelay);
            yield return new WaitForSeconds(delay);

            // Get random spawn point in list
            int spawnIndex = Random.Range(0, spawnPoints.Length);
            Transform spawnPoint = spawnPoints[spawnIndex];

            // Get random consumable 3D in list & Instantiate it
            int randomConsumableIndex = Random.Range(0, consumablesToSpawn.Length);

            if (consumablesToSpawn.Length == 0) yield break;

            GameObject spawnedConsumable = Instantiate(consumablesToSpawn[randomConsumableIndex], spawnPoint.position, spawnPoint.rotation);
            spawnedConsumable.transform.SetParent(transform);

            // Temporary convert array to list to remove random consumable 3D and convert list to array
            var temporary = new List<GameObject>(consumablesToSpawn);
            temporary.RemoveAt(randomConsumableIndex);

            consumablesToSpawn = temporary.ToArray();

            // Destroy consumable 3D after specific delay
            Destroy(spawnedConsumable, delayBeforeDestroyingConsumableObject);
        }

        spawning = false;

        yield return new WaitForSeconds(3f);

        cooking.HandleFinalProduct();
    }

    public void AddConsumablesToSpawn(GameObject[] consumables3D)
    {
        consumablesToSpawn = consumables3D;
        canSpawnConsumables = true;

        if (!spawning && canSpawnConsumables)
        {
            spawning = true;
            canSpawnConsumables = false;
            StartCoroutine(SpawnConsumables());
        }
    }
}