using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CaptureManager : MonoBehaviour
{
    public static CaptureManager instance;

    [Header("References")]
    [SerializeField] private GameObject interactionUI;
    [SerializeField] private GameObject pedestalUI;
    [SerializeField] private Transform fruitPedestal;
    [SerializeField] private GameObject area;
    [SerializeField] private List<GameObject> animals;
    [SerializeField] private List<Transform> spawnpoints;

    [SerializeField] private string[] captureGameSceneName = {
        "WaterMiniGameScene1",
        "WaterMiniGameScene2",
        "WaterMiniGameScene3"
    };

    [Header("Datas")]
    [SerializeField] private bool canPlaceFruit;
    [SerializeField] private bool canTryToCapture;
    [SerializeField] private bool pedestalInventoryInUse;
    [SerializeField] private GameObject wildAnimal;
    [SerializeField] private GameObject fruitPlaced;

    private string instruction;
    private PlayerInput playerInput;

    #region Getters / Setters

    public bool CanPlaceFruit
    {
        get { return canPlaceFruit; }
        set { canPlaceFruit = value; }
    }

    public bool CanTryToCapture
    {
        get { return canTryToCapture; }
        set { canTryToCapture = value; }
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

        canPlaceFruit = false;
        canTryToCapture = false;
        pedestalInventoryInUse = false;

        instruction = $"Utiliser {playerInput.InteractionAction.GetBindingDisplayString()} pour placer un fruit";
        interactionUI.GetComponentInChildren<Text>().text = instruction;

        instance = this;
    }

    private void Update()
    {
        HandlePlaceFruitUI();
        HandlePedestalInventory();

        HandleAnimals();

        HandleItemOnPedestal();

        HandleCapture();
    }

    #region Handle UI

    private void HandlePlaceFruitUI()
    {
        if (canPlaceFruit)
        {
            interactionUI.SetActive(true);
        }
        else
        {
            interactionUI.SetActive(false);
        }
    }

    private void HandlePedestalInventory()
    {
        pedestalUI.SetActive(pedestalInventoryInUse);

        if (!canPlaceFruit)
        {
            ClosePedestalInventory();
            return;
        }

        if (playerInput.InteractionAction.triggered && canPlaceFruit)
        {
            if (!pedestalInventoryInUse)
            {
                OpenPedestalInventory();
            }
            else
            {
                ClosePedestalInventory();
            }
        }
    }

    private void OpenPedestalInventory()
    {
        pedestalInventoryInUse = true;

        MinigameManager.AddOpenInventory(pedestalUI);
    }

    private void ClosePedestalInventory()
    {
        pedestalInventoryInUse = false;

        MinigameManager.RemoveOpenInventory(pedestalUI);
    }

    #endregion

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

        if (spawnpoints.Count == 0) return;

        int randomSapwnpointIndex = Random.Range(0, spawnpoints.Count);
        Transform randomSpawnpoint = spawnpoints[randomSapwnpointIndex];

        if (randomAnimal == null || randomSpawnpoint == null) return;

        wildAnimal = Instantiate(randomAnimal, randomSpawnpoint);
        wildAnimal.GetComponent<AnimalAI>().Area = area;
    }

    #endregion

    private void HandleItemOnPedestal()
    {
        Item item = GetItem();

        if (item == null && fruitPlaced == null) return;
        if (item == null && fruitPlaced != null)
        {
            Destroy(fruitPlaced);
            fruitPlaced = null;
            return;
        }

        if (fruitPlaced != null) return;

        fruitPlaced = Instantiate(item.itemObject, transform);

        Vector3 fruitPosition = fruitPedestal.position;
        fruitPosition.y += 1f;

        fruitPlaced.transform.position = fruitPosition;
    }

    private void HandleCapture()
    {
        if (canTryToCapture && playerInput.InteractionAction.triggered && captureGameSceneName.Length > 0)
        {
            AnimalAI animal = wildAnimal.GetComponent<AnimalAI>();

            MinigameManager.FinalizeMG(MinigameManager.MGType.Capture, animal.name, animal.AnimalType);

            SceneManager.LoadScene(
                captureGameSceneName[Random.Range(0, captureGameSceneName.Length)]
                );
        }
    }

    private Item GetItem()
    {
        Transform slot = pedestalUI.transform.GetChild(0);

        // If item present in slot
        if (slot.childCount > 0)
        {
            DraggableItem draggableItem = slot.GetChild(0).GetComponent<DraggableItem>();

            // If item is consumable
            if (draggableItem.Item.itemType == ItemType.Consumable && draggableItem.QuantityStacked > 0)
            {
                return draggableItem.Item;
            }
        }

        return null;
    }
}