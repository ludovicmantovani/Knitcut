using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CaptureManager : MonoBehaviour
{
    public static CaptureManager instance;

    [Header("References")]
    [SerializeField] private GameObject interactionUI;
    [SerializeField] private GameObject captureUI;
    [SerializeField] private GameObject captureContentUI;
    [SerializeField] private Transform fruitPedestal;
    [SerializeField] private GameObject area;
    [SerializeField] private List<GameObject> animals;
    [SerializeField] private List<Transform> spawnpoints;

    [SerializeField] private string[] captureGameSceneName = {
        "WaterMiniGameScene1",
        "WaterMiniGameScene2",
        "WaterMiniGameScene3"
    };

    private DraggableItem currentFruit;

    [Header("Datas")]
    [SerializeField] private bool zoneDetected;
    [SerializeField] private bool animalDetected;
    [SerializeField] private bool captureInventoryInUse;
    [SerializeField] private GameObject wildAnimal;
    [SerializeField] private GameObject fruitPlaced;
    [SerializeField] private float timeAnimalLife = 20f;

    private bool isCapturing;
    private string instruction;
    private PlayerInput playerInput;
    private PlayerController playerController;
    private List_Slots LS;

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
        LS = FindObjectOfType<List_Slots>();

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

        if (fruitPlaced != null) return;

        fruitPlaced = Instantiate(item.Item.itemObject, transform);
        currentFruit = item;

        Vector3 fruitPosition = fruitPedestal.position;
        fruitPosition.y += 1f;

        fruitPlaced.transform.position = fruitPosition;
    }

    private void HandleCapture()
    {
        if (animalDetected && fruitPlaced != null)
        {
            instruction = $"Utiliser {playerInput.InteractionAction.GetBindingDisplayString()} pour capturer l'animal";
            interactionUI.GetComponentInChildren<Text>().text = instruction;

            interactionUI.SetActive(true);
        }
        else
            interactionUI.SetActive(false);

        if (!isCapturing && animalDetected && currentFruit != null && playerInput.InteractionAction.triggered && captureGameSceneName.Length > 0)
        {
            Debug.Log($"D�but de la capture...");
            isCapturing = true;

            RemoveItem();
            Destroy(currentFruit.gameObject);

            captureUI.SetActive(false);
            MinigameManager.CleanOpenInventories();

            AnimalAI animal = wildAnimal.GetComponent<AnimalAI>();

            MinigameManager.FinalizeMG(MinigameManager.MGType.Capture, animal.name, animal.AnimalType);

            playerController.SavePlayerPositionInScene();

            StartCoroutine(Saving());
            /*string sceneToLoad = captureGameSceneName[Random.Range(0, captureGameSceneName.Length)];

            SceneManager.LoadScene(sceneToLoad);*/
        }
    }

    private IEnumerator Saving()
    {
        yield return new WaitForSeconds(0.5f);

        LS.SaveData();

        yield return new WaitForSeconds(0.5f);

        string sceneToLoad = captureGameSceneName[Random.Range(0, captureGameSceneName.Length)];

        MinigameManager.SwitchScene(sceneToLoad);
    }

    private DraggableItem GetItemData()
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