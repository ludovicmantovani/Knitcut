using System;
using UnityEngine;
using UnityEngine.UI;
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
    [SerializeField] private Transform playerSlotsParent;

    private GameObject[] playerSlots;
    private int[] itemsInSlots;
    private bool[] playerSlotsObjIn;
    private int[] quantityStackedPlayerInventory;

    [Header("ContainerInventory")]
    [SerializeField] private bool handleContainer;
    [SerializeField] private Transform containerSlotsParent;

    private GameObject[] containerSlots;
    private int[] containerItemsInSlot;
    private bool[] containerSlotsObjIn;
    private int[] quantityStackedContainerInventory;

    #region Getters / Setters

    public int[] ItemsInSlots
    {
        get { return itemsInSlots; }
    }

    public bool[] PlayerSlotsObjIn
    {
        get { return playerSlotsObjIn; }
    }

    public int[] QuantityStackedPlayerInventory
    {
        get { return quantityStackedPlayerInventory; }
    }

    public int[] ContainerItemsInSlot
    {
        get { return containerItemsInSlot; }
    }

    public bool[] ContainerSlotsObjIn
    {
        get { return containerSlotsObjIn; }
    }

    public int[] QuantityStackedContainerInventory
    {
        get { return quantityStackedContainerInventory; }
    }

    #endregion

    private void Start()
    {
        pC = FindObjectOfType<playerController>();
        pI = GetComponent<PlayerInput>();

        HandleInventories();

        HandleMoney();
    }

    #region Money

    public void AutoSaveMoney()
    {
        //SaveSystem.SavePlayerMoney(pC);
        SaveSystem.Save(SaveSystem.SaveType.Save_PlayerController, pC);
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

        //Player_Data playerData = SaveSystem.LoadPlayerController(pC);
        Player_Data playerData = (Player_Data)SaveSystem.Load(SaveSystem.SaveType.Save_PlayerController, pC);

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

        /*SaveSystem.SavePlayerMoney(pC);
        SaveSystem.SavePlayerInventory(this);
        SaveSystem.SaveContainerInventory(this);*/
        SaveSystem.Save(SaveSystem.SaveType.Save_PlayerController, pC);
        SaveSystem.Save(SaveSystem.SaveType.Save_PlayerInventory, this);
        if (handleContainer)
            SaveSystem.Save(SaveSystem.SaveType.Save_ContainerInventory, this);
    }

    #region Load Inventory

    private void HandleInventories()
    {
        SetupInventory();

        //player
        LoadPlayerInventory();

        //container
        if (handleContainer)
            LoadContainerInventory();

        CreateTempItem();
    }
    private void CreateTempItem()
    {
        if (!MinigameManager.StartOK)
        {
            MinigameManager.StartOK = true;

            HandleVerificationAndApplication();

            PlayerInventoryUI inventory = FindObjectOfType<PlayerInventoryUI>();

            if (inventory == null) return;

            GameObject itemUI = inventory.CreateItemUI();

            itemUI.GetComponent<DraggableItem>().quantityStacked = 3;

            itemUI.GetComponent<DraggableItem>().Item = (Item)stuffs[0];

            AutoSavePlayerInventory();
        }
    }

    public void AutoSavePlayerInventory()
    {
        //SaveSystem.SavePlayerInventory(this);
        SaveSystem.Save(SaveSystem.SaveType.Save_PlayerInventory, this);
    }

    public void AutoSaveContainerInventory()
    {
        //SaveSystem.SaveContainerInventory(this);
        SaveSystem.Save(SaveSystem.SaveType.Save_ContainerInventory, this);
    }

    private void LoadPlayerInventory()
    {
        LoadPlayerInventoryItem();
        LoadInventory(playerSlots, playerSlotsObjIn, itemsInSlots, quantityStackedPlayerInventory);
    }

    private void LoadContainerInventory()
    {
        LoadContainerInventoryItem();
        LoadInventory(containerSlots, containerSlotsObjIn, containerItemsInSlot, quantityStackedContainerInventory);
    }

    private void SetupInventory()
    {
        // player
        if (playerSlots == null)
        {
            // Setup up list
            playerSlots = new GameObject[playerSlotsParent.childCount];

            itemsInSlots = new int[playerSlotsParent.childCount];
            playerSlotsObjIn = new bool[playerSlotsParent.childCount];
            quantityStackedPlayerInventory = new int[playerSlotsParent.childCount];

            for (int i = 0; i < playerSlotsParent.childCount; i++)
            {
                playerSlots[i] = playerSlotsParent.GetChild(i).gameObject;
            }
        }

        // container
        if (containerSlots == null && handleContainer)
        {
            // Setup up list
            containerSlots = new GameObject[containerSlotsParent.childCount];

            containerItemsInSlot = new int[containerSlotsParent.childCount];
            containerSlotsObjIn = new bool[containerSlotsParent.childCount];
            quantityStackedContainerInventory = new int[containerSlotsParent.childCount];

            for (int i = 0; i < containerSlotsParent.childCount; i++)
            {
                containerSlots[i] = containerSlotsParent.GetChild(i).gameObject;
            }
        }
    }

    private void LoadInventory(GameObject[] listSlots, bool[] objectInSlots, int[] itemsInSlot, int[] quantityStackedInventory)
    {
        // Handle Inventory items
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
        //PlayerInventory_Data data = SaveSystem.LoadPlayerInventory(this);
        PlayerInventory_Data data = (PlayerInventory_Data)SaveSystem.Load(SaveSystem.SaveType.Save_PlayerInventory, this);

        if (data == null) return;

        for (int i = 0; i < playerSlotsObjIn.Length; i++)
        {
            playerSlotsObjIn[i] = data.playerSlotsObjIn[i];

            itemsInSlots[i] = data.itemsInSlot[i];

            quantityStackedPlayerInventory[i] = data.quantityStackedPlayerInventory[i];
        }
    }

    private void LoadContainerInventoryItem()
    {
        //ContainerInventory_Data data = SaveSystem.LoadContainerInventory(this);
        ContainerInventory_Data data = (ContainerInventory_Data)SaveSystem.Load(SaveSystem.SaveType.Save_ContainerInventory, this);

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
                if (itemsInSlots[i] != -1)
                    playerSlots[i].GetComponentInChildren<DraggableItem>().Item = (Item)stuffs[itemsInSlots[i]];
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

                itemsInSlots[i] = Array.IndexOf(stuffs, playerSlots[i].GetComponentInChildren<DraggableItem>().Item);
            }
            else
            {
                playerSlotsObjIn[i] = false;

                quantityStackedPlayerInventory[i] = 0;

                itemsInSlots[i] = -1;
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