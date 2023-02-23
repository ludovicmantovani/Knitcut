using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class List_Slots : MonoBehaviour
{
    public GameObject itemUI;

    public ScriptableObject[] stuffs;
    private bool checkLoad = false;

    [Header("PlayerInventory")]
    public GameObject[] playerSlots;
    public int[] itemsInSlot;
    public bool[] playerSlotsObjIn;
    public int[] quantityStackedPlayerInventory;

    [Header("ContainerInventory")]
    public GameObject[] containerSlots;
    public int[] containerItemsInSlot;
    public bool[] containerSlotsObjIn;
    public int[] quantityStackedContainerInventory;



    private void Start()
    {
        LoadPlayerInventoryItem();
        //player
        for (int i = 0; i < playerSlots.Length; i++)
        {
            if(playerSlotsObjIn[i] == true)
            {
                Instantiate(itemUI, playerSlots[i].transform);
            }
        }
        for (int i = 0; i < playerSlots.Length; i++)
        {
            if (playerSlots[i].transform.childCount > 0)
            {
                if (itemsInSlot[i] == 0)
                {
                    playerSlots[i].GetComponentInChildren<DraggableItem>().Item = (Item)stuffs[0];
                }
                if (itemsInSlot[i] == 1)
                {
                    playerSlots[i].GetComponentInChildren<DraggableItem>().Item = (Item)stuffs[1];
                }
                if (itemsInSlot[i] == 2)
                {
                    playerSlots[i].GetComponentInChildren<DraggableItem>().Item = (Item)stuffs[2];
                }
                playerSlots[i].GetComponentInChildren<DraggableItem>().quantityStacked = quantityStackedPlayerInventory[i];
            }

        }
        //container
        LoadContainerInventoryItem();
        for (int i = 0; i < containerSlots.Length; i++)
        {
            if (containerSlotsObjIn[i] == true)
            {
                Instantiate(itemUI, containerSlots[i].transform);
            }
        }
        for (int i = 0; i < containerSlots.Length; i++)
        {
            if (containerSlots[i].transform.childCount > 0)
            {
                if (containerItemsInSlot[i] == 0)
                {
                    containerSlots[i].GetComponentInChildren<DraggableItem>().Item = (Item)stuffs[0];
                }
                if (containerItemsInSlot[i] == 1)
                {
                    containerSlots[i].GetComponentInChildren<DraggableItem>().Item = (Item)stuffs[1];
                }
                if (containerItemsInSlot[i] == 2)
                {
                    containerSlots[i].GetComponentInChildren<DraggableItem>().Item = (Item)stuffs[2];
                }
                containerSlots[i].GetComponentInChildren<DraggableItem>().quantityStacked = quantityStackedContainerInventory[i];
            }

        }

    }
    private void Update()
    {
        SaveInventoryItem();
        
    }
    private void LateUpdate()
    {
        if (checkLoad == false)
        {
            ApplySave();
            ApplySaveContainer();
            checkLoad = true;

        }
        VerificationChild();
        VerificationChildContainer();
    }
    public void VerificationChild()
    {
        for (int i = 0; i < playerSlots.Length; i++)
        {
            if (playerSlots[i].transform.childCount > 0)
            {
                playerSlotsObjIn[i] = true;
            }
            else
            {
                playerSlotsObjIn[i] = false;
            }
            if (playerSlots[i].transform.childCount > 0)
            {
                quantityStackedPlayerInventory[i] = playerSlots[i].GetComponentInChildren<DraggableItem>().quantityStacked;
            }
            else
            {
                quantityStackedPlayerInventory[i] = 0;
            }
            if (playerSlots[i].transform.childCount > 0)
            {
                if (stuffs[0] == playerSlots[i].GetComponentInChildren<DraggableItem>().Item)
                {
                    itemsInSlot[i] = 0;
                }
                else if (stuffs[1] == playerSlots[i].GetComponentInChildren<DraggableItem>().Item)
                {
                    itemsInSlot[i] = 1;
                }
                else if (stuffs[2] == playerSlots[i].GetComponentInChildren<DraggableItem>().Item)
                {
                    itemsInSlot[i] = 2;
                }
                
            }
            else
            {
                itemsInSlot[i] = -1;
            }
        }
    }
    void SaveInventoryItem()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            SaveSystem.SavePlayerInventorySlot(this);
        }
    }
    void LoadPlayerInventoryItem()
    {
        Inventory_Data data = SaveSystem.LoadPlayerInventorySlot();
        for (int i = 0; i < playerSlotsObjIn.Length; i++)
        {
            playerSlotsObjIn[i] = data.playerSlotsObjIn[i];
        }
        for (int i = 0; i < itemsInSlot.Length; i++)
        {
            itemsInSlot[i] = data.itemsInSlot[i];
        }
        for (int i = 0; i < quantityStackedPlayerInventory.Length; i++)
        {
            quantityStackedPlayerInventory[i] = data.quantityStackedPlayerInventory[i];
        }
    }
    void ApplySave()
    {
        for (int i = 0; i < playerSlots.Length; i++)
        {
            if (playerSlots[i].transform.childCount > 0)
            {
                if(itemsInSlot[i] == 0)
                {
                    playerSlots[i].GetComponentInChildren<DraggableItem>().Item = (Item)stuffs[0];
                }
                if (itemsInSlot[i] == 1)
                {
                    playerSlots[i].GetComponentInChildren<DraggableItem>().Item = (Item)stuffs[1];
                }
                if (itemsInSlot[i] == 2)
                {
                    playerSlots[i].GetComponentInChildren<DraggableItem>().Item = (Item)stuffs[2];
                }
            }
            
        }
    }
    void ApplySaveContainer()
    {
        for (int i = 0; i < containerSlots.Length; i++)
        {
            if (containerSlots[i].transform.childCount > 0)
            {
                if (containerItemsInSlot[i] == 0)
                {
                    containerSlots[i].GetComponentInChildren<DraggableItem>().Item = (Item)stuffs[0];
                }
                if (containerItemsInSlot[i] == 1)
                {
                    containerSlots[i].GetComponentInChildren<DraggableItem>().Item = (Item)stuffs[1];
                }
                if (containerItemsInSlot[i] == 2)
                {
                    containerSlots[i].GetComponentInChildren<DraggableItem>().Item = (Item)stuffs[2];
                }
            }

        }
    }

    public void VerificationChildContainer()
    {
        for (int i = 0; i < containerSlots.Length; i++)
        {
            if (containerSlots[i].transform.childCount > 0)
            {
                containerSlotsObjIn[i] = true;
            }
            else
            {
                containerSlotsObjIn[i] = false;
            }
            if (containerSlots[i].transform.childCount > 0)
            {
                quantityStackedContainerInventory[i] = containerSlots[i].GetComponentInChildren<DraggableItem>().quantityStacked;
            }
            else
            {
                quantityStackedContainerInventory[i] = 0;
            }
            if (containerSlots[i].transform.childCount > 0)
            {
                if (stuffs[0] == containerSlots[i].GetComponentInChildren<DraggableItem>().Item)
                {
                    containerItemsInSlot[i] = 0;
                }
                else if (stuffs[1] == containerSlots[i].GetComponentInChildren<DraggableItem>().Item)
                {
                    containerItemsInSlot[i] = 1;
                }
                else if (stuffs[2] == containerSlots[i].GetComponentInChildren<DraggableItem>().Item)
                {
                    containerItemsInSlot[i] = 2;
                }

            }
            else
            {
                containerItemsInSlot[i] = -1;
            }
        }
    }

    public void LoadContainerInventoryItem()
    {
        Inventory_Data data = SaveSystem.LoadPlayerInventorySlot();
        for (int i = 0; i < containerSlotsObjIn.Length; i++)
        {
            containerSlotsObjIn[i] = data.containerSlotsObjIn[i];
        }
        for (int i = 0; i < containerItemsInSlot.Length; i++)
        {
            containerItemsInSlot[i] = data.containerItemsInSlot[i];
        }
        for (int i = 0; i < quantityStackedContainerInventory.Length; i++)
        {
            quantityStackedContainerInventory[i] = data.quantityStackedContainerInventory[i];
        }
    }
}
