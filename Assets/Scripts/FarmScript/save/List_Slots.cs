using System;
using UnityEngine;
using UnityEngine.UI;
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


        CreateTempItem();


        //player
        LoadPlayerInventory();

        //container
        if (handleContainer)
            LoadContainerInventory();

        HandleMoney();
    }

    private void CreateTempItem()
    {
        HandleVerificationAndApplication();

        PlayerInventoryUI inventory = FindObjectOfType<PlayerInventoryUI>();

        if (inventory == null) return;

        GameObject itemUI = inventory.CreateItemUI();

        itemUI.GetComponent<DraggableItem>().quantityStacked = 3;

        itemUI.GetComponent<DraggableItem>().Item = (Item)stuffs[0];

        AutoSavePlayerInventory();
    }

    #region Money

    public void AutoSaveMoney()
    {
        SaveSystem.SavePlayerMoney(pC);
    }

    public void UpdateMoney(int moneyUpdated)
    {
        pC.money = moneyUpdated;
        moneyUI.text = $"{pC.money}";

        AutoSaveMoney();
    }

    private void HandleMoney()
    {
        //pC.money = SaveSystem.LoadPlayerController().money;

        Player_Data playerData = SaveSystem.LoadPlayerController(pC);

        if (playerData != null)
        {
            pC.money = playerData.money;
        }

        moneyUI.text = $"{pC.money}";
    }

    #endregion

    private void Update()
    {
        if (pI.actions["save_Inventory"].triggered)
            SaveData();

        HandleMoney();
    }

    private void LateUpdate()
    {
        HandleVerificationAndApplication();
    }

    public void SaveData()
    {
        Debug.Log($"Force save");

        SaveSystem.SavePlayerMoney(pC);
        SaveSystem.SavePlayerInventory(this);
        SaveSystem.SaveContainerInventory(this);
    }

    #region Load Inventory

    public void AutoSavePlayerInventory()
    {
        SaveSystem.SavePlayerInventory(this);
    }

    public void AutoSaveContainerInventory()
    {
        SaveSystem.SaveContainerInventory(this);
    }

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
            if (listSlots[i].transform.childCount > 0 && itemsInSlot[i] != -1)
            {
                listSlots[i].GetComponentInChildren<DraggableItem>().Item = (Item)stuffs[itemsInSlot[i]];
                listSlots[i].GetComponentInChildren<DraggableItem>().quantityStacked = quantityStackedInventory[i];
            }
        }
    }

    private void LoadPlayerInventoryItem()
    {
        PlayerInventory_Data data = SaveSystem.LoadPlayerInventory(this);

        if (data == null) return;

        for (int i = 0; i < playerSlotsObjIn.Length; i++)
        {
            playerSlotsObjIn[i] = data.playerSlotsObjIn[i];

            itemsInSlot[i] = data.itemsInSlot[i];

            quantityStackedPlayerInventory[i] = data.quantityStackedPlayerInventory[i];
        }
    }

    private void LoadContainerInventoryItem()
    {
        ContainerInventory_Data data = SaveSystem.LoadContainerInventory(this);

        if (data == null) return;

        for (int i = 0; i < containerSlotsObjIn.Length; i++)
        {
            containerSlotsObjIn[i] = data.containerSlotsObjIn[i];

            containerItemsInSlot[i] = data.containerItemsInSlot[i];

            quantityStackedContainerInventory[i] = data.quantityStackedContainerInventory[i];
        }
    }

    #endregion

    #region Verification & Application

    public void HandleVerificationAndApplication()
    {
        if (!checkLoad)
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

    private void ApplySave()
    {
        for (int i = 0; i < playerSlots.Length; i++)
        {
            if (playerSlots[i].transform.childCount > 0)
            {
                if (itemsInSlot[i] != -1)
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
                if (containerItemsInSlot[i] != -1)
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

    #endregion
}