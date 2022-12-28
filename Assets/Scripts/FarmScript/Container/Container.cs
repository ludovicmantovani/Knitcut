using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Container : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject interactionPanel;
    [SerializeField] private GameObject inventoryPanel;

    [Header("Container states")]
    [SerializeField] private bool canUseContainer;
    [SerializeField] private bool containerInUse;

    public bool CanUseContainer
    {
        get { return canUseContainer; }
        set { canUseContainer = value; }
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
        inventoryPanel.SetActive(containerInUse);

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
}