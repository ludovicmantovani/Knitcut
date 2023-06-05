using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private int quantityStacked = 1;
    [SerializeField] private Item item;

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

    #endregion

    private void Start()
    {
        image = GetComponent<Image>();

        parentAfterDrag = transform.parent;
    }

    private void Update()
    {
        HandleItemSprite();

        //Debug.Log(overSlot);

        HandleDropOneItem();
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
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();

        image.raycastTarget = false;

        overSlot = eventData.pointerEnter.transform;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;

        if (eventData.pointerEnter != null) overSlot = eventData.pointerEnter.transform;
        else overSlot = null;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (parentAfterDrag.childCount == 0) transform.SetParent(parentAfterDrag);
        else
        {
            DraggableItem draggableItem = parentAfterDrag.GetChild(0).GetComponent<DraggableItem>();

            draggableItem.quantityStacked += quantityStacked;

            Destroy(gameObject);
        }

        image.raycastTarget = true;
    }

    #endregion

    private void HandleDropOneItem()
    {
        if (overSlot == null) return;

        if (Input.GetMouseButtonDown(1))
        {
            if (item == null || quantityStacked == 1) return;

            quantityStacked -= 1;

            if (overSlot.childCount == 0)
            {
                GameObject itemUI = Instantiate(gameObject, overSlot);

                itemUI.GetComponent<DraggableItem>().Item = item;
                itemUI.GetComponent<DraggableItem>().QuantityStacked = 1;
                itemUI.GetComponent<Image>().raycastTarget = true;
            }
            else
            {
                DraggableItem draggableItem = null;

                if (overSlot.GetComponent<Slot>()) draggableItem = overSlot.GetChild(0).GetComponent<DraggableItem>();
                else if (overSlot.GetComponent<DraggableItem>()) draggableItem = overSlot.GetComponent<DraggableItem>();

                if (draggableItem == null) return;

                draggableItem.QuantityStacked += 1;
            }
        }
    }

    public void DropItemToSlot(Transform slot, bool stacked = false)
    {
        if (slot != null) parentAfterDrag = slot;

        if (stacked) Destroy(gameObject);
    }

    public void ExchangeItems(DraggableItem secondItem)
    {
        Transform secondItemSlot = secondItem.parentAfterDrag;

        secondItem.parentAfterDrag = parentAfterDrag;
        secondItem.transform.SetParent(parentAfterDrag);
        parentAfterDrag = secondItemSlot;
    }
}