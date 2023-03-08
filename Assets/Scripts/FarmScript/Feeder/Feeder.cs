using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class Feeder : MonoBehaviour
{
    public PlayerInput pI;
    [Header("Feeder Inventory")]
    [SerializeField] private bool canUseFeeder;
    [SerializeField] private bool feederInUse;
    [SerializeField] private GameObject feederInventory;
    [SerializeField] private GameObject interactionPanel;
    [SerializeField] private GameObject feederModel;

    public bool CanUseFeeder
    {
        get { return canUseFeeder; }
        set { canUseFeeder = value; }
    }

    private void Start()
    {
        pI = GetComponent<PlayerInput>();
        canUseFeeder = false;
        feederInUse = false;

        interactionPanel.GetComponentInChildren<Text>().text = "Use E to open Feeder";
    }

    private void Update()
    {
        // GameObject Visual
        HandleVisual();

        // Inventory
        HandleFeederUse();
        HandleFeederInventory();
    }

    private void HandleVisual()
    {
        Vector3 positionVisual = feederModel.transform.GetChild(0).localPosition;

        if (IsFeederEmpty())
        {
            positionVisual.y = -0.001f;
        }
        else
        {
            positionVisual.y = 0.6f;
        }

        feederModel.transform.GetChild(0).localPosition = positionVisual;
    }

    public Item GetFood()
    {
        if (IsFeederEmpty()) return null;

        return GetItem();
    }

    #region Feeder Inventory

    #region Inventory UI

    private void HandleFeederUse()
    {
        if (canUseFeeder)
        {
            interactionPanel.SetActive(true);
        }
        else
        {
            interactionPanel.SetActive(false);
        }
    }

    private void HandleFeederInventory()
    {
        feederInventory.SetActive(feederInUse);

        if (!canUseFeeder)
        {
            CloseFeederInventory();
            return;
        }

        if (pI.actions["Intercation_Environnements"].triggered && canUseFeeder)
        {
            if (!feederInUse)
            {
                OpenFeederInventory();
            }
            else
            {
                CloseFeederInventory();
            }
        }
    }

    private void OpenFeederInventory()
    {
        feederInUse = true;

        interactionPanel.GetComponentInChildren<Text>().text = "Use " + pI.actions["Intercation_Environnements"].GetBindingDisplayString() + " to close Feeder";

        MinigameManager.AddOpenInventory(feederInventory);
    }

    private void CloseFeederInventory()
    {
        feederInUse = false;

        interactionPanel.GetComponentInChildren<Text>().text = "Use " + pI.actions["Intercation_Environnements"].GetBindingDisplayString() + " to open Feeder";

        MinigameManager.RemoveOpenInventory(feederInventory);
    }

    #endregion

    #region Inventory Content

    private Item GetItem()
    {
        for (int i = 0; i < feederInventory.transform.childCount; i++)
        {
            Transform slot = feederInventory.transform.GetChild(i);

            // If item present in slot
            if (slot.childCount > 0)
            {
                DraggableItem draggableItem = slot.GetChild(0).GetComponent<DraggableItem>();

                // If item is consumable
                if (draggableItem.Item.itemType == ItemType.Consumable && draggableItem.QuantityStacked > 0)
                {
                    return draggableItem.Item;
                }
            }
        }

        return null;
    }

    public void RemoveItem(Item item)
    {
        for (int i = 0; i < feederInventory.transform.childCount; i++)
        {
            Transform slot = feederInventory.transform.GetChild(i);

            // If item present in slot
            if (slot.childCount > 0)
            {
                DraggableItem draggableItem = slot.GetChild(0).GetComponent<DraggableItem>();

                if (draggableItem.Item == item)
                {
                    draggableItem.QuantityStacked -= 1;

                    if (draggableItem.QuantityStacked <= 0)
                    {
                        Destroy(draggableItem.gameObject);
                    }
                }
            }
        }
    }

    private bool IsFeederEmpty()
    {
        bool isEmpty = true;

        for (int i = 0; i < feederInventory.transform.childCount; i++)
        {
            Transform slot = feederInventory.transform.GetChild(i);

            if (slot.childCount > 0)
            {
                isEmpty = false;
            }
        }

        return isEmpty;
    }

    #endregion

    #endregion
}