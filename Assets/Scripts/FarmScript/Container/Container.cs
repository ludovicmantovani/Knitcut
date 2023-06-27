using TMPro;
using UnityEngine.UI;
using UnityEngine;
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
        interactionPanel.GetComponentInChildren<TMP_Text>().text = $"{interaction} pour ouvrir le coffre";
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
    }

    private void OpenContainerInventory()
    {
        containerInUse = true;

        interactionPanel.GetComponentInChildren<TMP_Text>().text = $"{interaction} pour fermer le coffre";

        GameManager.AddOpenInventory(this, containerInventoryContent);

        playerController.PlayerInventory.OpenInventory();
    }

    public void CloseContainerInventory()
    {
        containerInUse = false;

        interactionPanel.GetComponentInChildren<TMP_Text>().text = $"{interaction} pour ouvrir le coffre";

        GameManager.RemoveOpenInventory(this, containerInventoryContent);

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