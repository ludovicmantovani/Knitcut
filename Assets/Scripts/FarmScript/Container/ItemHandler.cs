using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Item item;
    [SerializeField] private int quantityStacked = 1;
    [SerializeField] private float uniqueValue = 0f;

    private bool inDrag = false;

    private Transform parentAfterDrag;
    private Transform overSlot;

    private Image image;

    #region Getters / Setters

    public Item Item
    {
        get { return item; }
        set { item = value; }
    }

    public int QuantityStacked
    {
        get { return quantityStacked; }
        set { quantityStacked = value; }
    }

    public float UniqueValue
    {
        get { return uniqueValue; }
        set { uniqueValue = value; }
    }

    public Transform ParentAfterDrag
    {
        get { return parentAfterDrag; }
        set { parentAfterDrag = value; }
    }

    #endregion

    private void Start()
    {
        image = GetComponent<Image>();

        parentAfterDrag = transform.parent;
    }

    private void Update()
    {
        HandleItemSprite();

        if (Input.GetMouseButtonDown(1) && inDrag) HandleDropOneItem();
    }

    private void HandleItemSprite()
    {
        if (item != null)
        {
            if (item.isStackable)
            {
                transform.GetChild(0).gameObject.SetActive(true);
                transform.GetChild(0).GetChild(0).GetComponent<Text>().text = quantityStacked.ToString();
            }
            else
            {
                transform.GetChild(0).gameObject.SetActive(false);
            }

            image.sprite = item.itemSprite;
        }
        else
        {
            image.sprite = null;
        }
    }

    #region Drag

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right) return;

        inDrag = true;

        transform.SetParent(transform.root);
        transform.SetAsLastSibling();

        image.raycastTarget = false;

        overSlot = eventData.pointerEnter.transform;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right) return;
        
        transform.position = Input.mousePosition;

        if (eventData.pointerEnter != null) overSlot = eventData.pointerEnter.transform;
        else overSlot = null;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right) return;

        if (parentAfterDrag.childCount == 0) transform.SetParent(parentAfterDrag);
        else
        {
            ItemHandler itemHandler = parentAfterDrag.GetComponentInChildren<ItemHandler>();

            itemHandler.quantityStacked += quantityStacked;

            Destroy(gameObject);
        }

        image.raycastTarget = true;

        inDrag = false;
    }

    #endregion

    private void HandleDropOneItem()
    {
        if (overSlot == null || item == null || quantityStacked == 1) return;

        if (overSlot.GetComponent<Text>()) return;

        ItemHandler itemFounded = overSlot.GetComponentInChildren<ItemHandler>();

        if (itemFounded == null && overSlot.GetComponent<Slot>())
        {
            GameObject itemUI = Instantiate(gameObject, overSlot);

            itemUI.GetComponent<ItemHandler>().Item = item;
            itemUI.GetComponent<ItemHandler>().QuantityStacked = 1;
            itemUI.GetComponent<Image>().raycastTarget = true;

            quantityStacked -= 1;
        }
        else if (itemFounded != null && overSlot.GetComponent<ItemHandler>())
        {
            if (itemFounded.item == item && itemFounded.UniqueValue == uniqueValue && itemFounded.QuantityStacked < itemFounded.Item.maxStackSize)
            {
                quantityStacked -= 1;

                itemFounded.QuantityStacked += 1;
            }
        }
    }

    public void DropItemToSlot(Transform slot, bool stacked = false)
    {
        if (slot != null) parentAfterDrag = slot;

        if (stacked) Destroy(gameObject);
    }

    public void ExchangeItems(ItemHandler secondItem)
    {
        Transform secondItemSlot = secondItem.ParentAfterDrag;

        if (parentAfterDrag.childCount != 0) return;

        secondItem.ParentAfterDrag = parentAfterDrag;
        secondItem.transform.SetParent(parentAfterDrag);

        parentAfterDrag = secondItemSlot;
    }
}