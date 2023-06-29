using Gameplay.UI.Quests;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HoudiniEngineUnity;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    #region Parameters

    [Header("Data")]
    [SerializeField] private float refreshRate = 0.5f;
    
    [Header("Quest")]
    [SerializeField] private string questCompletionCaptureAnimal = "";

    private static string sceneToLoad = "FarmScene";
    private static string menuScene = "Menu";
    private static string switchSceneToLoad;

    private static int piecesGain = 0;

    private static bool returnToFarm = false;

    private static MGType mgType;
    private static AnimalType animalTypeToKeep = AnimalType.None;

    private static List<object> dataToKeep;
    private static List<int> animalPenIndexToUpgrade = new List<int>();
    private static List<Recipe> recipesPossessed;
    private static Dictionary<object, GameObject> openInventoriesDict;
    private static List<PlayerItem> playerItems = new List<PlayerItem>();
    private static List<string> tutorialsPlayed = new List<string>();

    private static Dictionary<Item, int> itemsToRemoveQuantity = new Dictionary<Item, int>();

    private static PlayerController playerController;

    private static event Action OnInventoryListUpdate;

    private ListSlots listSlots;

    private bool dataLoaded = false;
    private bool animalCaptured = false;
    private bool piecesGained = false;
    private bool switchingScene = false;

    public enum MGType
    {
        Cooking,
        Recognition,
        Breeding,
        Capture
    }

    #endregion

    #region Getters / Setters

    public static int PiecesGain
    {
        get { return piecesGain; }
        set { piecesGain = value; }
    }

    public static List<string> TutorialsPlayed
    {
        get { return tutorialsPlayed; }
        set { tutorialsPlayed = value; }
    }

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

    public static List<PlayerItem> PlayerItems
    {
        get { return playerItems; }
        set { playerItems = value; }
    }

    #endregion

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
            instance = this;
        
        DontDestroyOnLoad(this.gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoaded;

        OnInventoryListUpdate += OnInventoryOpen;
        //OnReturnToMenu += ReturnToMenu;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoaded;

        OnInventoryListUpdate -= OnInventoryOpen;
        //OnReturnToMenu -= ReturnToMenu;
    }
    
    private void Update()
    {
        if (SceneManager.GetActiveScene().name.Contains("Menu")) return;

        if ((SceneManager.GetActiveScene().name.Contains("Farm") ||
             SceneManager.GetActiveScene().name.Contains("Village"))
            && listSlots == null && FindObjectOfType<ListSlots>())
            listSlots = FindObjectOfType<ListSlots>();
        
        UpdateDataLoaded();

        if (playerController != null)
        {
            if (playerController.PlayerInput.CancelAction.triggered)
            {
                if (openInventoriesDict.Count == 0)
                    ReturnToMenu();
                else
                    CloseAllOpenInventories();
            }

            if (piecesGained)
            {
                piecesGained = false;
                
                listSlots.UpdateMoney(listSlots.PlayerControl.Money + piecesGain);
                
                piecesGain = 0;
            }
        }

        UpdateAnimalCaptured();

        #region Cheat Codes

        // Money
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            if (listSlots == null) return;
            listSlots.UpdateMoney(listSlots.PlayerControl.Money + 400);
        }

        // Plant
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            CultureManager culture = FindObjectOfType<CultureManager>();
            if (culture == null) return;
            if (culture.CurrentCropPlot == null) return;
            if (culture.CurrentCropPlot.SeedSource == null) return;
            PlantGrowth plant = culture.CurrentCropPlot.SeedSource.GetComponent<PlantGrowth>();
            if (plant == null) return;
            plant.SetGrowthState("End", "InGrowth", plant.CurrentPlant, 0);
        }

        // Capture Animal
        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            CaptureManager captureManager = FindObjectOfType<CaptureManager>();
            if (captureManager == null) return;
            if (captureManager.WildAnimalAttracted == null) return;
            animalTypeToKeep = captureManager.WildAnimalAttracted.GetComponent<AnimalAI>().AnimalType;
            captureManager.WildAnimalAttracted.EndLife();
            AnimalPenManager animalPenManager = FindObjectOfType<AnimalPenManager>();
            if (animalPenManager == null) return;
            animalPenManager.InstantiateTamedAnimalInAnimalPen();
        }

        #endregion
    }

    #region In Update

    private void UpdateDataLoaded()
    {
        if (dataLoaded)
        {
            dataLoaded = false;

            CheckItemsToAdd();
        }
    }

    private void UpdateAnimalCaptured()
    {
        if (animalCaptured)
        {
            if (mgType != MGType.Capture) return;

            animalCaptured = false;

            AnimalPenManager animalPenManager = FindObjectOfType<AnimalPenManager>();

            if (SceneManager.GetActiveScene().name.Contains("Farm") && animalPenManager == null) return;

            animalPenManager.InstantiateTamedAnimalInAnimalPen();

            animalTypeToKeep = AnimalType.None;

            if (questCompletionCaptureAnimal.Length > 0)
                QuestManager.Instance.CompleteObjective(questCompletionCaptureAnimal);
        }
    }

    #endregion

    private void OnLevelFinishedLoaded(Scene scene, LoadSceneMode sceneMode)
    {
        FindObjectOfType<FadeInOut>().FadeOut();
        
        openInventoriesDict = new Dictionary<object, GameObject>();

        if (OnInventoryListUpdate == null)
            OnInventoryListUpdate += OnInventoryOpen;
        
        if (SceneManager.GetActiveScene().name.Contains("Menu"))
        {
            HandleCursor(true);
            
            return;
        }
        
        playerController = FindObjectOfType<PlayerController>();
        
        if (playerController != null)
        {
            //if (returnToFarm) playerController.LoadPlayerPositionInScene();
            if (scene.name.Contains("Farm")) playerController.LoadPlayerPositionInScene();

            if (piecesGain > 0) piecesGained = true;
            
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

    private void CloseAllOpenInventories()
    {
        foreach (object script in openInventoriesDict.Keys.ToList())
        {
            if (script.GetType() == typeof(PlayerInventory))
            {
                PlayerInventory playerInventory = (PlayerInventory)script;
                playerInventory.CloseInventory();
            }
            else if (script.GetType() == typeof(PlayerRecipesInventory))
            {
                PlayerRecipesInventory playerRecipesInventory = (PlayerRecipesInventory)script;
                playerRecipesInventory.CloseInventory();
            }
            else if (script.GetType() == typeof(Container))
            {
                Container container = (Container)script;
                container.CloseContainerInventory();
            }
            else if (script.GetType() == typeof(Feeder))
            {
                Feeder feeder = (Feeder)script;
                feeder.CloseFeederInventory();
            }
            else if (script.GetType() == typeof(ShopManager))
            {
                ShopManager shopManager = (ShopManager)script;
                shopManager.CloseShopUI();
            }
        }
        
        HandleCursor(false);
    }

    public static void AddOpenInventory(object script, GameObject inventory)
    {
        if (!openInventoriesDict.Keys.Contains(script))
            openInventoriesDict.Add(script, inventory);

        OnInventoryListUpdate?.Invoke();
    }

    public static void RemoveOpenInventory(object script, GameObject inventory)
    {
        if (openInventoriesDict.Keys.Contains(script))
            openInventoriesDict.Remove(script);

        OnInventoryListUpdate?.Invoke();
    }

    public static void CleanOpenInventories()
    {
        openInventoriesDict.Clear();
    }

    private static void OnInventoryOpen()
    {
        if (openInventoriesDict.Count > 0)
        {
            playerController.HandlePlayerMovement(false);

            HandleCursor(true);
        }
        else
        {
            playerController.HandlePlayerMovement(true);

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
        if (listSlots == null) return;

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
        
    }

    private void HandleBreedingData()
    {
        if (dataToKeep == null || dataToKeep.Count == 0 || mgType != MGType.Breeding) return;

        int nbChildrenToInstantiate = Convert.ToInt32(dataToKeep[0]);
        
        if (nbChildrenToInstantiate <= 0) return;
        
        AnimalPenManager animalPenManager = FindObjectOfType<AnimalPenManager>();
        AnimalType currentAnimalType = animalTypeToKeep;

        animalTypeToKeep = AnimalType.None;
        dataToKeep.Clear();
        
        if (!animalPenManager.CheckAnimalPenRestrictions(currentAnimalType, true)) return;

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

    private void HandleCaptureData()
    {
        
    }

    #endregion

    #region Switch Scenes

    public static void SwitchScene(string specificScene = "")
    {
        if (specificScene != "")
        {
            HandleCursor(true);

            returnToFarm = false;
            switchSceneToLoad = specificScene;
        }
        else
        {
            returnToFarm = true;
            switchSceneToLoad = sceneToLoad;
        }
        
        instance.LoadScene(switchSceneToLoad);
    }

    public void ReturnToMenu()
    {
        if (listSlots == null) return;
        
        instance.LoadScene(menuScene);
    }

    public void LoadScene(string sceneToLoad)
    {
        if (switchingScene) return;
        
        StartCoroutine(LoadSceneAsync(sceneToLoad));
    }

    private IEnumerator LoadSceneAsync(string scene)
    {
        switchingScene = true;
        
        FindObjectOfType<FadeInOut>().FadeIn();
        
        yield return new WaitForSeconds(refreshRate);
        
        if (listSlots != null) listSlots.SaveData();

        AsyncOperation operation = SceneManager.LoadSceneAsync(scene);

        while (!operation.isDone)
        {
            yield return null;
        }
        
        FindObjectOfType<FadeInOut>().FadeOut();

        switchingScene = false;
    }

    #endregion
}