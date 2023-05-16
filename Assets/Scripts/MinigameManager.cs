using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MinigameManager : MonoBehaviour
{
    private static List<object> dataToKeep;
    private static string sceneToLoad = "FarmScene";
    private static AnimalType animalTypeToKeep = AnimalType.None;
    private static List<int> animalPenIndexToUpgrade = new List<int>();
    private static List<Recipe> recipesPossessed;
    private static PlayerController playerController;
    private static List<GameObject> openInventories = new List<GameObject>();
    private static List<PlayerItem> playerItems = new List<PlayerItem>();
    private static Dictionary<Item, int> itemsToRemoveQuantity = new Dictionary<Item, int>();
    private static bool returnToFarm = false;

    #region Getters / Setters

    public class PlayerItem
    {
        public Item item;
        public int quantity;
    }

    public static List<object> DataToKeep
    {
        get { return dataToKeep; }
    }

    public static AnimalType AnimalTypeToKeep
    {
        get { return animalTypeToKeep; }
        set { animalTypeToKeep = value; }
    }

    public static List<int> AnimalPenIndexToUpgrade
    {
        get { return animalPenIndexToUpgrade; }
        set { animalPenIndexToUpgrade = value; }
    }

    public static List<Recipe> RecipesPossessed
    {
        get { return recipesPossessed; }
        set { recipesPossessed = value; }
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

    #endregion

    private List_Slots listSlots;

    bool dataLoaded = false;
    bool animalCaptured = false;

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

            if (SceneManager.GetActiveScene().name.Contains("Farm") && !FindObjectOfType<List_Slots>()) return;

            CheckItemsToAdd();
        }

        if (animalCaptured)
        {
            animalCaptured = false;

            AnimalPenManager animalPenManager = FindObjectOfType<AnimalPenManager>();

            if (SceneManager.GetActiveScene().name.Contains("Farm") && !animalPenManager) return;

            animalPenManager.InstantiateTamedAnimalInAnimalPen();
        }
    }

    private void OnLevelWasLoaded()
    {
        if (returnToFarm && SceneManager.GetActiveScene().name.Contains("Farm"))
        {
            PlayerController playerController = FindObjectOfType<PlayerController>();

            playerController.LoadPlayerPositionInScene();
        }

        if (SceneManager.GetActiveScene().name.Contains("Farm") || SceneManager.GetActiveScene().name.Contains("Water"))
        {
            if (dataToKeep != null) dataLoaded = true;

            if (animalTypeToKeep != AnimalType.None) animalCaptured = true;
        }
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

        for (int i = 0; i < openInventories.Count; i++)
        {
            if (openInventories[i] == null) openInventories.Remove(openInventories[i]);
        }
    }

    public static void CleanOpenInventories()
    {
        openInventories.Clear();
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

        Item item = (Item)listSlots.stuffs[FindFirstGenericDishSO() + Convert.ToInt32(dataToKeep[3])];

        item.itemName = (string)dataToKeep[0];
        item.itemDescription = (string)dataToKeep[1];
        item.itemValue = (float)dataToKeep[2];

        inventory.AddItemToInventory(item);

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
        int nbChildrenToInstantiate = Convert.ToInt32(dataToKeep[0]);

        if (nbChildrenToInstantiate > 0)
        {
            AnimalPenManager animalPenManager = FindObjectOfType<AnimalPenManager>();
            AnimalType currentAnimalType = animalTypeToKeep;

            for (int i = 0; i < nbChildrenToInstantiate; i++)
            {
                animalTypeToKeep = currentAnimalType;
                animalPenManager.InstantiateTamedAnimalInAnimalPen(true);
            }
        }
    }

    private void HandleCaptureData()
    {

    }

    #endregion

    public static void SwitchScene(string specificScene = "")
    {
        if (specificScene != "")
        {
            returnToFarm = false;
            SceneManager.LoadScene(specificScene);
        }
        else
        {
            returnToFarm = true;
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}