using Gameplay.Quests;
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

    private PlayerInput playerInput;
    private PlayerController playerController;

    [Header("Datas")]
    [SerializeField] private bool canPlantSeed;
    [SerializeField] private bool cultureUIInUse;

    [Header("Quest")]
    [SerializeField] private QuestCompletion questCompletionPlant;
    [SerializeField] private QuestCompletion questCompletionPick;

    private string instruction;

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
                SeedGrowth.ProductState seedProductState = currentCropPlot.GetSeedProductState();

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
                instruction = $"Croissance termin�e\nUtiliser {playerInput.InteractionAction.GetBindingDisplayString()} pour ramasser le fruit";
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

    private void CloseCultureUI()
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

        for (int i = 0; i < playerController.PlayerInventory.SearchItemsPossessed().Count; i++)
        {
            DraggableItem itemPossessed = playerController.PlayerInventory.SearchItemsPossessed()[i];

            if (itemPossessed.Item.itemType == ItemType.Seed) ShowObjectUI(itemPossessed);
        }
    }

    private void ShowObjectUI(DraggableItem itemPossessed)
    {
        InfosUIRefs infosUIRefs = Instantiate(seedToPlantUI, seedsParent).GetComponent<InfosUIRefs>();

        Sprite objectSprite = itemPossessed.Item.itemSprite;
        string objectName = itemPossessed.Item.itemName;
        float objectQuantity = itemPossessed.QuantityStacked;

        infosUIRefs.ImageUI.sprite = itemPossessed.Item.itemSprite; ;
        infosUIRefs.NameUI.text = itemPossessed.Item.itemName;
        infosUIRefs.PriceUI.text = $"{itemPossessed.QuantityStacked} P";

        infosUIRefs.OperationUI.onClick.AddListener(delegate { PlantSeed(itemPossessed); });
    }
    private void Clear()
    {
        for (int j = 0; j < seedsParent.childCount; j++)
        {
            Destroy(seedsParent.GetChild(j).gameObject);
        }
    }

    #endregion

    public void PlantSeed(DraggableItem itemPossessed)
    {
        if (currentCropPlot == null) return;

        if (currentCropPlot != null && currentCropPlot.SeedSource != null) return;

        itemPossessed.QuantityStacked -= 1;

        if (itemPossessed.QuantityStacked <= 0)
            Destroy(itemPossessed.gameObject);

        GameObject seed = Instantiate(itemPossessed.Item.itemObject, currentCropPlot.transform);

        seed.GetComponent<SeedGrowth>().CropPlot = currentCropPlot;

        currentCropPlot.SeedSource = seed;

        CloseCultureUI();

        Debug.Log($"Plant {seed.name}");
        if (questCompletionPlant != null)
            questCompletionPlant.CompleteObjective();
    }
}