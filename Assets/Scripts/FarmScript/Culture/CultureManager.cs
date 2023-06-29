using Gameplay.UI.Quests;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CultureManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject interactionUI;
    [SerializeField] private GameObject itemUI;
    [SerializeField] private GameObject cultureUI;
    [SerializeField] private CropPlot currentCropPlot;

    private PlayerInput playerInput;
    private PlayerController playerController;

    private int cropsCount;
    private List<CropPlot> crops;
    private int[] cropsSeeds;
    private bool[] cropsCultivation;
    private string[] cropsGrowth;
    private string[] cropsState;
    private float[] cropsTimer;
    private int seedIndexInSlot;
    private int seedQuantityInSlot;

    [Header("Datas")]
    [SerializeField] private bool inCultureArea;
    [SerializeField] private bool canPlantSeed;

    [Header("Quest")]
    [SerializeField] private string questCompletionPlant = "";
    [SerializeField] private string questCompletionPick = "";

    private string instruction;

    [Serializable]
    public class SeedToHandle
    {
        public Item item;
        public int price;
    }

    #region Getters / Setters

    public int CropsCount
    {
        get { return cropsCount; }
        set { cropsCount = value; }
    }

    public int[] CropsSeeds
    {
        get { return cropsSeeds; }
        set { cropsSeeds = value; }
    }

    public bool[] CropsCultivation
    {
        get { return cropsCultivation; }
        set { cropsCultivation = value; }
    }

    public string[] CropsGrowth
    {
        get { return cropsGrowth; }
        set { cropsGrowth = value; }
    }

    public string[] CropsState
    {
        get { return cropsState; }
        set { cropsState = value; }
    }

    public float[] CropsTimer
    {
        get { return cropsTimer; }
        set { cropsTimer = value; }
    }

    public int SeedIndexInSlot
    {
        get { return seedIndexInSlot; }
        set { seedIndexInSlot = value; }
    }

    public int SeedQuantityInSlot
    {
        get { return seedQuantityInSlot; }
        set { seedQuantityInSlot = value; }
    }

    public GameObject InteractionUI
    {
        get { return interactionUI; }
        set { interactionUI = value; }
    }

    public PlayerInput PlayerInput
    {
        get { return playerInput; }
        set { playerInput = value; }
    }

    public CropPlot CurrentCropPlot
    {
        get { return currentCropPlot; }
        set { currentCropPlot = value; }
    }

    public string Instruction
    {
        get { return instruction; }
        set { instruction = value; }
    }

    public bool CanPlantSeed
    {
        get { return canPlantSeed; }
        set { canPlantSeed = value; }
    }

    public bool InCultureArea
    {
        get { return inCultureArea; }
        set { inCultureArea = value; }
    }

    #endregion

    private void Start()
    {
        playerInput = FindObjectOfType<PlayerInput>();
        playerController = FindObjectOfType<PlayerController>();

        instruction = $"Utiliser {playerInput.InteractionAction.GetBindingDisplayString()} pour planter une graine";
        interactionUI.GetComponentInChildren<TMP_Text>().text = instruction;

        SetupCulture();
        LoadCulture();
    }

    private void Update()
    {
        HandlePlantSeedUI();
        HandleCropPlotState();

        HandleCultureData();
    }

    private void SetupCulture()
    {
        cropsCount = transform.childCount;

        crops = new List<CropPlot>();

        cropsSeeds = new int[cropsCount];

        cropsCultivation = new bool[cropsCount];

        cropsGrowth = new string[cropsCount];
        cropsState = new string[cropsCount];

        cropsTimer = new float[cropsCount];

        seedIndexInSlot = -1;
        seedQuantityInSlot = 0;

        for (int i = 0; i < cropsCount; i++)
        {
            CropPlot crop = transform.GetChild(i).GetComponent<CropPlot>();

            if (!crops.Contains(crop)) crops.Add(crop);
        }

        cultureUI.SetActive(false);
    }

    private void HandleCultureData()
    {
        for (int i = 0; i < cropsCount; i++)
        {
            CropPlot crop = crops[i];

            if (crop == null) return;

            if (crop.IsCultivating)
            {
                if (crop.SeedSource == null) return;

                if (!crop.SeedSource.TryGetComponent<PlantGrowth>(out var plantGrowth)) return;

                cropsSeeds[i] = GetPlantIndex(plantGrowth.CurrentPlant);
                cropsCultivation[i] = true;
                cropsGrowth[i] = plantGrowth.GetProductGrowth.ToString();
                cropsState[i] = plantGrowth.GetProductState.ToString();
                cropsTimer[i] = plantGrowth.CurrentTimer;
            }
            else
            {
                cropsSeeds[i] = -1;
                cropsCultivation[i] = false;
                cropsGrowth[i] = string.Empty;
                cropsState[i] = string.Empty;
                cropsTimer[i] = 0f;
            }
        }
    }

    public void SaveCulture()
    {
        SaveItemUI();

        SaveSystem.Save(SaveSystem.SaveType.Save_Culture, this);
    }

    private void SaveItemUI()
    {
        ItemHandler itemHandler = GetItemUI();

        if (itemHandler != null)
        {
            int itemIndex = playerController.ListSlots.GetItemIndex(itemHandler.Item);

            seedIndexInSlot = itemIndex;
            seedQuantityInSlot = itemHandler.QuantityStacked;
        }
        else
        {
            seedIndexInSlot = -1;
            seedQuantityInSlot = 0;
        }
    }

    public void LoadCulture()
    {
        Culture_Data data = (Culture_Data)SaveSystem.Load(SaveSystem.SaveType.Save_Culture, this);

        if (data == null) return;

        cropsSeeds = data.cropsSeeds;
        cropsCultivation = data.cropsCultivation;
        cropsGrowth = data.cropsGrowth;
        cropsState = data.cropsState;
        cropsTimer = data.cropsTimer;

        seedIndexInSlot = data.seedIndexInSlot;
        seedQuantityInSlot = data.seedQuantityInSlot;

        if (seedIndexInSlot != -1)
        {
            Item item = (Item)playerController.ListSlots.Stuffs[seedIndexInSlot];

            if (item == null) return;

            AddItem(item, seedQuantityInSlot);
        }

        for (int i = 0; i < cropsCount; i++)
        {
            if (cropsCultivation[i])
            {
                CropPlot crop = crops[i];

                crop.IsCultivating = true;

                Plant plant = (Plant)playerController.ListSlots.Stuffs[cropsSeeds[i]];

                PlantSpectificPlantAtCropPlot(plant, crop, cropsGrowth[i], cropsState[i], cropsTimer[i]);
            }
        }
    }

    private int GetPlantIndex(Plant plant)
    {
        for (int i = 0; i < playerController.ListSlots.Stuffs.Length; i++)
        {
            if (playerController.ListSlots.Stuffs[i] == plant)
            {
                return i;
            }
        }

        return -1;
    }

    #region Handle UI

    private void HandlePlantSeedUI()
    {
        if (inCultureArea)
            cultureUI.SetActive(true);
        else
            cultureUI.SetActive(false);

        if (canPlantSeed)
            interactionUI.SetActive(true);
        else
            interactionUI.SetActive(false);
    }

    private void HandleCropPlotState()
    {
        if (currentCropPlot == null) return;

        if (!currentCropPlot.IsCultivating)
        {
            instruction = $"Utiliser {playerInput.InteractionAction.GetBindingDisplayString()} pour planter une graine";
            interactionUI.GetComponentInChildren<TMP_Text>().text = instruction;

            if (playerInput.InteractionAction.triggered)
                PlantSeedInSlot();
        }
        else if (currentCropPlot.IsCultivating && currentCropPlot.Product == null)
        {
            if (currentCropPlot.SeedSource == null) return;

            PlantGrowth.ProductState plantProductState = currentCropPlot.SeedSource.GetComponent<PlantGrowth>().GetProductState;

            if (plantProductState == PlantGrowth.ProductState.Sick)
                instruction = $"Utiliser {playerInput.HealAction.GetBindingDisplayString()} pour soigner la plante";
            else if (plantProductState == PlantGrowth.ProductState.Dehydrated)
                instruction = $"Utiliser {playerInput.HydrateAction.GetBindingDisplayString()} pour hydrater la plante";
            else
                instruction = $"Croissance en cours...";

            interactionUI.GetComponentInChildren<TMP_Text>().text = instruction;
        }
        else if (currentCropPlot.IsCultivating && currentCropPlot.Product != null)
        {
            if (playerController.PlayerInventory.InventoryIsFull())
            {
                instruction = $"Croissance termin�e\nL'inventaire est plein";
                interactionUI.GetComponentInChildren<TMP_Text>().text = instruction;

                return;
            }
            
            instruction = $"Croissance termin�e\nUtiliser {playerInput.InteractionAction.GetBindingDisplayString()} pour ramasser le fruit";
            interactionUI.GetComponentInChildren<TMP_Text>().text = instruction;

            if (playerInput.InteractionAction.triggered/* && !playerController.PlayerInventory.InventoryIsFull()*/)
            {
                Item fruitProduced = currentCropPlot.Product.GetComponent<KeepItem>().Item;

                GameObject seed = currentCropPlot.SeedSource.GetComponent<PlantGrowth>().CurrentPlant.seed;
                Item seedSource = seed.GetComponent<KeepItem>().Item;
                
                if (questCompletionPick.Length > 0)
                    QuestManager.Instance.CompleteObjective(questCompletionPick);

                playerController.PlayerInventory.AddItemToInventory(fruitProduced);

                if (!playerController.PlayerInventory.InventoryIsFull())
                    RandomToKeepSeedsAfterPickUp(seedSource);
                
                Destroy(currentCropPlot.SeedSource);

                currentCropPlot.SeedSource = null;
                currentCropPlot.Product = null;
                currentCropPlot.IsCultivating = false;
            }
        }
    }

    private void RandomToKeepSeedsAfterPickUp(Item item)
    {
        int randomChance = UnityEngine.Random.Range(0, 3);

        if (randomChance == 0)
        {
            int randomQuantity = UnityEngine.Random.Range(0, 3);

            if (randomQuantity > 0)
            {
                for (int i = 0; i < randomQuantity; i++)
                {
                    playerController.PlayerInventory.AddItemToInventory(item);
                }
            }
        }
    }

    public void HandleCropPlot(CropPlot cropPlot, bool state)
    {
        canPlantSeed = state;

        if (canPlantSeed)
        {
            currentCropPlot = cropPlot;
        }
        else
        {
            currentCropPlot = null;
        }
    }

    #endregion

    private void PlantSeedInSlot()
    {
        Slot seedSlot = cultureUI.GetComponentInChildren<Slot>();

        if (seedSlot.transform.childCount == 0) return;

        Item seedToPlant = seedSlot.GetComponentInChildren<ItemHandler>().Item;

        if (seedToPlant.itemType != ItemType.Seed) return;

        canPlantSeed = false;

        PlantSeed(seedToPlant);
    }

    private void PlantSeed(Item item)
    {
        if (currentCropPlot == null) return;

        if (currentCropPlot != null && currentCropPlot.SeedSource != null) return;

        RemoveItem();

        GameObject plant = Instantiate(item.itemObject, currentCropPlot.transform);

        plant.GetComponent<PlantGrowth>().CropPlot = currentCropPlot;

        currentCropPlot.SeedSource = plant;

        if (questCompletionPlant.Length > 0)
            QuestManager.Instance.CompleteObjective(questCompletionPlant);
    }

    private void PlantSpectificPlantAtCropPlot(Plant plantSO, CropPlot currentCropPlot, string growth, string state, float timeSkip)
    {
        if (plantSO == null || currentCropPlot == null) return;

        GameObject plant = Instantiate(plantSO.source, currentCropPlot.transform);

        plant.GetComponent<PlantGrowth>().CropPlot = currentCropPlot;

        currentCropPlot.SeedSource = plant;

        plant.GetComponent<PlantGrowth>().SetGrowthState(growth, state, plantSO, timeSkip);
    }

    #region HandleItem

    private void RemoveItem()
    {
        Transform slot = cultureUI.GetComponentInChildren<Slot>().transform;

        // If item present in slot
        if (slot.childCount > 0)
        {
            ItemHandler itemHandler = slot.GetChild(0).GetComponent<ItemHandler>();

            itemHandler.QuantityStacked -= 1;

            if (itemHandler.QuantityStacked <= 0)
                Destroy(itemHandler.gameObject);
        }
    }

    private void AddItem(Item item, int quantity)
    {
        Transform slot = cultureUI.GetComponentInChildren<Slot>().transform;

        ItemHandler itemHandler = Instantiate(itemUI, slot).GetComponent<ItemHandler>();

        itemHandler.Item = item;
        itemHandler.QuantityStacked = quantity;
    }

    private ItemHandler GetItemUI()
    {
        Transform slot = cultureUI.GetComponentInChildren<Slot>().transform;

        if (slot.childCount > 0) return slot.GetComponentInChildren<ItemHandler>();
        else return null;
    }

    #endregion
}