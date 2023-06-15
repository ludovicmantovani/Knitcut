using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MinigameManager : MonoBehaviour
{
    private static List<object> dataToKeep;
    private static string sceneToLoad = "FarmScene";
    private static string menuScene = "Menu";
    private static AnimalType animalTypeToKeep = AnimalType.None;
    private static List<int> animalPenIndexToUpgrade = new List<int>();
    private static List<Recipe> recipesPossessed;
    private static PlayerController playerController;
    private static List<GameObject> openInventories = new List<GameObject>();
    private static List<PlayerItem> playerItems = new List<PlayerItem>();
    private static Dictionary<Item, int> itemsToRemoveQuantity = new Dictionary<Item, int>();
    private static bool returnToFarm = false;
    private static MGType mgType;

    private ListSlots listSlots;
    private bool dataLoaded = false;
    private bool animalCaptured = false;

    private static event Action OnInventoryListUpdate;

    public enum MGType
    {
        Cooking,
        Recognition,
        Breeding,
        Capture
    }

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

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoaded;

        OnInventoryListUpdate += OnInventoryOpen;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoaded;

        OnInventoryListUpdate -= OnInventoryOpen;
    }

    private void Update()
    {
        if (dataLoaded)
        {
            dataLoaded = false;

            if (SceneManager.GetActiveScene().name.Contains("Farm") && !FindObjectOfType<ListSlots>()) return;

            CheckItemsToAdd();
        }

        if (animalCaptured)
        {
            if (mgType != MGType.Capture) return;

            animalCaptured = false;

            AnimalPenManager animalPenManager = FindObjectOfType<AnimalPenManager>();

            if (SceneManager.GetActiveScene().name.Contains("Farm") && animalPenManager == null) return;

            animalPenManager.InstantiateTamedAnimalInAnimalPen();

            animalTypeToKeep = AnimalType.None;
        }

        if (Input.GetKeyDown(KeyCode.LeftControl) && openInventories.Count == 0)
        {
            if (Cursor.visible)
            {
                playerController.HandlePlayerMovement(true);

                HandleCursor(false);
            }
            else
            {
                playerController.HandlePlayerMovement(false);

                HandleCursor(true);
            }
        }

        // Cheat Codes

        // Money
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            listSlots = FindObjectOfType<ListSlots>();
            if (listSlots == null) return;
            listSlots.UpdateMoney(listSlots.PlayerControl.Money + 400);
        }

        // Plant
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            CultureManager culture = FindObjectOfType<CultureManager>();
            if (culture == null) return;
            PlantGrowth plant = culture.CurrentCropPlot.SeedSource.GetComponent<PlantGrowth>();
            if (plant == null) return;
            plant.SetGrowthState("End", "InGrowth", plant.CurrentPlant, 0);
        }

        // Capture Animal
        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            CaptureManager captureManager = FindObjectOfType<CaptureManager>();
            if (captureManager == null) return;
            if (captureManager.WildAnimal == null) return;
            animalTypeToKeep = captureManager.WildAnimal.GetComponent<AnimalAI>().AnimalType;
            Destroy(captureManager.WildAnimal);
            AnimalPenManager animalPenManager = FindObjectOfType<AnimalPenManager>();
            if (animalPenManager == null) return;
            animalPenManager.InstantiateTamedAnimalInAnimalPen();
        }
    }

    private void OnLevelFinishedLoaded(Scene scene, LoadSceneMode sceneMode)
    {
        playerController = FindObjectOfType<PlayerController>();

        if (playerController != null)
        {
            if (returnToFarm) playerController.LoadPlayerPositionInScene();

            OnInventoryListUpdate?.Invoke();
        }

        if (SceneManager.GetActiveScene().name.Contains("Farm") || SceneManager.GetActiveScene().name.Contains("Water"))
        {
            if (dataToKeep != null) dataLoaded = true;

            if (animalTypeToKeep != AnimalType.None && mgType == MGType.Capture) animalCaptured = true;
        }
    }

    #region Handle Player Items

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

    public static void ClearPlayerItems(bool stateItemsToRemove, bool statePlayerItems)
    {
        if (stateItemsToRemove) itemsToRemoveQuantity.Clear();
        if (statePlayerItems) playerItems.Clear();
    }

    #endregion

    #region Open Inventories

    public static void AddOpenInventory(GameObject inventory)
    {
        if (!openInventories.Contains(inventory))
            openInventories.Add(inventory);

        OnInventoryListUpdate?.Invoke();
    }

    public static void RemoveOpenInventory(GameObject inventory)
    {
        if (openInventories.Contains(inventory))
            openInventories.Remove(inventory);

        for (int i = 0; i < openInventories.Count; i++)
        {
            if (openInventories[i] == null) openInventories.Remove(openInventories[i]);
        }

        OnInventoryListUpdate?.Invoke();
    }

    public static void CleanOpenInventories()
    {
        openInventories.Clear();
    }

    public static void OnInventoryOpen()
    {
        if (openInventories.Count > 0)
        {
            playerController.HandlePlayerMovement(false);

            playerController.ReturnToMenuButton.gameObject.SetActive(false);

            HandleCursor(true);
        }
        else
        {
            playerController.HandlePlayerMovement(true);

            playerController.ReturnToMenuButton.gameObject.SetActive(true);

            HandleCursor(false);
        }
    }

    private static void HandleCursor(bool state)
    {
        if (state)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
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
            listSlots = FindObjectOfType<ListSlots>();

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
        if (dataToKeep == null || dataToKeep.Count == 0) return;

        PlayerInventory inventory = listSlots.PlayerSlotsParent.GetComponent<PlayerInventory>();

        int firstDishDataSO = FindFirstDishData();

        if (inventory == null || firstDishDataSO == -1 || itemsToRemoveQuantity.Count == 0)
        {
            ClearPlayerItems(true, true);
            return;
        }

        Recipe recipe = (Recipe)dataToKeep[0];
        float price = (float)dataToKeep[1];

        if (recipe == null)
        {
            ClearPlayerItems(true, true);
            return;
        }

        Item item = (Item)listSlots.Stuffs[firstDishDataSO + Convert.ToInt32(recipe.recipeIndex)];

        if (item == null)
        {
            ClearPlayerItems(true, true);
            return;
        }

        DishInfos dish = recipe.finalProduct.GetComponent<DishInfos>();

        if (dish == null)
        {
            ClearPlayerItems(true, true);
            return;
        }

        item.itemName = dish.dishName;
        item.itemDescription = dish.dishDescription;
        item.itemSprite = dish.dishSprite;
        item.itemObject = dish.gameObject;

        inventory.AddItemToInventory(item, 1, (float)price);

        HandleItemsInInventory(inventory);
    }

    private int FindFirstDishData()
    {
        for (int i = 0; i < listSlots.Stuffs.Length; i++)
        {
            ScriptableObject itemSO = listSlots.Stuffs[i];

            if (itemSO.GetType() == typeof(Item))
            {
                Item item = (Item)itemSO;

                if (item.itemType == ItemType.Dish) return i;
            }
        }

        return -1;
    }

    private void HandleItemsInInventory(PlayerInventory inventory)
    {
        foreach (Item item in itemsToRemoveQuantity.Keys)
        {
            inventory.RemoveItemQuantity(item, itemsToRemoveQuantity[item]);
        }

        ClearPlayerItems(true, true);
    }

    private void HandleRecognitionData()
    {
        if (dataToKeep == null || dataToKeep.Count == 0) return;

        listSlots.UpdateMoney(listSlots.PlayerControl.Money + Convert.ToInt32(dataToKeep[0]));

        dataToKeep.Clear();
    }

    private void HandleBreedingData()
    {
        if (dataToKeep == null || dataToKeep.Count == 0 || mgType != MGType.Breeding) return;

        int nbChildrenToInstantiate = Convert.ToInt32(dataToKeep[0]);

        if (nbChildrenToInstantiate > 0)
        {
            AnimalPenManager animalPenManager = FindObjectOfType<AnimalPenManager>();
            AnimalType currentAnimalType = animalTypeToKeep;

            animalTypeToKeep = AnimalType.None;
            dataToKeep.Clear();

            if (animalPenManager.CheckAnimalPenRestrictions(currentAnimalType, true))
            {
                AnimalPenManager.AnimalPen linkedAnimalPen = animalPenManager.GetLinkedAnimalPen(currentAnimalType);
                AnimalPenManager.AnimalPenStates currentAnimalStates = animalPenManager.GetCurrentRestrictions(linkedAnimalPen);

                int currentChildren = animalPenManager.GetAnimalsCount(linkedAnimalPen.animalPenInScene.transform)[1];

                if (nbChildrenToInstantiate > currentAnimalStates.maxChildrenRestriction)
                {
                    nbChildrenToInstantiate = currentAnimalStates.maxChildrenRestriction;
                }

                if (currentChildren > 0 && currentChildren + nbChildrenToInstantiate > currentAnimalStates.maxChildrenRestriction)
                {
                    nbChildrenToInstantiate = currentAnimalStates.maxChildrenRestriction - currentChildren;
                }

                for (int i = 0; i < nbChildrenToInstantiate; i++)
                {
                    animalTypeToKeep = currentAnimalType;
                    animalPenManager.InstantiateTamedAnimalInAnimalPen(true);
                }
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
            HandleCursor(true);

            returnToFarm = false;
            SceneManager.LoadScene(specificScene);
        }
        else
        {
            returnToFarm = true;
            SceneManager.LoadScene(sceneToLoad);
        }
    }

    public static void ReturnToMenu()
    {
        FindObjectOfType<ListSlots>().SaveData();

        SceneManager.LoadScene(menuScene);
    }
}