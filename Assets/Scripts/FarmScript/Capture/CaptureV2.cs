using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CaptureV2 : MonoBehaviour, IDropHandler
{
    public static CaptureV2 instance;

    [Header("References")]
    [SerializeField] private GameObject itemUI;
    [SerializeField] private GameObject interactionUI;
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
    [SerializeField] private GameObject wildAnimal;
    [SerializeField] private GameObject fruitPlaced;

    private string instruction;
    private PlayerInput playerInput;
    private PlayerController playerController;
    private ListSlots listSlots;
    private AnimalPenManager animalPenManager;

    private GameObject previousAnimal = null;

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

    public GameObject WildAnimal
    {
        get { return wildAnimal; }
        set { wildAnimal = value; }
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

        zoneDetected = false;
        animalDetected = false;
        playerIsHidden = false;
        canCheckAnimal = false;

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
        if (wildAnimal == null)
        {
            if (animalDetected) animalDetected = false;

            SpawnRandomAnimal();
        }
    }

    private void SpawnRandomAnimal()
    {
        if (animals.Count == 0) return;

        int randomAnimalIndex = Random.Range(0, animals.Count);

        GameObject randomAnimal = animals[randomAnimalIndex];

        // Verify if new random animal is same as previous
        if (previousAnimal != null && previousAnimal == randomAnimal)
        {
            SpawnRandomAnimal();
            return;
        }

        previousAnimal = randomAnimal;

        if (spawnPoints.Count == 0) return;

        int randomSapwnpointIndex = Random.Range(0, spawnPoints.Count);
        Transform randomSpawnpoint = spawnPoints[randomSapwnpointIndex];

        if (randomAnimal == null || randomSpawnpoint == null) return;

        wildAnimal = Instantiate(randomAnimal, randomSpawnpoint);
        wildAnimal.GetComponent<AnimalAI>().Area = area;
    }

    #endregion
    
    #region Handle Fruit

    public void LoadFruitPlaced(int fruitIndex)
    {
        if (fruitIndex == -1) return;

        Item fruitItem = (Item)listSlots.GetItemByIndex(fruitIndex);
        
        HandleItemOnPedestal(fruitItem);
    }

    private void RecoverFruitPlacedOnClick()
    {
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
    }
    
    #endregion
    
    #region Handle Capture

    private void HandleCapture()
    {
        if (zoneDetected)
        {
            if (fruitPlaced == null)
                instruction = $"Placer un fruit sur la souche au centre";
            else
            {
                AnimalAI animal = wildAnimal.GetComponent<AnimalAI>();
                /*if (animal != previousAnimal.GetComponent<AnimalAI>())
                    canCheckAnimal = false;*/

                if (canCheckAnimal && animalDetected)
                {
                    bool animalPenRestrictionsOK = animalPenManager.CheckAnimalPenRestrictions((animal));

                    Debug.Log($"animal pen restriction : {animalPenRestrictionsOK}");
                    
                    if (!animalPenRestrictionsOK)
                    {
                        instruction = "L'enclos associé à l'animal n'est pas assez grand";

                        AnimalRunAway(animal);
                    }
                    else
                        CheckPlayerNearAnimal(animal);
                }
                else if (!canCheckAnimal)
                {
                    if (playerIsHidden)
                    {
                        instruction = $"Eloignez-vous le temps qu'un animal soit attiré par le fruit";

                        animal.CanBeAttracted = false;
                        animal.IsAttracted = false;
                        
                        if (animal.PlayerIsNear || animal.PlayerIsBehind)
                            AnimalRunAway(animal, false);
                    }
                    else if (!playerIsHidden)
                    {
                        animal.CanBeAttracted = true;
                        
                        if (animalDetected && animal.IsAttracted)
                            canCheckAnimal = true;
                        
                        instruction = $"Attendez qu'un animal soit attiré au centre";
                    }
                }
            }
            
            interactionUI.GetComponentInChildren<Text>().text = instruction;
            interactionUI.SetActive(true);
        }
        else
            interactionUI.SetActive(false);
    }

    private void CheckPlayerNearAnimal(AnimalAI animal)
    {
        Debug.Log($"animal : {animal}");
        if (animal == null) return;

        instruction = $"Un(e) {animal.AnimalType} a été attiré(e), approchez-vous derrière lui pour le capturer en appuyant sur {playerInput.InteractionAction.GetBindingDisplayString()}!";
        
        Debug.Log($"check player animal : {instruction}");
        
        if (animal.PlayerIsNear)
        {
            if (animal.PlayerIsBehind)
            {
                if (playerInput.InteractionAction.triggered)
                    StartCapture(animal);
            }
            else
            {
                Debug.Log($"Run away eat fruit player not behind");
                AnimalRunAway(animal);
            }
        }
        /*else
            instruction = $"Un(e) {animal.AnimalType} a été attiré(e), approchez-vous derrière lui pour le capturer !";*/
    }

    private void StartCapture(AnimalAI animal)
    {
        instruction = "Début de la capture";
        interactionUI.GetComponentInChildren<Text>().text = instruction;

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