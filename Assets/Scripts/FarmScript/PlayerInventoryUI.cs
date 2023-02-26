using UnityEngine;
using UnityEngine.UI;

public class PlayerInventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject itemUI;

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