using System;
using UnityEngine;
using UnityEngine.UI;

public class ListSlots : MonoBehaviour
{
    [Header("References / Parameters")]
    [SerializeField] private GameObject itemUI;
    [SerializeField] private ScriptableObject[] stuffs;

    private PlayerController playerController;
    private PlayerInput playerInput;
    private AnimalPenManager animalPenManager;
    private CultureManager cultureManager;
    private bool checkLoad = false;

    [Header("Money")]
    [SerializeField] private Text moneyUI;

    [Header("Player Inventory")]
    [SerializeField] private Transform playerSlotsParent;

    private GameObject[] playerSlots;
    private int[] itemsInSlots;
    private bool[] playerSlotsObjIn;
    private int[] quantityStackedPlayerInventory;
    private float[] uniqueValuePlayerInventory;

    [Header("ContainerInventory")]
    [SerializeField] private bool handleContainer;
    [SerializeField] private Transform containerSlotsParent;

    private GameObject[] containerSlots;
    private int[] containerItemsInSlot;
    private bool[] containerSlotsObjIn;
    private int[] quantityStackedContainerInventory;
    private float[] uniqueValueContainerInventory;

    #region Getters / Setters

    public PlayerController PlayerControl
    {
        get { return playerController; }
        set { playerController = value; }
    }

    public ScriptableObject[] Stuffs
    {
        get { return stuffs; }
        set { stuffs = value; }
    }

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

    public float[] UniqueValuePlayerInventory
    {
        get { return uniqueValuePlayerInventory; }
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

    public float[] UniqueValueContainerInventory
    {
        get { return uniqueValueContainerInventory; }
    }

    #endregion

    private void Awake()
    {
        playerController = FindObjectOfType<PlayerController>();
        playerInput = FindObjectOfType<PlayerInput>();
        animalPenManager = FindObjectOfType<AnimalPenManager>();
        cultureManager = FindObjectOfType<CultureManager>();

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
        playerController.SavePlayerPositionInScene();

        AutoSavePlayerInventory();

        playerController.PlayerRecipesInventory.SaveRecipes();

        if (handleContainer) AutoSaveContainerInventory();
        if (handleContainer) cultureManager.SaveCulture();

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
        UpdateMoney(200);

        ItemCreation(6, 5);
        ItemCreation(7, 5);
        ItemCreation(8, 5);
        ItemCreation(9, 5);
        ItemCreation(10, 5);

        //ItemCreation(4, 4); // fruit citrouille
        //ItemCreation(7, 2); // fruit radis

        ItemCreation(13, 2); // graine framboise
        ItemCreation(14, 2); // graine palmier
    }

    private void ItemCreation(int indexItem, int quantity)
    {
        HandleVerificationAndApplication();

        PlayerInventory inventory = playerController.PlayerInventory;

        if (inventory == null) return;

        inventory.AddItemToInventory((Item)stuffs[indexItem], quantity);

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
        LoadInventory(playerSlots, playerSlotsObjIn, itemsInSlots, quantityStackedPlayerInventory, uniqueValuePlayerInventory);
    }

    private void LoadContainerInventory()
    {
        LoadContainerInventoryItem();
        LoadInventory(containerSlots, containerSlotsObjIn, containerItemsInSlot, quantityStackedContainerInventory, uniqueValueContainerInventory);
    }

    private void SetupInventory()
    {
        // player
        if (playerSlots == null)
        {
            // Setup up list
            int playerSlotsCount = playerSlotsParent.childCount;

            playerSlots = new GameObject[playerSlotsCount];

            itemsInSlots = new int[playerSlotsCount];
            playerSlotsObjIn = new bool[playerSlotsCount];
            quantityStackedPlayerInventory = new int[playerSlotsCount];
            uniqueValuePlayerInventory = new float[playerSlotsCount];

            for (int i = 0; i < playerSlotsCount; i++)
            {
                playerSlots[i] = playerSlotsParent.GetChild(i).gameObject;
            }
        }

        // container
        if (containerSlots == null && handleContainer)
        {
            // Setup up list
            int containerSlotsCount = containerSlotsParent.childCount;

            containerSlots = new GameObject[containerSlotsCount];

            containerItemsInSlot = new int[containerSlotsCount];
            containerSlotsObjIn = new bool[containerSlotsCount];
            quantityStackedContainerInventory = new int[containerSlotsCount];
            uniqueValueContainerInventory = new float[containerSlotsCount];

            for (int i = 0; i < containerSlotsCount; i++)
            {
                containerSlots[i] = containerSlotsParent.GetChild(i).gameObject;
            }
        }
    }

    private void LoadInventory(GameObject[] listSlots, bool[] objectInSlots, int[] itemsInSlot, int[] quantityStackedInventory, float[] uniqueValueInventory)
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
                listSlots[i].GetComponentInChildren<ItemHandler>().Item = (Item)stuffs[itemsInSlot[i]];
                listSlots[i].GetComponentInChildren<ItemHandler>().QuantityStacked = quantityStackedInventory[i];
                listSlots[i].GetComponentInChildren<ItemHandler>().UniqueValue = uniqueValueInventory[i];
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

            uniqueValuePlayerInventory[i] = data.uniqueValuePlayerInventory[i];
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

            uniqueValueContainerInventory[i] = data.uniqueValueContainerInventory[i];
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
                    playerSlots[i].GetComponentInChildren<ItemHandler>().Item = (Item)stuffs[itemsInSlots[i]];
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
                    containerSlots[i].GetComponentInChildren<ItemHandler>().Item = (Item)stuffs[containerItemsInSlot[i]];
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

                quantityStackedPlayerInventory[i] = playerSlots[i].GetComponentInChildren<ItemHandler>().QuantityStacked;

                uniqueValuePlayerInventory[i] = playerSlots[i].GetComponentInChildren<ItemHandler>().UniqueValue;

                itemsInSlots[i] = Array.IndexOf(stuffs, playerSlots[i].GetComponentInChildren<ItemHandler>().Item);
            }
            else
            {
                playerSlotsObjIn[i] = false;

                quantityStackedPlayerInventory[i] = 0;

                uniqueValuePlayerInventory[i] = 0;

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

                quantityStackedContainerInventory[i] = containerSlots[i].GetComponentInChildren<ItemHandler>().QuantityStacked;

                uniqueValueContainerInventory[i] = containerSlots[i].GetComponentInChildren<ItemHandler>().UniqueValue;

                containerItemsInSlot[i] = Array.IndexOf(stuffs, containerSlots[i].GetComponentInChildren<ItemHandler>().Item);
            }
            else
            {
                containerSlotsObjIn[i] = false;

                quantityStackedContainerInventory[i] = 0;

                uniqueValueContainerInventory[i] = 0;

                containerItemsInSlot[i] = -1;
            }
        }
    }

    #endregion

    public int GetItemIndex(ScriptableObject item)
    {
        int index = -1;

        for (int i = 0; i < stuffs.Length; i++)
        {
            if (item == stuffs[i]) index = i;
        }

        return index;
    }

    public ScriptableObject GetItemByIndex(int index)
    {
        ScriptableObject item = null;

        if (index != -1) item = stuffs[index];

        return item;
    }
}