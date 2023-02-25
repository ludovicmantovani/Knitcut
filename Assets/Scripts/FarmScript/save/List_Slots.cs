using System;
using UnityEngine;
//rajout
using UnityEngine.InputSystem;

public class List_Slots : MonoBehaviour
{
    [Header("References / Parameters")]
    public GameObject itemUI;
    public playerController pC;
    public PlayerInput pI;
    public ScriptableObject[] stuffs;

    private bool checkLoad = false;
    public bool canVerif;

    [Header("Money")]
    public Text moneyUI;

    [Header("PlayerInventory")]
    public GameObject[] playerSlots;
    public int[] itemsInSlot;
    public bool[] playerSlotsObjIn;
    public int[] quantityStackedPlayerInventory;

    [Header("ContainerInventory")]
    public bool handleContainer;
    public GameObject[] containerSlots;
    public int[] containerItemsInSlot;
    public bool[] containerSlotsObjIn;
    public int[] quantityStackedContainerInventory;



    private void Start()
    {
        pC = FindObjectOfType<playerController>();
        pI = GetComponent<PlayerInput>();

        canVerif = true;

        //player
        LoadPlayerInventory();

        //container
        if (handleContainer)
            LoadContainerInventory();

        HandleMoney();
    }

    #region Money

    public void UpdateMoney(int moneyUpdated)
    {
        pC.money = moneyUpdated;
        moneyUI.text = $"{pC.money}";

        SaveSystem.SavePlayerMoney(pC);
    }

    private void HandleMoney()
    {
        //pC.money = SaveSystem.LoadPlayerController().money;

        if (SaveSystem.LoadPlayerController() != null)
        {
            pC.money = SaveSystem.LoadPlayerController().money;
        }

        moneyUI.text = $"{pC.money}";
    }

    #endregion

    #region Load Inventory

    private void LoadPlayerInventory()
    {
        LoadPlayerInventoryItem();
        LoadInventory(playerSlots, playerSlotsObjIn, itemsInSlot, quantityStackedPlayerInventory);
    }

    private void LoadContainerInventory()
    {
        LoadContainerInventoryItem();
        LoadInventory(containerSlots, containerSlotsObjIn, containerItemsInSlot, quantityStackedContainerInventory);
    }

    private void LoadInventory(GameObject[] listSlots, bool[] objectInSlots, int[] itemsInSlot, int[] quantityStackedInventory)
    {
        for (int i = 0; i < listSlots.Length; i++)
        {
            if (objectInSlots[i] == true)
            {
                Instantiate(itemUI, listSlots[i].transform);
            }
        }
        for (int i = 0; i < listSlots.Length; i++)
        {
            if (listSlots[i].transform.childCount > 0)
            {
                listSlots[i].GetComponentInChildren<DraggableItem>().Item = (Item)stuffs[itemsInSlot[i]];
                listSlots[i].GetComponentInChildren<DraggableItem>().quantityStacked = quantityStackedInventory[i];
            }

        }
    }

    #endregion

    private void Update()
    {
        SaveInventoryItem();

        HandleMoney();
    }

    private void LateUpdate()
    {
        if (canVerif)
        {
            if (checkLoad == false)
            {
                ApplySave();
                if (handleContainer)
                    ApplySaveContainer();
                checkLoad = true;
            }

            VerificationChild();
            if (handleContainer)
                VerificationChildContainer();
        }
    }

    public void AutoSavePlayerInventory()
    {
        SaveSystem.SavePlayerInventory(this);
    }

    public void AutoSaveContainerInventory()
    {
        SaveSystem.SaveContainerInventory(this);
    }

    private void SaveInventoryItem()
    {
        if (Input.GetKey(KeyCode.M))
        {
            SaveSystem.SavePlayerMoney(pC);
            SaveSystem.SavePlayerInventory(this);
            SaveSystem.SaveContainerInventory(this);
        }
        /*if (pI.actions["save_Inventory"].triggered)
        {
            SaveSystem.SavePlayerInventorySlot(this);
        }*/
    }

    private void LoadPlayerInventoryItem()
    {
        PlayerInventory_Data data = SaveSystem.LoadPlayerInventory();

        for (int i = 0; i < playerSlotsObjIn.Length; i++)
        {
            playerSlotsObjIn[i] = data.playerSlotsObjIn[i];

            itemsInSlot[i] = data.itemsInSlot[i];

            quantityStackedPlayerInventory[i] = data.quantityStackedPlayerInventory[i];
        }
    }

    private void LoadContainerInventoryItem()
    {
        ContainerInventory_Data data = SaveSystem.LoadContainerInventory();

        for (int i = 0; i < containerSlotsObjIn.Length; i++)
        {
            containerSlotsObjIn[i] = data.containerSlotsObjIn[i];

            containerItemsInSlot[i] = data.containerItemsInSlot[i];

            quantityStackedContainerInventory[i] = data.quantityStackedContainerInventory[i];
        }
    }

    private void ApplySave()
    {
        for (int i = 0; i < playerSlots.Length; i++)
        {
            if (playerSlots[i].transform.childCount > 0)
            {
                playerSlots[i].GetComponentInChildren<DraggableItem>().Item = (Item)stuffs[itemsInSlot[i]];
            }            
        }
    }

    private void ApplySaveContainer()
    {
        for (int i = 0; i < containerSlots.Length; i++)
        {
            if (containerSlots[i].transform.childCount > 0)
            {
                containerSlots[i].GetComponentInChildren<DraggableItem>().Item = (Item)stuffs[containerItemsInSlot[i]];
            }
        }
    }

    private void VerificationChild()
    {
        for (int i = 0; i < playerSlots.Length; i++)
        {
            if (playerSlots[i].transform.childCount > 0)
            {
                playerSlotsObjIn[i] = true;

                quantityStackedPlayerInventory[i] = playerSlots[i].GetComponentInChildren<DraggableItem>().quantityStacked;

                itemsInSlot[i] = Array.IndexOf(stuffs, playerSlots[i].GetComponentInChildren<DraggableItem>().Item);
            }
            else
            {
                playerSlotsObjIn[i] = false;

                quantityStackedPlayerInventory[i] = 0;

                itemsInSlot[i] = -1;
            }
        }
    }

    private void VerificationChildContainer()
    {
        for (int i = 0; i < containerSlots.Length; i++)
        {
            if (containerSlots[i].transform.childCount > 0)
            {
                containerSlotsObjIn[i] = true;

                quantityStackedContainerInventory[i] = containerSlots[i].GetComponentInChildren<DraggableItem>().quantityStacked;

                containerItemsInSlot[i] = Array.IndexOf(stuffs, containerSlots[i].GetComponentInChildren<DraggableItem>().Item);
            }
            else
            {
                containerSlotsObjIn[i] = false;

                quantityStackedContainerInventory[i] = 0;

                containerItemsInSlot[i] = -1;
            }
        }
    }
}