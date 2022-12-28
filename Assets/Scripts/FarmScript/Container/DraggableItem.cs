using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [HideInInspector] public Transform parentAfterDrag;

    [SerializeField] private int quantityStacked = 1;
    [SerializeField] private Item item;

    public Item Item
    {
        get { return item; }
    }

    public int QuantityStacked
    {
        get { return quantityStacked; }
        set { quantityStacked = value; }
    }

    private Image image;

    private void Start()
    {
        image = GetComponent<Image>();
    }

    private void Update()
    {
        HandleItemSprite();
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
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        image.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(parentAfterDrag);
        image.raycastTarget = true;
    }

    #endregion

    public void DropItem(Transform parent, bool stacked = false)
    {
        if (parent != null)
        {
            parentAfterDrag = parent;
        }

        if (stacked)
        {
            Destroy(gameObject);
        }
    }
}