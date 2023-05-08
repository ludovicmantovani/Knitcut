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
    }

    private void Update()
    {
        HandlePlantSeedUI();
        HandleCultureInventory();
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

        if (!canPlantSeed)
        {
            CloseCultureUI();
            return;
        }

        HandleCropPlotState();
    }

    private void HandleCropPlotState()
    {
        if (!cultureUIInUse)
        {
            if (!currentCropPlot.IsCultivating)
            {
                instruction = $"Utiliser {playerInput.InteractionAction.GetBindingDisplayString()} pour planter une graine";
                interactionUI.GetComponentInChildren<Text>().text = instruction;

                if (playerInput.InteractionAction.triggered)
                {
                    OpenCultureUI();

                    HandleSeedsUI();
                }
            }
            else if (currentCropPlot.IsCultivating && currentCropPlot.Product == null)
            {
                if (currentCropPlot.SeedSource == null) return;

                SeedGrowth.ProductState seedProductState = currentCropPlot.SeedSource.GetComponent<SeedGrowth>().GetProductState;

                if (seedProductState == SeedGrowth.ProductState.Sick)
                    instruction = $"Utiliser {playerInput.HealAction.GetBindingDisplayString()} pour soigner la plante";
                else if (seedProductState == SeedGrowth.ProductState.Dehydrated)
                    instruction = $"Utiliser {playerInput.HydrateAction.GetBindingDisplayString()} pour hydrater la plante";
                else
                    instruction = $"Croissance en cours...";
                
                interactionUI.GetComponentInChildren<Text>().text = instruction;
            }
            else if (currentCropPlot.IsCultivating && currentCropPlot.Product != null)
            {
                instruction = $"Croissance terminée\nUtiliser {playerInput.InteractionAction.GetBindingDisplayString()} pour ramasser le fruit";
                interactionUI.GetComponentInChildren<Text>().text = instruction;

                if (playerInput.InteractionAction.triggered)
                {
                    Item item = currentCropPlot.Product.GetComponent<KeepItem>().Item;
                    if (questCompletionPick != null)
                        questCompletionPick.CompleteObjective();

                    playerController.PlayerInventory.AddItemToInventory(item);

                    Destroy(currentCropPlot.SeedSource);

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
            DraggableItem itemPossessed = playerController.PlayerInventory.SearchItemsPossessed()[i];

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

        GameObject seed = Instantiate(item.itemObject, currentCropPlot.transform);

        seed.GetComponent<SeedGrowth>().CropPlot = currentCropPlot;

        currentCropPlot.SeedSource = seed;

        CloseCultureUI();

        if (questCompletionPlant != null)
            questCompletionPlant.CompleteObjective();
    }
}