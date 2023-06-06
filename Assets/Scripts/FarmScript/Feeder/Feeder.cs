using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;

public class Feeder : MonoBehaviour
{
    private PlayerInput playerInput;
    private PlayerController playerController;

    [Header("Feeder Inventory")]
    [SerializeField] private bool canUseFeeder;
    [SerializeField] private bool feederInUse;
    [SerializeField] private GameObject itemUI;
    [SerializeField] private GameObject feederInventory;
    [SerializeField] private GameObject feederInventoryContent;
    [SerializeField] private GameObject interactionPanel;
    [SerializeField] private GameObject feederModel;
    [SerializeField] private Item[] items = new Item[3];
    [SerializeField] private int[] itemsQuantities = new int[3];

    private string interaction;

    #region Getters / Setters

    public bool CanUseFeeder
    {
        get { return canUseFeeder; }
        set { canUseFeeder = value; }
    }

    public GameObject InteractionPanel
    {
        get { return interactionPanel; }
        set { interactionPanel = value; }
    }

    public Item[] Items
    {
        get { return items; }
        set { items = value; }
    }

    public int[] ItemsQuantities
    {
        get { return itemsQuantities; }
        set { itemsQuantities = value; }
    }

    #endregion

    private void Start()
    {
        playerInput = FindObjectOfType<PlayerInput>();
        playerController = FindObjectOfType<PlayerController>();

        canUseFeeder = false;
        feederInUse = false;

        feederInventory.SetActive(false);

        interaction = "Utiliser " + playerInput.InteractionAction.GetBindingDisplayString();
    }

    private void Update()
    {
        // GameObject Visual
        //HandleVisual();

        HandleFeederInventory();

        if (feederInUse) GetAllItemsInFeederSlots();
    }

    private void HandleVisual()
    {
        Vector3 positionVisual = feederModel.transform.GetChild(0).localPosition;

        if (IsFeederEmpty())
        {
            positionVisual.y = -0.001f;
        }
        else
        {
            positionVisual.y = 0.6f;
        }

        feederModel.transform.GetChild(0).localPosition = positionVisual;
    }

    public Item GetFood()
    {
        if (IsFeederEmpty()) return null;

        return GetItem();
    }

    #region Feeder Inventory

    #region Inventory UI

    public void UpdateInteraction(bool state)
    {
        canUseFeeder = state;

        if (canUseFeeder)
            interactionPanel.GetComponentInChildren<Text>().text = $"{interaction} pour ouvrir la mangeoire";

        interactionPanel.SetActive(state);
    }

    private void HandleFeederInventory()
    {
        if (playerInput.InteractionAction.triggered && canUseFeeder)
        {
            if (!feederInUse)
            {
                OpenFeederInventory();
            }
            else
            {
                CloseFeederInventory();
            }
        }

        if (feederInUse && playerInput.CancelAction.triggered)
            CloseFeederInventory();
    }

    private void OpenFeederInventory()
    {
        feederInUse = true;

        feederInventory.SetActive(true);

        interactionPanel.GetComponentInChildren<Text>().text = $"{interaction} pour fermer la mangeoire";

        MinigameManager.AddOpenInventory(feederInventory);

        ShowItemsInFeeder();

        playerController.PlayerInventory.OpenInventory();
    }

    private void CloseFeederInventory()
    {
        feederInUse = false;

        feederInventory.SetActive(false);

        interactionPanel.GetComponentInChildren<Text>().text = $"{interaction} pour ouvrir la mangeoire";

        MinigameManager.RemoveOpenInventory(feederInventory);

        Clear();

        playerController.PlayerInventory.CloseInventory();
    }

    private void ShowItemsInFeeder()
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] != null)
            {
                Transform slot = feederInventoryContent.transform.GetChild(i);

                ItemHandler itemSlotUI = Instantiate(itemUI, slot).GetComponent<ItemHandler>();

                itemSlotUI.Item = items[i];
                itemSlotUI.QuantityStacked = itemsQuantities[i];
            }
        }
    }

    private void GetAllItemsInFeederSlots()
    {
        for (int i = 0; i < feederInventoryContent.transform.childCount; i++)
        {
            Transform slot = feederInventoryContent.transform.GetChild(i);

            if (slot.childCount > 0)
            {
                ItemHandler itemHandler = slot.GetChild(0).GetComponent<ItemHandler>();

                if (itemHandler.Item.itemType != ItemType.Consumable) return;

                items[i] = itemHandler.Item;
                itemsQuantities[i] = itemHandler.QuantityStacked;
            }
            else if (slot.childCount == 0 && items[i] != null)
            {
                items[i] = null;
                itemsQuantities[i] = 0;
            }
        }
    }

    public void TransferItems(Feeder oldFeeder)
    {
        for (int i = 0; i < items.Length; i++)
        {
            items[i] = oldFeeder.Items[i];
            itemsQuantities[i] = oldFeeder.ItemsQuantities[i];
        }
    }

    private void Clear()
    {
        for (int i = 0; i < feederInventoryContent.transform.childCount; i++)
        {
            Transform slot = feederInventoryContent.transform.GetChild(i);

            if (slot.childCount > 0)
            {
                Destroy(slot.GetChild(0).gameObject);
            }
        }
    }
    private Item GetItem()
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] != null && items[i].itemType == ItemType.Consumable && itemsQuantities[i] > 0)
            {
                return items[i];
            }
        }

        return null;
    }

    public void RemoveItem(Item item)
    {
        for (int i = 0; i < feederInventoryContent.transform.childCount; i++)
        {
            Transform slot = feederInventoryContent.transform.GetChild(i);

            // If item present in slot
            if (slot.childCount > 0)
            {
                ItemHandler itemHandler = slot.GetChild(0).GetComponent<ItemHandler>();

                if (itemHandler.Item == item)
                {
                    itemHandler.QuantityStacked -= 1;

                    if (itemHandler.QuantityStacked <= 0)
                    {
                        Destroy(itemHandler.gameObject);
                    }
                }
            }
        }
    }

    private bool IsFeederEmpty()
    {
        bool isEmpty = true;

        for (int i = 0; i < feederInventoryContent.transform.childCount; i++)
        {
            Transform slot = feederInventoryContent.transform.GetChild(i);

            if (slot.childCount > 0)
            {
                isEmpty = false;
            }
        }

        return isEmpty;
    }

    #endregion

    #endregion
}