using System;
using UnityEngine;
using UnityEngine.UI;

public class List_Slots : MonoBehaviour
{
    [Header("References / Parameters")]
    public GameObject itemUI;
    public PlayerController playerController;
    public PlayerInput playerInput;
    public AnimalPenManager animalPenManager;
    public ScriptableObject[] stuffs;

    private bool checkLoad = false;

    [Header("Money")]
    public Text moneyUI;

    [Header("Player Inventory")]
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

    public Transform PlayerSlotsParent
    {
        get { return playerSlotsParent; }
        set { playerSlotsParent = value; }
    }

    public GameObject[] PlayerSlots
    {
        get { return playerSlots; }
        set { playerSlots = value; }
    }

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

    private void Awake()
    {
        playerController = FindObjectOfType<PlayerController>();
        playerInput = FindObjectOfType<PlayerInput>();
        animalPenManager = FindObjectOfType<AnimalPenManager>();

        HandleInventories();

        HandleMoney();
    }

    #region Money

    public void AutoSaveMoney()
    {
        SaveSystem.Save(SaveSystem.SaveType.Save_PlayerController, playerController);
    }

    public void UpdateMoney(int moneyUpdated)
    {
        playerController.Money = moneyUpdated;
        moneyUI.text = $"{playerController.Money}";

        AutoSaveMoney();
    }

    private void HandleMoney()
    {
        Player_Data playerData = (Player_Data)SaveSystem.Load(SaveSystem.SaveType.Save_PlayerController, playerController);

        if (playerData != null)
        {
            playerController.Money = playerData.money;
        }

        moneyUI.text = $"{playerController.Money}";
    }

    #endregion

    private void Update()
    {
        if (playerInput.QuickSaveAction.triggered)
            SaveData();

        HandleMoney();
    }

    private void LateUpdate()
    {
        HandleVerificationAndApplication();
    }

    public void SaveData()
    {
        SaveSystem.Save(SaveSystem.SaveType.Save_PlayerController, playerController);

        AutoSavePlayerInventory();

        playerController.PlayerRecipesInventory.SaveRecipes();

        if (handleContainer) AutoSaveContainerInventory();

        if (animalPenManager != null)
            animalPenManager.SaveAnimalPenData();
    }

    private void OnApplicationQuit()
    {
        SaveData();
    }

    #region Load Inventory

    public void HandleInventories()
    {
        SetupInventory();

        //player
        LoadPlayerInventory();

        //container
        if (handleContainer)
            LoadContainerInventory();
    }

    #region Temp Item at Start

    public void CreateTempItems()
    {
        UpdateMoney(750);

        ItemCreation(7, 5);
    }

    private void ItemCreation(int indexItem, int quantity)
    {
        HandleVerificationAndApplication();

        PlayerInventory inventory = playerController.PlayerInventory;

        if (inventory == null) return;

        GameObject itemUI = inventory.CreateItemUI();

        itemUI.GetComponent<DraggableItem>().Item = (Item)stuffs[indexItem];

        itemUI.GetComponent<DraggableItem>().QuantityStacked = quantity;

        AutoSavePlayerInventory();
    }

    #endregion

    public void AutoSavePlayerInventory()
    {
        SaveSystem.Save(SaveSystem.SaveType.Save_PlayerInventory, this);
    }

    public void AutoSaveContainerInventory()
    {
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
                listSlots[i].GetComponentInChildren<DraggableItem>().QuantityStacked = quantityStackedInventory[i];
            }
        }
    }

    private void LoadPlayerInventoryItem()
    {
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
        if (playerSlots == null) return;

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
        if (containerSlots == null) return;

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
        if (playerSlots == null) return;

        for (int i = 0; i < playerSlots.Length; i++)
        {
            if (playerSlots[i].transform.childCount > 0)
            {
                playerSlotsObjIn[i] = true;

                quantityStackedPlayerInventory[i] = playerSlots[i].GetComponentInChildren<DraggableItem>().QuantityStacked;

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
        if (containerSlots == null) return;

        for (int i = 0; i < containerSlots.Length; i++)
        {
            if (containerSlots[i].transform.childCount > 0)
            {
                containerSlotsObjIn[i] = true;

                quantityStackedContainerInventory[i] = containerSlots[i].GetComponentInChildren<DraggableItem>().QuantityStacked;

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