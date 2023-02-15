using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Feeder : MonoBehaviour
{
    [Header("Feeder Timer")]
    [SerializeField] private List<GameObject> animalsToFeed;
    [SerializeField] private float timeBetweenFeeding = 10f;

    private bool feedingActive;

    [Header("Feeder Inventory")]
    [SerializeField] private bool canUseFeeder;
    [SerializeField] private bool feederInUse;
    [SerializeField] private GameObject feederInventory;
    [SerializeField] private GameObject interactionPanel;

    public bool CanUseFeeder
    {
        get { return canUseFeeder; }
        set { canUseFeeder = value; }
    }

    public List<GameObject> AnimalsToFeed
    {
        get { return animalsToFeed; }
        set { animalsToFeed = value; }
    }

    private void Start()
    {
        animalsToFeed = new List<GameObject>();
        feedingActive = false;

        canUseFeeder = false;
        feederInUse = false;

        interactionPanel.GetComponentInChildren<Text>().text = "Use E to open Feeder";
    }

    private void Update()
    {
        // Timer
        CheckAnimalsBeforeFeeding();

        // Inventory
        HandleFeederUse();
        HandleFeederInventory();
    }

    #region Handle Feeder Timer

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

    #endregion

    #region Feeder Inventory

    private void HandleFeederUse()
    {
        if (canUseFeeder)
        {
            interactionPanel.SetActive(true);
        }
        else
        {
            interactionPanel.SetActive(false);
        }
    }

    private void HandleFeederInventory()
    {
        feederInventory.SetActive(feederInUse);

        if (!canUseFeeder)
        {
            CloseFeederInventory();
            return;
        }

        if (Input.GetKeyDown(KeyCode.E) && canUseFeeder)
        {
            if (!feederInUse)
            {
                OpenFeederInventory();
            }
            else
            {
                CloseFeederInventory();
            }
        }
    }

    private void OpenFeederInventory()
    {
        feederInUse = true;

        interactionPanel.GetComponentInChildren<Text>().text = "Use E to close Feeder";
    }

    private void CloseFeederInventory()
    {
        feederInUse = false;

        interactionPanel.GetComponentInChildren<Text>().text = "Use E to open Feeder";
    }

    #endregion
}