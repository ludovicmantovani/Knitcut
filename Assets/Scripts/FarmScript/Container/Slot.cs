using UnityEngine;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        DraggableItem droppedDItem = dropped.GetComponent<DraggableItem>();

        if (transform.childCount == 1)
        {
            DraggableItem dItemInParent = transform.GetChild(0).GetComponent<DraggableItem>();
            Item itemInParent = dItemInParent.Item;
            if (itemInParent.isStackable)
            {
                if (itemInParent.Equals(droppedDItem.Item) && dItemInParent.QuantityStacked < itemInParent.maxStackSize)
                {

                    if (itemInParent.Equals(droppedDItem.Item) && dItemInParent.QuantityStacked < itemInParent.maxStackSize)
                    {
                        int quantityInDropped = droppedDItem.QuantityStacked;
                        int quantityInDraggable = dItemInParent.QuantityStacked;

                        int totalQuantity = quantityInDropped + quantityInDraggable;

                        if (totalQuantity <= itemInParent.maxStackSize)
                        {
                            dItemInParent.QuantityStacked = totalQuantity;
                            dropped.GetComponent<DraggableItem>().DropItem(null, true);
                        }

                        if (totalQuantity > itemInParent.maxStackSize)
                        {
                            dItemInParent.QuantityStacked = itemInParent.maxStackSize;
                            droppedDItem.QuantityStacked = totalQuantity - itemInParent.maxStackSize;
                            dropped.GetComponent<DraggableItem>().DropItem(null);
                        }
                    }
                }
                else
                {
                    dropped.GetComponent<DraggableItem>().DropItem(null);
                }
            }
            else if (transform.childCount == 0)
            {
                dropped.GetComponent<DraggableItem>().DropItem(transform);
            }
        }

        if (transform.childCount == 0)
        {
            dropped.GetComponent<DraggableItem>().DropItem(transform);
        }
    }
    public void OnDrop2(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        DraggableItem droppedDItem = dropped.GetComponent<DraggableItem>();

        // If there is already an item
        if (transform.childCount > 0)
        {
            DraggableItem dItemInParent = transform.GetChild(0).GetComponent<DraggableItem>();
            Item itemInParent = dItemInParent.Item;

            // If 2 items are same && item is stackable && stack size limit is not reached
            if (itemInParent.Equals(droppedDItem.Item) && itemInParent.isStackable && dItemInParent.QuantityStacked < itemInParent.maxStackSize)
            {
                int quantityInDropped = droppedDItem.QuantityStacked;
                int quantityInDraggable = dItemInParent.QuantityStacked;

                int totalQuantity = quantityInDropped + quantityInDraggable;

                // If total quantity is less or equal than stack size limit
                if (totalQuantity <= itemInParent.maxStackSize)
                {
                    dItemInParent.QuantityStacked = totalQuantity;
                    dropped.GetComponent<DraggableItem>().DropItem(null, true);
                }

                // If total quantity is more than stack size limit
                if (totalQuantity > itemInParent.maxStackSize)
                {
                    dItemInParent.QuantityStacked = itemInParent.maxStackSize;
                    droppedDItem.QuantityStacked = totalQuantity - itemInParent.maxStackSize;
                    dropped.GetComponent<DraggableItem>().DropItem(null);
                }
            }
            else
            {
                dropped.GetComponent<DraggableItem>().DropItem(null);
            }
        }
        // Else it's free
        else
        {
            dropped.GetComponent<DraggableItem>().DropItem(transform);
        }
    }
}