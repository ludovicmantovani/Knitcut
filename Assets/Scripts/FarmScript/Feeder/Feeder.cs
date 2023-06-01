using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;

public class Feeder : MonoBehaviour
{
    public PlayerInput playerInput;

    [Header("Feeder Inventory")]
    [SerializeField] private bool canUseFeeder;
    [SerializeField] private bool feederInUse;
    [SerializeField] private GameObject itemUI;
    [SerializeField] private GameObject feederInventory;
    [SerializeField] private GameObject feederInventoryContent;
    [SerializeField] private GameObject interactionPanel;
    [SerializeField] private GameObject feederModel;
    [SerializeField] private ItemsInFeeder[] items = new ItemsInFeeder[3];

    [Serializable]
    public class ItemsInFeeder
    {
        public Item item;
        public int quantity;
    }

    private string interaction;

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

    public ItemsInFeeder[] Items
    {
        get { return items; }
        set { items = value; }
    }

    private void Start()
    {
        playerInput = FindObjectOfType<PlayerInput>();
        canUseFeeder = false;
        feederInUse = false;
        feederInventory.SetActive(false);

        interaction = "Utiliser " + playerInput.InteractionAction.GetBindingDisplayString();
    }

    private void Update()
    {
        // GameObject Visual
        //HandleVisual();

        // Inventory
        //HandleFeederUse();
        //HandleFeederInventory();

        HandleFeederInventory();
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
    }

    private void OpenFeederInventory()
    {
        feederInUse = true;

        feederInventory.SetActive(true);

        interactionPanel.GetComponentInChildren<Text>().text = $"{interaction} pour fermer la mangeoire";

        MinigameManager.AddOpenInventory(feederInventory);

        ShowItemsInFeeder();
    }

    private void CloseFeederInventory()
    {
        feederInUse = false;

        feederInventory.SetActive(false);

        interactionPanel.GetComponentInChildren<Text>().text = $"{interaction} pour ouvrir la mangeoire";

        MinigameManager.RemoveOpenInventory(feederInventory);

        GetAllItemsInFeederSlots();

        Clear();
    }

    private void ShowItemsInFeeder()
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i].item != null)
            {
                Transform slot = feederInventoryContent.transform.GetChild(i);

                DraggableItem itemSlotUI = Instantiate(itemUI, slot).GetComponent<DraggableItem>();

                itemSlotUI.Item = items[i].item;
                itemSlotUI.QuantityStacked = items[i].quantity;
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
                DraggableItem draggableItem = slot.GetChild(0).GetComponent<DraggableItem>();

                if (draggableItem.Item.itemType != ItemType.Consumable) return;

                items[i].item = draggableItem.Item;
                items[i].quantity = draggableItem.QuantityStacked;
            }
        }
    }

    public void TransferItems(Feeder oldFeeder)
    {
        for (int i = 0; i < items.Length; i++)
        {
            items[i].item = oldFeeder.Items[i].item;
            items[i].quantity = oldFeeder.Items[i].quantity;
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
            if (items[i].item != null && items[i].item.itemType == ItemType.Consumable && items[i].quantity > 0)
            {
                return items[i].item;
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
                DraggableItem draggableItem = slot.GetChild(0).GetComponent<DraggableItem>();

                if (draggableItem.Item == item)
                {
                    draggableItem.QuantityStacked -= 1;

                    if (draggableItem.QuantityStacked <= 0)
                    {
                        Destroy(draggableItem.gameObject);
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