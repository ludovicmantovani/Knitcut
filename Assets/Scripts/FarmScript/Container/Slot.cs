using UnityEngine;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right) return;

        GameObject objectToDrop = eventData.pointerDrag;

        if (objectToDrop == null) return;

        ItemHandler itemHandlerToDrop = objectToDrop.GetComponent<ItemHandler>();

        if (itemHandlerToDrop == null) return;

        // If there is already an item
        #region 1 item already in slot

        if (transform.childCount > 0)
        {
            ItemHandler itemHandlerInSlot = transform.GetComponentInChildren<ItemHandler>();
            Item itemInSlot = itemHandlerInSlot.Item;

            if (itemHandlerInSlot == null || itemInSlot == null) return;

            // If 2 items are same
            #region 2 items are same

            if (itemInSlot == itemHandlerToDrop.Item && itemHandlerInSlot.UniqueValue == itemHandlerToDrop.UniqueValue)
            {
                // If item is stackable && stack size limit is not reached
                #region item in slot is stackable & quantity stacked is less than stack limit
                if (itemInSlot.isStackable && itemHandlerInSlot.QuantityStacked < itemInSlot.maxStackSize)
                {
                    int quantityInDropped = itemHandlerToDrop.QuantityStacked;
                    int quantityInDraggable = itemHandlerInSlot.QuantityStacked;

                    int totalQuantity = quantityInDropped + quantityInDraggable;

                    // If total quantity is less or equal than stack size limit
                    if (totalQuantity <= itemInSlot.maxStackSize)
                    {
                        itemHandlerInSlot.QuantityStacked = totalQuantity;

                        itemHandlerToDrop.DropItemToSlot(null, true);
                    }

                    // If total quantity is more than stack size limit
                    if (totalQuantity > itemInSlot.maxStackSize)
                    {
                        itemHandlerInSlot.QuantityStacked = itemInSlot.maxStackSize;
                        itemHandlerToDrop.QuantityStacked = totalQuantity - itemInSlot.maxStackSize;

                        itemHandlerToDrop.DropItemToSlot(null);
                    }
                }
                #endregion
            }

            #endregion
            // Else they are differents
            else
            {
                itemHandlerToDrop.ExchangeItems(itemHandlerInSlot);
            }
        }
        #endregion

        // Else it's free
        else
        {
            if (itemHandlerToDrop == null) return;

            itemHandlerToDrop.DropItemToSlot(transform);
        }
    }
}