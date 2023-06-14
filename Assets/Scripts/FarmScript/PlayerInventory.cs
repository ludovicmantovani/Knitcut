using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private GameObject itemUI;

    private bool inUse = false;
    private PlayerController player;

    private void Start()
    {
        player = FindObjectOfType<PlayerController>();

        gameObject.SetActive(false);
    }

    private void Update()
    {
        HandleInventoryUI();
    }

    #region Inventory UI

    public void HandleInventoryUI()
    {
        if (player.PlayerInput.InventoryAction.triggered)
        {
            if (!inUse) OpenInventory();
            else CloseInventory();
        }

        if (player.PlayerInput.CancelAction.triggered && inUse)
            CloseInventory();
    }

    public void OpenInventory()
    {
        inUse = true;

        gameObject.SetActive(true);

        MinigameManager.AddOpenInventory(gameObject);
    }

    public void CloseInventory()
    {
        inUse = false;

        gameObject.SetActive(false);

        MinigameManager.RemoveOpenInventory(gameObject);
    }

    #endregion

    #region Items

    private GameObject CreateItemUI(bool setup = false, Item item = null, int quantity = -1, float uniqueValue = -1)
    {
        GameObject itemObject = Instantiate(itemUI, GetFreeSlot());

        if (setup)
        {
            itemObject.GetComponent<ItemHandler>().Item = item;

            itemObject.GetComponent<Image>().sprite = item.itemSprite;

            if (quantity != -1)
                itemObject.GetComponent<ItemHandler>().QuantityStacked = quantity;

            if (uniqueValue != -1)
                itemObject.GetComponent<ItemHandler>().UniqueValue = uniqueValue;
        }

        return itemObject;
    }

    public void AddItemToInventory(Item item, int quantity = 1, float uniqueValue = 0f)
    {
        List<ItemHandler> sameItems = SearchSameItemInInventory(item);
        List<ItemHandler> sameItemsNotFull = new List<ItemHandler>();

        if (sameItems.Count > 0)
        {
            for (int i = 0; i < sameItems.Count; i++)
            {
                ItemHandler sameItem = sameItems[i];

                if (sameItem.QuantityStacked < sameItem.Item.maxStackSize) sameItemsNotFull.Add(sameItem);
            }

            int remainingQuantity = quantity;

            for (int i = 0; i < sameItemsNotFull.Count; i++)
            {
                if (remainingQuantity == 0) return;

                ItemHandler sameItemNotFull = sameItemsNotFull[i];

                if (sameItemNotFull.QuantityStacked + quantity <= sameItemNotFull.Item.maxStackSize)
                {
                    sameItemNotFull.QuantityStacked += quantity;

                    remainingQuantity = 0;
                }
                else if (sameItemNotFull.QuantityStacked + quantity > sameItemNotFull.Item.maxStackSize)
                {
                    remainingQuantity = (sameItemNotFull.QuantityStacked + quantity) - sameItemNotFull.Item.maxStackSize;

                    sameItemNotFull.QuantityStacked = sameItemNotFull.Item.maxStackSize;
                    sameItemsNotFull.Remove(sameItemNotFull);
                }
            }

            if (sameItemsNotFull.Count == 0 && remainingQuantity > 0) CreateItemUI(true, item, remainingQuantity, uniqueValue);
        }
        else
        {
            CreateItemUI(true, item, quantity, uniqueValue);
        }
    }

    public List<ItemHandler> SearchSameItemInInventory(Item item, float uniqueValue = 0f)
    {
        List<ItemHandler> itemHandlersInInventory = new List<ItemHandler>();
        
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).GetComponentInChildren<ItemHandler>())
            {
                ItemHandler itemHandler = transform.GetChild(i).GetComponentInChildren<ItemHandler>();

                if (itemHandler.Item == item)
                {
                    if ((itemHandler.Item.itemType == ItemType.Dish && itemHandler.UniqueValue == uniqueValue)
                        || itemHandler.Item.itemType != ItemType.Dish)
                    {
                        itemHandlersInInventory.Add(itemHandler);
                    }
                }
            }
        }

        return itemHandlersInInventory;
    }

    public bool RemoveItemQuantity(Item item, int quantityToRemove, float uniqueValue = 0)
    {
        // Get all same item from inventory
        List<ItemHandler> itemHandlers = SearchSameItemInInventory(item, uniqueValue);

        Debug.Log($"RemoveItemQuantity {itemHandlers.Count}");

        if (itemHandlers.Count == 0) return false;

        itemHandlers = itemHandlers.OrderBy(item => item.QuantityStacked).ToList();

        // Get total quantity possessed
        int quantityPossessed = 0;

        for (int i = 0; i < itemHandlers.Count; i++)
        {
            quantityPossessed += itemHandlers[i].QuantityStacked;
        }

        Debug.Log($"RemoveItemQuantity quantityPossessed {quantityPossessed}");

        // Remove total quantityToRemove
        if (quantityToRemove > quantityPossessed) return false;

        for (int i = 0; i < itemHandlers.Count; i++)
        {
            if (itemHandlers[i].QuantityStacked > quantityToRemove)
            {
                itemHandlers[i].QuantityStacked -= quantityToRemove;

                Debug.Log($"RemoveItemQuantity remove {quantityToRemove} to {itemHandlers[i].QuantityStacked}");

                return true;
            }
            else
            {
                quantityToRemove -= itemHandlers[i].QuantityStacked;

                Destroy(itemHandlers[i].gameObject);
            }
        }

        return true;
    }

    private Transform GetFreeSlot()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform slotObject = transform.GetChild(i);

            if (slotObject.childCount == 0)
            {
                return slotObject;
            }
        }

        return null;
    }

    public bool InventoryIsFull()
    {
        if (GetFreeSlot() == null) return true;
        else return false;
    }

    public List<ItemHandler> SearchItemsPossessed()
    {
        List<ItemHandler> itemsFounded = new List<ItemHandler>();

        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).childCount > 0)
            {
                ItemHandler itemHandler = transform.GetChild(i).GetComponentInChildren<ItemHandler>();

                itemsFounded.Add(itemHandler);
            }
        }

        return itemsFounded;
    }

    public int GetItemQuantity(Item item, float uniqueValue = 0f)
    {
        int quantity = 0;

        List<ItemHandler> itemHandlers = SearchSameItemInInventory(item, uniqueValue);

        if (itemHandlers.Count == 0) return -1;

        for (int i = 0; i < itemHandlers.Count; i++)
        {
            quantity += itemHandlers[i].QuantityStacked;
        }
        
        return quantity;
    }

    #endregion
}