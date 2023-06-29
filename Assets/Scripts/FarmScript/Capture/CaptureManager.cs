using Gameplay.UI.Quests;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gameplay.Quests;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class CaptureManager : MonoBehaviour, IDropHandler
{
    public static CaptureManager instance;

    [Header("References")]
    [SerializeField] private GameObject interactionUI;
    [SerializeField] private GameObject infosUI;
    [SerializeField] private Transform fruitPedestal;
    [SerializeField] private GameObject area;
    [SerializeField] private List<GameObject> animals;
    [SerializeField] private List<Transform> spawnPoints;

    [SerializeField]
    private string[] captureGameSceneName = {
        "WaterMiniGameScene1",
        "WaterMiniGameScene2",
        "WaterMiniGameScene3"
    };

    private Item currentFruit;

    [Header("Datas")]
    [SerializeField] private bool zoneDetected;
    [SerializeField] private bool animalDetected;
    [SerializeField] private bool playerIsHidden;
    [SerializeField] private bool canCheckAnimal;
    [SerializeField] private bool animalPenRestriction;
    [SerializeField] private AnimalAI wildAnimalAttracted;
    [SerializeField] private int maxWildsAnimals = 3;
    [SerializeField] private List<GameObject> wildsAnimals;
    [SerializeField] private GameObject fruitPlaced;

    [Header("Quest")]
    [SerializeField] private string questCompletionPlaceFruit = "";

    private string instruction;
    private bool searchWildAnimal;
    private PlayerInput playerInput;
    private PlayerController playerController;
    private ListSlots listSlots;
    private AnimalPenManager animalPenManager;

    #region Getters / Setters

    public bool ZoneDetected
    {
        get { return zoneDetected; }
        set { zoneDetected = value; }
    }

    public bool AnimalDetected
    {
        get { return animalDetected; }
        set { animalDetected = value; }
    }

    public bool PlayerIsHidden
    {
        get { return playerIsHidden; }
        set { playerIsHidden = value; }
    }

    public List<GameObject> WildsAnimals
    {
        get { return wildsAnimals; }
        set { wildsAnimals = value; }
    }

    public AnimalAI WildAnimalAttracted
    {
        get { return wildAnimalAttracted; }
        set { wildAnimalAttracted = value; }
    }

    public GameObject FruitPlaced
    {
        get { return fruitPlaced; }
        set { fruitPlaced = value; }
    }

    public Item CurrentFruit
    {
        get { return currentFruit; }
        set { currentFruit = value; }
    }

    #endregion

    private void Start()
    {
        playerInput = FindObjectOfType<PlayerInput>();
        playerController = FindObjectOfType<PlayerController>();
        listSlots = FindObjectOfType<ListSlots>();
        animalPenManager = FindObjectOfType<AnimalPenManager>();

        wildsAnimals = new List<GameObject>();

        zoneDetected = false;
        animalDetected = false;
        playerIsHidden = false;
        canCheckAnimal = false;
        animalPenRestriction = false;

        searchWildAnimal = false;
        
        infosUI.SetActive(false);
        
        instance = this;
    }

    private void Update()
    {
        HandleAnimals();

        RecoverFruitPlacedOnClick();

        HandleCapture();
    }

    #region Handle Animals

    private void HandleAnimals()
    {
        if (wildsAnimals.Count < maxWildsAnimals)
        {
            if (wildAnimalAttracted == null && animalDetected) animalDetected = false;

            for (int i = 0; i < maxWildsAnimals; i++)
            {
                if (CurrentQuestIsCapture())
                    SpawnSpecificAnimal(AnimalType.Fox);
                else
                    SpawnRandomAnimal();
            }
        }
    }

    private bool CurrentQuestIsCapture()
    {
        bool isQuestCapture = false;
        
        QuestManager questManager = QuestManager.Instance;
        if (questManager == null) return isQuestCapture;

        QuestStatus questStatus = questManager.GetCurrentQuestStatus();
        if (questStatus == null) return isQuestCapture;

        Quest quest = questStatus.GetQuest();
        if (quest == null) return isQuestCapture;

        if (quest.GetTitle().Contains("bébé")) isQuestCapture = true;

        return isQuestCapture;
    }

    private void SpawnRandomAnimal()
    {
        if (animals.Count == 0 || wildsAnimals.Count >= maxWildsAnimals) return;

        int randomAnimalIndex = Random.Range(0, animals.Count);

        GameObject randomAnimal = animals[randomAnimalIndex];

        if (randomAnimal == null) return;
        
        // Verify if new random animal is aleady here
        if (AnimalIsInArea(randomAnimal) != null)
        {
            SpawnRandomAnimal();
            return;
        }

        if (spawnPoints.Count == 0) return;

        int randomSapwnpointIndex = Random.Range(0, spawnPoints.Count);
        Transform randomSpawnpoint = spawnPoints[randomSapwnpointIndex];

        if (randomAnimal == null || randomSpawnpoint == null) return;

        GameObject wildAnimal = Instantiate(randomAnimal, randomSpawnpoint);
        wildAnimal.GetComponent<AnimalAI>().Area = area;
        
        wildsAnimals.Add(wildAnimal);
    }

    private void SpawnSpecificAnimal(AnimalType animalType)
    {
        for (int i = 0; i < animals.Count; i++)
        {
            if (animals[i].GetComponent<AnimalAI>().AnimalType != animalType) return;
            
            if (spawnPoints.Count == 0) return;

            int randomSapwnpointIndex = Random.Range(0, spawnPoints.Count);
            Transform randomSpawnpoint = spawnPoints[randomSapwnpointIndex];

            if (animals[i] == null || randomSpawnpoint == null) return;

            GameObject wildAnimal = Instantiate(animals[i], randomSpawnpoint);
            wildAnimal.GetComponent<AnimalAI>().Area = area;
    
            wildsAnimals.Add(wildAnimal);
        }
    }

    private GameObject AnimalIsInArea(GameObject animal)
    {
        for (int i = 0; i < wildsAnimals.Count; i++)
        {
            if (wildsAnimals[i].name.Contains(animal.name)) return wildsAnimals[i];
        }

        return null;
    }

    #endregion
    
    #region Handle Fruit

    public void LoadFruitPlaced(int fruitIndex)
    {
        if (fruitIndex == -1) return;

        Item fruitItem = (Item)listSlots.GetItemByIndex(fruitIndex);
        
        HandleItemOnPedestal(fruitItem);

        wildAnimalAttracted = null;
    }

    private void RecoverFruitPlacedOnClick()
    {
        if (fruitPlaced == null) 
            infosUI.SetActive(false);
        else
            infosUI.SetActive(true);
        
        if (Input.GetMouseButtonDown(0) && fruitPlaced != null && playerIsHidden)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, 1000f, 1 << 8))
            {
                if (hit.transform.GetComponent<KeepItem>())
                {
                    Item item = hit.transform.GetComponent<KeepItem>().Item;
                    
                    if (item != currentFruit) return;
                    
                    playerController.PlayerInventory.AddItemToInventory(currentFruit, 1);

                    RemoveItem();
                }
            }
        }
    }
    
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null && eventData.pointerDrag.GetComponent<ItemHandler>() && playerIsHidden)
        {
            ItemHandler itemHandler = eventData.pointerDrag.GetComponent<ItemHandler>();
            
            if (itemHandler.Item.itemType != ItemType.Consumable) return;
            
            HandleItemOnPedestal(itemHandler.Item);

            int quantity = itemHandler.QuantityStacked;

            quantity -= 1;

            if (quantity > 0)
            {
                playerController.PlayerInventory.AddItemToInventory(itemHandler.Item, quantity);
            }
            
            Destroy(itemHandler.gameObject);
        }
    }

    private void HandleItemOnPedestal(Item item)
    {
        if (item == null) return;

        if (fruitPlaced != null)
        {
            Item oldItem = fruitPlaced.GetComponent<KeepItem>().Item;
            playerController.PlayerInventory.AddItemToInventory(oldItem, 1);

            RemoveItem();
        }

        fruitPlaced = Instantiate(item.itemObject, transform);

        DeactivateFruitOptions();

        currentFruit = item;

        Vector3 fruitPosition = fruitPedestal.position;
        fruitPosition.y += 1f;

        fruitPlaced.transform.position = fruitPosition;

        if (questCompletionPlaceFruit.Length > 0)
            QuestManager.Instance.CompleteObjective(questCompletionPlaceFruit);
    }

    private void DeactivateFruitOptions()
    {
        DestroyImmediate(fruitPlaced.GetComponent<Rigidbody2D>());
        DestroyImmediate(fruitPlaced.GetComponent<CircleCollider2D>());
        Destroy(fruitPlaced.GetComponent<Consumable3D>());

        fruitPlaced.AddComponent<SphereCollider>();
    }

    public void RemoveItem()
    {
        Destroy(fruitPlaced);
        fruitPlaced = null;
        currentFruit = null;

        if (canCheckAnimal) canCheckAnimal = false;

        wildAnimalAttracted = null;
    }
    
    #endregion
    
    #region Handle Capture

    private AnimalAI GetAnimalAttracted()
    {
        searchWildAnimal = true;

        AnimalAI animal = null;
        
        for (int i = 0; i < wildsAnimals.Count; i++)
        {
            AnimalAI wildAnimal = wildsAnimals[i].GetComponent<AnimalAI>();

            Item fruitItem = null;
            
            if (fruitPlaced != null)
                fruitItem = fruitPlaced.GetComponent<KeepItem>().Item;

            if (fruitItem == wildAnimal.FavoriteFruit)
                animal = wildAnimal;
        }

        searchWildAnimal = false;
        
        return animal;
    }
    
    private void HandleCapture()
    {
        if (zoneDetected)
        {
            if (fruitPlaced == null)
            {
                if (animalPenRestriction)
                    StartCoroutine(ShowAnimalPenRestriction());
                else
                    instruction = $"Placer un fruit sur la souche au centre";
            }
            else
            {
                if (!searchWildAnimal && wildAnimalAttracted == null)
                    wildAnimalAttracted = GetAnimalAttracted();

                if (wildAnimalAttracted == null) return;

                if (canCheckAnimal && animalDetected)
                        CheckPlayerNearAnimal(wildAnimalAttracted);
                else if (!canCheckAnimal)
                {
                    if (playerIsHidden)
                    {
                        instruction = $"Eloignez-vous le temps qu'un animal soit attiré par le fruit";

                        wildAnimalAttracted.CanBeAttracted = false;
                        wildAnimalAttracted.IsAttracted = false;

                        if (wildAnimalAttracted.PlayerIsNear || wildAnimalAttracted.PlayerIsBehind)
                            AnimalRunAway(wildAnimalAttracted, false);
                    }
                    else if (!playerIsHidden)
                    {
                        wildAnimalAttracted.CanBeAttracted = true;

                        if (animalDetected && wildAnimalAttracted.IsAttracted)
                        {
                            bool animalPenRestrictionsOK = animalPenManager.CheckAnimalPenRestrictions((wildAnimalAttracted));
                    
                            if (!animalPenRestrictionsOK)
                            {
                                animalPenRestriction = true;
                                
                                AnimalRunAway(wildAnimalAttracted);
                            }
                            else
                                canCheckAnimal = true;
                        }
                        
                        if (animalPenRestriction)
                            instruction = $"L'enclos associé à l'animal n'est pas assez grand";
                        else
                            instruction = $"Attendez qu'un animal soit attiré au centre";
                    }
                }
            }
            
            interactionUI.GetComponentInChildren<TMP_Text>().text = instruction;
            interactionUI.SetActive(true);
        }
        else
            interactionUI.SetActive(false);
    }

    private IEnumerator ShowAnimalPenRestriction()
    {
        yield return new WaitForSeconds(2f);

        animalPenRestriction = false;
    }

    private void CheckPlayerNearAnimal(AnimalAI animal)
    {
        if (animal == null) return;

        instruction = $"Un animal a été attiré, approchez-vous derrière lui puis appuyez sur {playerInput.InteractionAction.GetBindingDisplayString()} pour le capturer";
        
        if (animal.PlayerIsNear)
        {
            if (animal.PlayerIsBehind)
            {
                if (playerInput.InteractionAction.triggered)
                    StartCapture(animal);
            }
            else
                AnimalRunAway(animal);
        }
    }

    private void StartCapture(AnimalAI animal)
    {
        instruction = "Début de la capture";
        interactionUI.GetComponentInChildren<TMP_Text>().text = instruction;

        RemoveItem();

        GameManager.CleanOpenInventories();

        GameManager.FinalizeMG(GameManager.MGType.Capture, animal.name, animal.AnimalType);

        playerController.SavePlayerPositionInScene();

        StartCoroutine(Saving());
    }

    private void AnimalRunAway(AnimalAI animal, bool eatFruit = true)
    {
        animal.ForceRunAway(eatFruit);

        canCheckAnimal = false;
    }

    private IEnumerator Saving()
    {
        yield return new WaitForSeconds(0.2f);

        listSlots.SaveData();

        yield return new WaitForSeconds(0.2f);

        string sceneToLoad = captureGameSceneName[Random.Range(0, captureGameSceneName.Length)];

        GameManager.SwitchScene(sceneToLoad);
    }
    
    #endregion
}