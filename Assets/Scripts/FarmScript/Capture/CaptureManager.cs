using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CaptureManager : MonoBehaviour
{
    public static CaptureManager instance;

    [Header("References")]
    [SerializeField] private GameObject itemUI;
    [SerializeField] private GameObject interactionUI;
    [SerializeField] private GameObject captureUI;
    [SerializeField] private GameObject captureContentUI;
    [SerializeField] private Transform fruitPedestal;
    [SerializeField] private GameObject area;
    [SerializeField] private List<GameObject> animals;
    [SerializeField] private List<Transform> spawnpoints;

    [SerializeField]
    private string[] captureGameSceneName = {
        "WaterMiniGameScene1",
        "WaterMiniGameScene2",
        "WaterMiniGameScene3"
    };

    private DraggableItem currentFruit;

    [Header("Datas")]
    [SerializeField] private bool zoneDetected;
    [SerializeField] private bool animalDetected;
    [SerializeField] private GameObject wildAnimal;
    [SerializeField] private GameObject fruitPlaced;
    [SerializeField] private float timeAnimalLife = 20f;

    private bool isCapturing;
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

    #endregion

    private void Start()
    {
        playerInput = FindObjectOfType<PlayerInput>();
        playerController = FindObjectOfType<PlayerController>();
        listSlots = FindObjectOfType<ListSlots>();
        animalPenManager = FindObjectOfType<AnimalPenManager>();

        zoneDetected = false;
        animalDetected = false;

        isCapturing = false;

        instruction = $"Utiliser {playerInput.InteractionAction.GetBindingDisplayString()} pour placer un fruit";
        interactionUI.GetComponentInChildren<Text>().text = instruction;

        instance = this;
    }

    private void Update()
    {
        captureUI.SetActive(zoneDetected);

        HandleAnimals();

        HandleItemOnPedestal();

        HandleCapture();
    }

    #region Handle Animals

    private void HandleAnimals()
    {
        if (wildAnimal == null)
        {
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

        if (spawnpoints.Count == 0) return;

        int randomSapwnpointIndex = Random.Range(0, spawnpoints.Count);
        Transform randomSpawnpoint = spawnpoints[randomSapwnpointIndex];

        if (randomAnimal == null || randomSpawnpoint == null) return;

        wildAnimal = Instantiate(randomAnimal, randomSpawnpoint);
        wildAnimal.GetComponent<AnimalAI>().Area = area;

        StartCoroutine(AnimalLife());
    }

    private IEnumerator AnimalLife()
    {
        yield return new WaitForSeconds(timeAnimalLife);

        Destroy(wildAnimal);
        wildAnimal = null;
        animalDetected = false;
    }

    #endregion

    public void LoadFruitPlaced(int fruitIndex, int fruitQuantity)
    {
        if (fruitIndex == -1 && fruitQuantity <= 0) return;

        SetItemData(fruitIndex, fruitQuantity);
    }

    private void HandleItemOnPedestal()
    {
        DraggableItem item = GetItemData();

        if (item == null && fruitPlaced == null) return;
        if (item == null && fruitPlaced != null)
        {
            Destroy(fruitPlaced);
            fruitPlaced = null;
            currentFruit = null;
            return;
        }
        if (item != null && fruitPlaced != item.Item.itemObject)
        {
            Destroy(fruitPlaced);
            fruitPlaced = null;
            currentFruit = null;
        }

        if (fruitPlaced != null) return;

        fruitPlaced = Instantiate(item.Item.itemObject, transform);

        DeactivateFruitOptions();

        currentFruit = item;

        Vector3 fruitPosition = fruitPedestal.position;
        fruitPosition.y += 1f;

        fruitPlaced.transform.position = fruitPosition;
    }

    private void DeactivateFruitOptions()
    {
        Destroy(fruitPlaced.GetComponent<Rigidbody2D>());
        Destroy(fruitPlaced.GetComponent<CircleCollider2D>());
        Destroy(fruitPlaced.GetComponent<Consumable3D>());
    }

    private void HandleCapture()
    {
        if (zoneDetected && animalDetected && fruitPlaced != null)
        {
            if (animalPenManager.CheckAnimalPenRestrictions(wildAnimal.GetComponent<AnimalAI>()))
                instruction = $"Utiliser {playerInput.InteractionAction.GetBindingDisplayString()} pour capturer l'animal";
            else
                instruction = "L'enclos associé n'est pas assez grand";

            interactionUI.GetComponentInChildren<Text>().text = instruction;

            interactionUI.SetActive(true);
        }
        else
            interactionUI.SetActive(false);

        if (!isCapturing && zoneDetected && animalDetected && currentFruit != null && playerInput.InteractionAction.triggered && captureGameSceneName.Length > 0)
        {
            if (animalPenManager.CheckAnimalPenRestrictions(wildAnimal.GetComponent<AnimalAI>()))
            {
                instruction = "Début de la capture";
                interactionUI.GetComponentInChildren<Text>().text = instruction;

                isCapturing = true;

                RemoveItem();
                Destroy(currentFruit.gameObject);

                captureUI.SetActive(false);
                MinigameManager.CleanOpenInventories();

                AnimalAI animal = wildAnimal.GetComponent<AnimalAI>();

                MinigameManager.FinalizeMG(MinigameManager.MGType.Capture, animal.name, animal.AnimalType);

                playerController.SavePlayerPositionInScene();

                StartCoroutine(Saving());
            }
            else
            {
                RemoveItem();
                Destroy(currentFruit.gameObject);
            }
        }
    }

    private IEnumerator Saving()
    {
        yield return new WaitForSeconds(0.2f);

        listSlots.SaveData();

        yield return new WaitForSeconds(0.2f);

        string sceneToLoad = captureGameSceneName[Random.Range(0, captureGameSceneName.Length)];

        MinigameManager.SwitchScene(sceneToLoad);
    }

    public DraggableItem GetItemData()
    {
        Transform slot = captureContentUI.transform.GetChild(0);

        // If item present in slot
        if (slot.childCount > 0)
        {
            DraggableItem draggableItem = slot.GetChild(0).GetComponent<DraggableItem>();

            // If item is consumable
            if (draggableItem.Item.itemType == ItemType.Consumable && draggableItem.QuantityStacked > 0)
            {
                return draggableItem;
            }
        }

        return null;
    }

    private void SetItemData(int fruitIndex, int fruitQuantity)
    {
        Transform slot = captureContentUI.transform.GetChild(0);

        DraggableItem item = Instantiate(itemUI, slot).GetComponent<DraggableItem>();

        item.Item = (Item)listSlots.GetItemByIndex(fruitIndex);
        item.QuantityStacked = fruitQuantity;
    }

    public void RemoveItem()
    {
        Transform slot = captureContentUI.transform.GetChild(0);

        // If item present in slot
        if (slot.childCount > 0)
        {
            DraggableItem draggableItem = slot.GetChild(0).GetComponent<DraggableItem>();

            draggableItem.QuantityStacked -= 1;

            if (draggableItem.QuantityStacked > 0)
                playerController.PlayerInventory.AddItemToInventory(draggableItem.Item, draggableItem.QuantityStacked);

            Destroy(draggableItem.gameObject);
        }
    }
}