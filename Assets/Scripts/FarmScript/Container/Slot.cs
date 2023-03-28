using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IDropHandler, IPointerClickHandler, IPointerEnterHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        GameObject objectToDrop = eventData.pointerDrag;
        DraggableItem draggableItemToDrop = objectToDrop.GetComponent<DraggableItem>();

        // If there is already an item
        #region 1 item already in slot

        if (transform.childCount > 0)
        {
            DraggableItem draggableItemInSlot = transform.GetChild(0).GetComponent<DraggableItem>();
            Item itemInSlot = draggableItemInSlot.Item;

            // If 2 items are same
            #region 2 items are same

            if (itemInSlot.Equals(draggableItemToDrop.Item))
            {
                // If item is stackable && stack size limit is not reached
                #region item in slot is stackable & quantity stacked is less than stack limit

                if (itemInSlot.isStackable && draggableItemInSlot.QuantityStacked < itemInSlot.maxStackSize)
                {
                    int quantityInDropped = draggableItemToDrop.QuantityStacked;
                    int quantityInDraggable = draggableItemInSlot.QuantityStacked;

                    int totalQuantity = quantityInDropped + quantityInDraggable;

                    // If total quantity is less or equal than stack size limit
                    if (totalQuantity <= itemInSlot.maxStackSize)
                    {
                        draggableItemInSlot.QuantityStacked = totalQuantity;
                        draggableItemToDrop.DropItemToSlot(null, true);
                    }

                    // If total quantity is more than stack size limit
                    if (totalQuantity > itemInSlot.maxStackSize)
                    {
                        draggableItemInSlot.QuantityStacked = itemInSlot.maxStackSize;
                        draggableItemToDrop.QuantityStacked = totalQuantity - itemInSlot.maxStackSize;
                        draggableItemToDrop.DropItemToSlot(null);
                    }
                }

                #endregion
            }

            #endregion
            // Else they are differents
            else
            {
                draggableItemToDrop.ExchangeItems(draggableItemInSlot);
            }
        }
        #endregion

        // Else it's free
        else
        {
            if (draggableItemToDrop == null) return;

            draggableItemToDrop.DropItemToSlot(transform);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"OnPointerClick : {name} - {eventData} - {eventData.pointerClick}");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log($"OnPointerEnter : {name} - {eventData} - {eventData.pointerClick}");
    }
}