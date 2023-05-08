using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private GameObject itemUI;

    private bool inventoryOpen = true;
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
        if (player.PlayerInput.InventoryAction.triggered && inventoryOpen)
        {
            inventoryOpen = false;
            gameObject.SetActive(true);
            MinigameManager.AddOpenInventory(gameObject);
        }
        else if (player.PlayerInput.InventoryAction.triggered && !inventoryOpen)
        {
            inventoryOpen = true;
            gameObject.SetActive(false);
            MinigameManager.RemoveOpenInventory(gameObject);
        }

    }

    #endregion

    #region Items

    private GameObject CreateItemUI(bool setup = false, Item item = null, int quantity = -1)
    {
        GameObject itemObject = Instantiate(itemUI, GetFreeSlot());

        if (setup)
        {
            itemObject.GetComponent<DraggableItem>().Item = item;

            itemObject.GetComponent<Image>().sprite = item.itemSprite;

            if (quantity != -1)
                itemObject.GetComponent<DraggableItem>().QuantityStacked = quantity;
        }

        return itemObject;
    }

    public void AddItemToInventory(Item item, int quantity = 1)
    {
        List<DraggableItem> sameItems = SearchSameItemInInventory(item);
        List<DraggableItem> sameItemsNotFull = new List<DraggableItem>();

        if (sameItems.Count > 0)
        {
            for (int i = 0; i < sameItems.Count; i++)
            {
                DraggableItem sameItem = sameItems[i];

                if (sameItem.QuantityStacked < sameItem.Item.maxStackSize) sameItemsNotFull.Add(sameItem);
            }

            int remainingQuantity = quantity;

            for (int i = 0; i < sameItemsNotFull.Count; i++)
            {
                if (remainingQuantity == 0) return;

                DraggableItem sameItemNotFull = sameItemsNotFull[i];

                if (sameItemNotFull.QuantityStacked + quantity <= sameItemNotFull.Item.maxStackSize)
                {
                    sameItemNotFull.QuantityStacked += quantity;

                    remainingQuantity = 0;
                }

                if (sameItemNotFull.QuantityStacked + quantity > sameItemNotFull.Item.maxStackSize)
                {
                    remainingQuantity = (sameItemNotFull.QuantityStacked + quantity) - sameItemNotFull.Item.maxStackSize;

                    sameItemNotFull.QuantityStacked = sameItemNotFull.Item.maxStackSize;
                    sameItemsNotFull.Remove(sameItemNotFull);
                }
            }

            if (sameItemsNotFull.Count == 0 && remainingQuantity > 0) CreateItemUI(true, item, remainingQuantity);
        }
        else
        {
            CreateItemUI(true, item, quantity);
        }
    }

    public List<DraggableItem> SearchSameItemInInventory(Item item)
    {
        List<DraggableItem> draggableItemsInInventory = new List<DraggableItem>();
        
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).GetComponentInChildren<DraggableItem>())
            {
                DraggableItem draggableItem = transform.GetChild(i).GetComponentInChildren<DraggableItem>();

                if (draggableItem.Item == item)
                {
                    draggableItemsInInventory.Add(draggableItem);
                }
            }
        }

        return draggableItemsInInventory;
    }

    public void RemoveItemQuantity(Item item, int quantityToRemove)
    {
        // Get all same item from inventory
        List<DraggableItem> draggableItems = SearchSameItemInInventory(item);

        if (draggableItems.Count == 0) return;

        draggableItems = draggableItems.OrderBy(item => item.QuantityStacked).ToList();

        // Get total quantity possessed
        int quantityPossessed = 0;

        for (int i = 0; i < draggableItems.Count; i++)
        {
            quantityPossessed += draggableItems[i].QuantityStacked;
        }

        // Remove total quantityToRemove
        if (quantityToRemove > quantityPossessed) return;

        for (int i = 0; i < draggableItems.Count; i++)
        {
            if (draggableItems[i].QuantityStacked > quantityToRemove)
            {
                draggableItems[i].QuantityStacked -= quantityToRemove;

                return;
            }
            else
            {
                quantityToRemove -= draggableItems[i].QuantityStacked;

                Destroy(draggableItems[i].gameObject);
            }
        }
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

    public List<DraggableItem> SearchItemsPossessed()
    {
        List<DraggableItem> itemsFounded = new List<DraggableItem>();

        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).childCount > 0)
            {
                DraggableItem draggableItem = transform.GetChild(i).GetComponentInChildren<DraggableItem>();

                itemsFounded.Add(draggableItem);
            }
        }

        return itemsFounded;
    }

    public int GetItemQuantity(Item item)
    {
        int quantity = 0;

        List<DraggableItem> draggableItems = SearchSameItemInInventory(item);

        if (draggableItems.Count == 0) return -1;

        for (int i = 0; i < draggableItems.Count; i++)
        {
            quantity += draggableItems[i].QuantityStacked;
        }
        
        return quantity;
    }

    #endregion
}