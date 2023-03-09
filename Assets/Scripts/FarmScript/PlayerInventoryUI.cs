using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject itemUI;

    private bool inventoryOpen = true;
    private playerController player;

    private void Start()
    {
        player = FindObjectOfType<playerController>();

        gameObject.SetActive(false);
    }

    private void Update()
    {
        HandleInventoryUI();
    }

    #region Inventory

    public void HandleInventoryUI()
    {
        if (player.pI.actions["Inventory"].triggered && inventoryOpen)
        {
            inventoryOpen = false;
            gameObject.SetActive(true);
            MinigameManager.AddOpenInventory(gameObject);
        }
        else if (player.pI.actions["Inventory"].triggered && !inventoryOpen)
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

    public void RemoveQuantityItem(Item item, int quantity)
    {
        List<DraggableItem> dragItemSlot = GetItemSlot(item);

        if (dragItemSlot == null || dragItemSlot.Count == 0) return;

        for (int i = 0; i < dragItemSlot.Count; i++)
        {
            if (dragItemSlot[i].quantityStacked > quantity)
            {
                dragItemSlot[i].quantityStacked -= quantity;
            }
            else
            {
                Destroy(dragItemSlot[i].gameObject);

                quantity -= dragItemSlot[i].quantityStacked;
            }
        }
    }

    private List<DraggableItem> GetItemSlot(Item item)
    {
        List<DraggableItem> dragItems = new List<DraggableItem>();

        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.childCount > 0)
            {
                if (transform.GetChild(i).GetComponentInChildren<DraggableItem>())
                {
                    DraggableItem dragItemInSlot = transform.GetChild(i).GetComponentInChildren<DraggableItem>();

                    if (dragItemInSlot == null) return null;

                    if (dragItemInSlot.Item == item)
                    {
                        dragItems.Add(dragItemInSlot);
                    }
                }                
            }
        }

        return dragItems;
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

    #endregion
}