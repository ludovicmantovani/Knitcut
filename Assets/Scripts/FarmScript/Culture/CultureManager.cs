using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static ShopManager;

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

    [Header("Datas")]
    [SerializeField] private bool canPlantSeed;
    [SerializeField] private bool cultureUIInUse;

    private string instruction;
    private bool resetting;

    public bool CanPlantSeed
    {
        get { return canPlantSeed; }
        set { canPlantSeed = value; }
    }

    private void Start()
    {
        playerInput = FindObjectOfType<PlayerInput>();
        playerController = FindObjectOfType<PlayerController>();

        resetting = false;

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

        if (playerInput.InteractionAction.triggered && canPlantSeed)
        {
            if (!cultureUIInUse)
            {
                if (!currentCropPlot.IsCultivating)
                {
                    instruction = $"Utiliser {playerInput.InteractionAction.GetBindingDisplayString()} pour planter une graine";
                    interactionUI.GetComponentInChildren<Text>().text = instruction;

                    OpenCultureUI();

                    HandleSeedsUI();
                }
                else if (currentCropPlot.IsCultivating && currentCropPlot.Product == null)
                {
                    instruction = $"Une graine est actuellement en production sur cette parcelle";
                    interactionUI.GetComponentInChildren<Text>().text = instruction;
                }
                else if (currentCropPlot.IsCultivating && currentCropPlot.Product != null)
                {
                    instruction = $"Une graine est prête à être ramassé";
                    interactionUI.GetComponentInChildren<Text>().text = instruction;
                }
            }
            else
            {
                CloseCultureUI();
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
            currentCropPlot = cropPlot;
        else
            currentCropPlot = null;
    }

    private void HandleSeedsUI()
    {
        Clear();

        for (int i = 0; i < playerController.PlayerInventory.SearchItemsPossessed().Count; i++)
        {
            DraggableItem itemPossessed = playerController.PlayerInventory.SearchItemsPossessed()[i];

            ShowObjectUI(itemPossessed);
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

        if (currentCropPlot != null && currentCropPlot.ItemInCropPlot != null) return;

        currentCropPlot.ItemInCropPlot = itemPossessed.Item;

        itemPossessed.QuantityStacked -= 1;

        if (itemPossessed.QuantityStacked <= 0)
            Destroy(itemPossessed.gameObject);

        GameObject seed = Instantiate(itemPossessed.Item.itemObject, currentCropPlot.transform);

        seed.GetComponent<SeedGrowth>().CropPlot = currentCropPlot;

        CloseCultureUI();

        Debug.Log($"Plant {seed.name}");
    }
}