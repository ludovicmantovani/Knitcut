using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MinigameManager : MonoBehaviour
{
    private static List<object> dataToKeep;
    private static bool startOK = false;
    private static string sceneToLoad = "FarmScene";
    private static string animalToKeep = "";
    private static List<int> animalPenIndexToUpgrade = new List<int>();
    private static PlayerController playerController;
    private static List<GameObject> openInventories = new List<GameObject>();
    private static List<PlayerItem> playerItems = new List<PlayerItem>();
    private static Dictionary<Item, int> itemsToRemoveQuantity = new Dictionary<Item, int>();

    public class PlayerItem
    {
        public Item item;
        public int quantity;
    }

    public static List<object> DataToKeep
    {
        get { return dataToKeep; }
    }

    public static bool StartOK
    {
        get { return startOK; }
        set { startOK = value; }
    }

    public static string AnimalToKeep
    {
        get { return animalToKeep; }
        set { animalToKeep = value; }
    }

    public static List<int> AnimalPenIndexToUpgrade
    {
        get { return animalPenIndexToUpgrade; }
        set { animalPenIndexToUpgrade = value; }
    }

    public static List<GameObject> OpenInventories
    {
        get { return openInventories; }
        set { openInventories = value; }
    }

    public static List<PlayerItem> PlayerItems
    {
        get { return playerItems; }
        set { playerItems = value; }
    }

    private List_Slots listSlots;

    bool dataLoaded;

    public enum MGType
    {
        Cooking,
        Recognition,
        Breeding,
        Capture
    }

    private static MGType mgType;

    private void Update()
    {
        HandlePanels();

        if (playerController == null && FindObjectOfType<PlayerController>())
            playerController = FindObjectOfType<PlayerController>();

        if (dataLoaded)
        {
            dataLoaded = false;

            if (SceneManager.GetActiveScene().name.Contains("Farm") && !FindObjectOfType<List_Slots>())
            {
                return;
            }

            CheckItemsToAdd();
        }
    }

    private void OnLevelWasLoaded()
    {
        if ((SceneManager.GetActiveScene().name.Contains("Farm") || SceneManager.GetActiveScene().name.Contains("Water"))
            && dataToKeep != null) dataLoaded = true;
    }

    public static void AddPlayerItem(Item item, int quantity)
    {
        PlayerItem playerItem = new PlayerItem();
        playerItem.item = item;
        playerItem.quantity = quantity;

        playerItems.Add(playerItem);
    }

    public static void RemovePlayerItem(Item item, int quantity)
    {
        itemsToRemoveQuantity.Add(item, quantity);
    }

    #region Open Inventories

    public static void AddOpenInventory(GameObject inventory)
    {
        if (!openInventories.Contains(inventory))
            openInventories.Add(inventory);
    }

    public static void RemoveOpenInventory(GameObject inventory)
    {
        if (openInventories.Contains(inventory))
            openInventories.Remove(inventory);
    }

    private void HandlePanels()
    {
        if (playerController == null) return;

        if (openInventories.Count > 0)
        {
            playerController.CameraCineBrain.enabled = false;
            playerController.CanMove = false;
        }
        else
        {
            playerController.CameraCineBrain.enabled = true;
            playerController.CanMove = true;
        }
    }

#endregion

    #region Handle Mini Games Datas

    public static void FinalizeMG(MGType _mgType, params object[] data)
    {
        mgType = _mgType;

        dataToKeep = new List<object>();

        for (int i = 0; i < data.Length; i++)
        {
            dataToKeep.Add(data[i]);
        }
    }

    private void CheckItemsToAdd()
    {
        if (SceneManager.GetActiveScene().name.Contains("Farm"))
            listSlots = FindObjectOfType<List_Slots>();

        switch (mgType)
        {
            case MGType.Cooking:
                HandleCookingData();
                break;
            case MGType.Recognition:
                HandleRecognitionData();
                break;
            case MGType.Breeding:
                HandleBreedingData();
                break;
            case MGType.Capture:
                HandleCaptureData();
                break;
            default:
                Debug.Log($"Error, data mgType");
                break;
        }

        dataLoaded = false;

        dataToKeep = new List<object>();
    }

    private void HandleCookingData()
    {
        PlayerInventory inventory = listSlots.PlayerSlotsParent.GetComponent<PlayerInventory>();

        if (inventory == null || FindFirstGenericDishSO() == -1) return;

        GameObject itemUI = inventory.CreateItemUI();

        itemUI.GetComponent<DraggableItem>().QuantityStacked = 1;

        itemUI.GetComponent<DraggableItem>().Item = (Item)listSlots.stuffs[FindFirstGenericDishSO() + Convert.ToInt32(dataToKeep[3])];

        itemUI.GetComponent<DraggableItem>().Item.itemName = (string)dataToKeep[0];
        itemUI.GetComponent<DraggableItem>().Item.itemDescription = (string)dataToKeep[1];
        itemUI.GetComponent<DraggableItem>().Item.itemValue = (float)dataToKeep[2];

        HandleItemsInInventory(inventory);
    }

    private int FindFirstGenericDishSO()
    {
        for (int i = 0; i < listSlots.stuffs.Length; i++)
        {
            Item item = (Item)listSlots.stuffs[i];

            if (item.name.Contains("Dish"))
            {
                return i;
            }
        }

        return -1;
    }

    private void HandleItemsInInventory(PlayerInventory inventory)
    {
        foreach (Item item in itemsToRemoveQuantity.Keys)
        {
            if (itemsToRemoveQuantity.TryGetValue(item, out int quantity))
                inventory.RemoveItemQuantity(item, quantity);
        }
    }

    private void HandleRecognitionData()
    {
        listSlots.UpdateMoney(listSlots.playerController.Money + Convert.ToInt32(dataToKeep[0]));
    }

    private void HandleBreedingData()
    {
        bool breedingSuccess = Convert.ToBoolean(dataToKeep[0]);

        Debug.Log($"breeding success ? {breedingSuccess}");
    }

    private void HandleCaptureData()
    {

    }

    #endregion

    public static void SwitchScene()
    {
        Debug.Log($"Switch scene '{SceneManager.GetActiveScene().name}' to scene '{sceneToLoad}'");
        SceneManager.LoadScene(sceneToLoad);
    }
}