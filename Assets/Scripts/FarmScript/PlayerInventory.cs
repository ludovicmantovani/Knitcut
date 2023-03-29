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

    #region Inventory

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

    public GameObject CreateItemUI()
    {
        GameObject itemObject = Instantiate(itemUI, GetFreeSlot());

        return itemObject;
    }

    public void AddItemToInventory(Item item)
    {
        Transform slotParent = GetFreeSlot();

        if (slotParent == null) return;

        GameObject itemObject = Instantiate(itemUI, slotParent);

        itemObject.GetComponent<DraggableItem>().Item = item;

        itemObject.GetComponent<Image>().sprite = item.itemSprite;
    }

    private List<DraggableItem> SearchSameItemInInventory(Item item)
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

        /*for (int i = 0; i < SearchItemsPossessed().Count; i++)
        {
            if (item == SearchItemsPossessed()[i].Item)
            {
                quantity += SearchItemsPossessed()[i].QuantityStacked;
            }
        }*/
        
        return quantity;
    }

    #endregion
}