using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Container : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject interactionPanel;
    [SerializeField] private GameObject containerInventoryPanel;
    [SerializeField] private GameObject containerInventoryContent;
    [SerializeField] private GameObject playerInventoryPanel;
    [SerializeField] private GameObject itemUI;

    [Header("Container states")]
    [SerializeField] private bool canUseContainer;
    [SerializeField] private bool containerInUse;

    public bool CanUseContainer
    {
        get { return canUseContainer; }
        set { canUseContainer = value; }
    }

    public GameObject ContainerInventory
    {
        get { return containerInventoryPanel; }
        set { containerInventoryPanel = value; }
    }

    public GameObject PlayerInventory
    {
        get { return playerInventoryPanel; }
        set { playerInventoryPanel = value; }
    }

    private void Start()
    {
        canUseContainer = false;
        containerInUse = false;

        interactionPanel.GetComponentInChildren<Text>().text = "Use E to open Container";
    }

    private void Update()
    {
        HandleContainerUse();

        HandleContainerInventory();
    }

    #region Container Use

    private void HandleContainerUse()
    {
        if (canUseContainer)
        {
            interactionPanel.SetActive(true);
        }
        else
        {
            interactionPanel.SetActive(false);
        }
    }

    #endregion

    #region Container Inventory

    private void HandleContainerInventory()
    {
        containerInventoryPanel.SetActive(containerInUse);

        if (!canUseContainer)
        {
            CloseContainerInventory();
            return;
        }

        if (Input.GetKeyDown(KeyCode.E) && canUseContainer)
        {
            if (!containerInUse)
            {
                OpenContainerInventory();
            }
            else
            {
                CloseContainerInventory();
            }
        }
    }

    private void OpenContainerInventory()
    {
        containerInUse = true;

        interactionPanel.GetComponentInChildren<Text>().text = "Use E to close Container";
    }

    private void CloseContainerInventory()
    {
        containerInUse = false;

        interactionPanel.GetComponentInChildren<Text>().text = "Use E to open Container";
    }

    #endregion

    #region Items

    public void AddItemToInventory(Item item, GameObject inventory)
    {
        Transform slotParent = GetFreeSlot(inventory);

        if (slotParent == null) return;

        GameObject itemObject = Instantiate(itemUI, slotParent);

        itemObject.GetComponent<DraggableItem>().Item = item;

        itemObject.GetComponent<Image>().sprite = item.itemSprite;
    }

    private Transform GetFreeSlot(GameObject inventory)
    {
        for (int i = 0; i < inventory.transform.childCount; i++)
        {
            Transform slotObject = inventory.transform.GetChild(i);

            if (slotObject.childCount == 0)
            {
                return slotObject;
            }
        }

        return null;
    }

    #endregion
}