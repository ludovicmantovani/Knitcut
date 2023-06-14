using Gameplay.Quests;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CultureManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject interactionUI;
    [SerializeField] private GameObject cultureUI;
    [SerializeField] private GameObject seedToPlantUI;
    [SerializeField] private Transform seedsParent;
    [SerializeField] private CropPlot currentCropPlot;
    [SerializeField] private List<SeedToHandle> seeds;

    private PlayerInput playerInput;
    private PlayerController playerController;

    private int cropsCount;
    private List<CropPlot> crops;
    private int[] cropsSeeds;
    private bool[] cropsCultivation;
    public string[] cropsGrowth;
    public string[] cropsState;
    public float[] cropsTimer;

    [Header("Datas")]
    [SerializeField] private bool canPlantSeed;
    [SerializeField] private bool cultureUIInUse;

    [Header("Quest")]
    [SerializeField] private QuestCompletion questCompletionPlant;
    [SerializeField] private QuestCompletion questCompletionPick;

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

    public GameObject CultureUI
    {
        get { return cultureUI; }
        set { cultureUI = value; }
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

    #endregion

    private void Start()
    {
        playerInput = FindObjectOfType<PlayerInput>();
        playerController = FindObjectOfType<PlayerController>();

        instruction = $"Utiliser {playerInput.InteractionAction.GetBindingDisplayString()} pour planter une graine";
        interactionUI.GetComponentInChildren<Text>().text = instruction;

        SetupCulture();
        LoadCulture();
    }

    private void Update()
    {
        HandlePlantSeedUI();
        HandleCultureInventory();

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

        for (int i = 0; i < cropsCount; i++)
        {
            CropPlot crop = transform.GetChild(i).GetComponent<CropPlot>();

            if (!crops.Contains(crop)) crops.Add(crop);
        }
    }

    private void HandleCultureData()
    {
        for (int i = 0; i < cropsCount; i++)
        {
            CropPlot crop = crops[i];

            if (crop == null) return;

            if (crop.IsCultivating)
            {
                //PlantGrowth plantGrowth = crop.SeedSource.GetComponent<PlantGrowth>();
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
        SaveSystem.Save(SaveSystem.SaveType.Save_Culture, this);
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
        if (canPlantSeed)
        {
            interactionUI.SetActive(true);
        }
        else
        {
            interactionUI.SetActive(false);
        }
    }

    private void HandleCultureInventory()
    {
        cultureUI.SetActive(cultureUIInUse);

        HandleCropPlotState();

        if (cultureUIInUse && playerInput.CancelAction.triggered)
            CloseCultureUI();
    }

    private void HandleCropPlotState()
    {
        if (!cultureUIInUse)
        {
            if (currentCropPlot == null) return;

            if (!currentCropPlot.IsCultivating)
            {
                instruction = $"Utiliser {playerInput.InteractionAction.GetBindingDisplayString()} pour planter une graine";
                interactionUI.GetComponentInChildren<Text>().text = instruction;

                if (playerInput.InteractionAction.triggered)
                {
                    canPlantSeed = false;

                    OpenCultureUI();

                    HandleSeedsUI();
                }
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

                interactionUI.GetComponentInChildren<Text>().text = instruction;
            }
            else if (currentCropPlot.IsCultivating && currentCropPlot.Product != null)
            {
                instruction = $"Croissance terminée\nUtiliser {playerInput.InteractionAction.GetBindingDisplayString()} pour ramasser le fruit";
                interactionUI.GetComponentInChildren<Text>().text = instruction;

                if (playerInput.InteractionAction.triggered && !playerController.PlayerInventory.InventoryIsFull())
                {
                    Item item = currentCropPlot.Product.GetComponent<KeepItem>().Item;

                    if (questCompletionPick != null)
                        questCompletionPick.CompleteObjective();

                    playerController.PlayerInventory.AddItemToInventory(item);

                    Destroy(currentCropPlot.SeedSource);

                    currentCropPlot.SeedSource = null;
                    currentCropPlot.Product = null;
                    currentCropPlot.IsCultivating = false;
                }
            }
        }
    }

    private void OpenCultureUI()
    {
        cultureUIInUse = true;

        MinigameManager.AddOpenInventory(cultureUI);
    }

    public void CloseCultureUI()
    {
        cultureUIInUse = false;

        MinigameManager.RemoveOpenInventory(cultureUI);

        canPlantSeed = true;
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

    private void HandleSeedsUI()
    {
        Clear();

        Dictionary<Item, int> seeds = new Dictionary<Item, int>();

        seedsParent.GetComponent<Swipe>().ScrollPos = 1;

        for (int i = 0; i < playerController.PlayerInventory.SearchItemsPossessed().Count; i++)
        {
            ItemHandler itemPossessed = playerController.PlayerInventory.SearchItemsPossessed()[i];

            if (itemPossessed.Item.itemType == ItemType.Seed)
            {
                if (!seeds.ContainsKey(itemPossessed.Item))
                {
                    seeds.Add(itemPossessed.Item, itemPossessed.QuantityStacked);
                }
                else
                {
                    seeds.TryGetValue(itemPossessed.Item, out int currentQuantity);
                    seeds[itemPossessed.Item] = currentQuantity + itemPossessed.QuantityStacked;
                }
            }
        }

        foreach (Item item in seeds.Keys)
        {
            seeds.TryGetValue(item, out int quantity);

            ShowObjectUI(item, quantity);
        }
    }

    private void ShowObjectUI(Item item, int quantity)
    {
        InfosUIRefs infosUIRefs = Instantiate(seedToPlantUI, seedsParent).GetComponent<InfosUIRefs>();

        Sprite objectSprite = item.itemSprite;
        string objectName = item.itemName;
        float objectQuantity = quantity;

        infosUIRefs.ImageUI.sprite = item.itemSprite; ;
        infosUIRefs.NameUI.text = item.itemName;
        infosUIRefs.PriceUI.text = $"x{quantity}";

        infosUIRefs.OperationUI.onClick.AddListener(delegate { PlantSeed(item); });
    }

    private void Clear()
    {
        for (int j = 0; j < seedsParent.childCount; j++)
        {
            Destroy(seedsParent.GetChild(j).gameObject);
        }
    }

    #endregion

    public void PlantSeed(Item item)
    {
        if (currentCropPlot == null) return;

        if (currentCropPlot != null && currentCropPlot.SeedSource != null) return;

        playerController.PlayerInventory.RemoveItemQuantity(item, 1);

        GameObject plant = Instantiate(item.itemObject, currentCropPlot.transform);

        plant.GetComponent<PlantGrowth>().CropPlot = currentCropPlot;

        currentCropPlot.SeedSource = plant;

        CloseCultureUI();

        if (questCompletionPlant != null)
            questCompletionPlant.CompleteObjective();
    }

    private void PlantSpectificPlantAtCropPlot(Plant plantSO, CropPlot currentCropPlot, string growth, string state, float timeSkip)
    {
        if (plantSO == null || currentCropPlot == null) return;

        GameObject plant = Instantiate(plantSO.source, currentCropPlot.transform);

        plant.GetComponent<PlantGrowth>().CropPlot = currentCropPlot;

        currentCropPlot.SeedSource = plant;

        plant.GetComponent<PlantGrowth>().SetGrowthState(growth, state, plantSO, timeSkip);
    }
}