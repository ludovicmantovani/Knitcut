using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Feeder : MonoBehaviour
{
    //[Header("Feeder Timer")]
    //[SerializeField] private List<GameObject> animalsToFeed;
    //[SerializeField] private float timeBetweenFeeding = 10f;

    //private bool feedingActive;

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

    /*public List<GameObject> AnimalsToFeed
    {
        get { return animalsToFeed; }
        set { animalsToFeed = value; }
    }*/

    private void Start()
    {
        //animalsToFeed = new List<GameObject>();
        //feedingActive = false;

        canUseFeeder = false;
        feederInUse = false;

        interactionPanel.GetComponentInChildren<Text>().text = "Use E to open Feeder";
    }

    private void Update()
    {
        // Timer
        //CheckAnimalsBeforeFeeding();
        //HandleFeederContent();

        // Inventory
        HandleFeederUse();
        HandleFeederInventory();
    }

    public Item GetFood()
    {
        if (IsFeederEmpty()) return null;

        return GetItem();
    }

    #region Feeder Inventory

    #region Inventory UI

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

    #region Inventory Content

    private Item GetItem()
    {
        for (int i = 0; i < feederInventory.transform.childCount; i++)
        {
            Transform slot = feederInventory.transform.GetChild(i);

            // If item present in slot
            if (slot.childCount > 0)
            {
                DraggableItem draggableItem = slot.GetChild(0).GetComponent<DraggableItem>();

                // If item is consumable
                if (draggableItem.Item.itemType == ItemType.Consumable && draggableItem.QuantityStacked > 0)
                {
                    return draggableItem.Item;
                }
            }
        }

        return null;
    }

    public void RemoveItem(Item item)
    {
        for (int i = 0; i < feederInventory.transform.childCount; i++)
        {
            Transform slot = feederInventory.transform.GetChild(i);

            // If item present in slot
            if (slot.childCount > 0)
            {
                DraggableItem draggableItem = slot.GetChild(0).GetComponent<DraggableItem>();

                if (draggableItem.Item == item)
                {
                    draggableItem.QuantityStacked -= 1;

                    if (draggableItem.QuantityStacked <= 0)
                    {
                        Destroy(draggableItem.gameObject);
                    }
                }
            }
        }
    }

    private bool IsFeederEmpty()
    {
        bool isEmpty = true;

        for (int i = 0; i < feederInventory.transform.childCount; i++)
        {
            Transform slot = feederInventory.transform.GetChild(i);

            if (slot.childCount > 0)
            {
                isEmpty = false;
            }
        }

        return isEmpty;
    }

    #endregion

    #endregion
}