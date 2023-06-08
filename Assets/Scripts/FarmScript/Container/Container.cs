using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;


public class Container : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject interactionPanel;
    [SerializeField] private GameObject containerInventoryPanel;
    [SerializeField] private GameObject containerInventoryContent;
    [SerializeField] private GameObject playerInventoryPanel;
    [SerializeField] private GameObject itemUI;

    [Header("Container states")]
    [SerializeField] private bool canUseContainer;
    [SerializeField] private bool containerInUse;

    private PlayerInput playerInput;
    private PlayerController playerController;

    private string interaction;

    public bool CanUseContainer
    {
        get { return canUseContainer; }
        set { canUseContainer = value; }
    }

    public GameObject ContainerInventory
    {
        get { return containerInventoryPanel; }
        set { containerInventoryPanel = value; }
    }

    public GameObject PlayerInventory
    {
        get { return playerInventoryPanel; }
        set { playerInventoryPanel = value; }
    }

    private void Start()
    {
        playerInput = FindObjectOfType<PlayerInput>();
        playerController = FindObjectOfType<PlayerController>();

        canUseContainer = false;
        containerInUse = false;

        interaction = "Utiliser " + playerInput.InteractionAction.GetBindingDisplayString();
    }

    private void Update()
    {
        HandleContainerUse();

        HandleContainerInventory();
    }

    #region Container Use

    private void HandleContainerUse()
    {
        if (canUseContainer)
        {
            interactionPanel.SetActive(true);
        }
        else
        {
            interactionPanel.SetActive(false);
        }
    }

    #endregion

    #region Container Inventory

    private void HandleContainerInventory()
    {
        containerInventoryPanel.SetActive(containerInUse);

        if (playerInput.InteractionAction.triggered && canUseContainer)
        {
            if (!containerInUse)
            {
                OpenContainerInventory();
            }
            else
            {
                CloseContainerInventory();
            }
        }

        if (containerInUse && playerInput.CancelAction.triggered)
            CloseContainerInventory();
    }

    private void OpenContainerInventory()
    {
        containerInUse = true;

        interactionPanel.GetComponentInChildren<Text>().text = $"{interaction} pour fermer la réserve";

        MinigameManager.AddOpenInventory(containerInventoryContent);

        playerController.PlayerInventory.OpenInventory();
    }

    private void CloseContainerInventory()
    {
        containerInUse = false;

        interactionPanel.GetComponentInChildren<Text>().text = $"{interaction} pour ouvrir la réserve";

        MinigameManager.RemoveOpenInventory(containerInventoryContent);

        playerController.PlayerInventory.CloseInventory();
    }

    #endregion

    #region Items

    public void AddItemToInventory(Item item, GameObject inventory)
    {
        Transform slotParent = GetFreeSlot(inventory);

        if (slotParent == null) return;

        GameObject itemObject = Instantiate(itemUI, slotParent);

        itemObject.GetComponent<ItemHandler>().Item = item;

        itemObject.GetComponent<Image>().sprite = item.itemSprite;
    }

    private Transform GetFreeSlot(GameObject inventory)
    {
        for (int i = 0; i < inventory.transform.childCount; i++)
        {
            Transform slotObject = inventory.transform.GetChild(i);

            if (slotObject.childCount == 0)
            {
                return slotObject;
            }
        }

        return null;
    }

    #endregion
}