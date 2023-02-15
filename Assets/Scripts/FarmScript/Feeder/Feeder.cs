using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Feeder : MonoBehaviour
{
    [SerializeField] private List<GameObject> animalsToFeed;
    [SerializeField] private float timeBetweenFeeding = 10f;

    private bool feedingActive;

    public List<GameObject> AnimalsToFeed
    {
        get { return animalsToFeed; }
        set { animalsToFeed = value; }
    }

    private void Start()
    {
        animalsToFeed = new List<GameObject>();
        feedingActive = false;
    }

    private void Update()
    {
        CheckAnimalsBeforeFeeding();
    }

    private void CheckAnimalsBeforeFeeding()
    {
        CleanAnimals();

        if (animalsToFeed.Count == 0 || feedingActive) return;

        StartCoroutine(FeedAnimalsEveryXTime());
    }

    private void CleanAnimals()
    {
        for (int i = 0; i < animalsToFeed.Count; i++)
        {
            if (animalsToFeed[i] == null) animalsToFeed.Remove(animalsToFeed[i]);
        }
    }

    private IEnumerator FeedAnimalsEveryXTime()
    {
        feedingActive = true;

        while (animalsToFeed.Count > 0)
        {
            for (int i = 0; i < animalsToFeed.Count; i++)
            {
                Debug.Log($"Feed '{animalsToFeed[i].name}' and wait {timeBetweenFeeding}s...");
            }

            yield return new WaitForSeconds(timeBetweenFeeding);
        }

        feedingActive = false;
    }
}