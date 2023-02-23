using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Inventory_Data 
{
    //player
    public int[] quantityStackedPlayerInventory;
    public int[] itemsInSlot;
    public bool[] playerSlotsObjIn;
    //container
    public int[] quantityStackedContainerInventory;
    public int[] containerItemsInSlot;
    public bool[] containerSlotsObjIn;

    public Inventory_Data(List_Slots LS)
    {
        //player
        playerSlotsObjIn = new bool[10];
        for (int i = 0; i < LS.playerSlots.Length; i++)
        {
            playerSlotsObjIn[i] = LS.playerSlotsObjIn[i];
        }
        quantityStackedPlayerInventory = new int[10];
        for (int i = 0; i < LS.quantityStackedPlayerInventory.Length; i++)
        {
            quantityStackedPlayerInventory[i] = LS.quantityStackedPlayerInventory[i];
        }
        itemsInSlot = new int[10];
        for (int i = 0; i < LS.itemsInSlot.Length; i++)
        {
            itemsInSlot[i] = LS.itemsInSlot[i];
        }
        //container
        containerSlotsObjIn = new bool[100];
        for (int i = 0; i < LS.containerSlots.Length; i++)
        {
            containerSlotsObjIn[i] = LS.containerSlotsObjIn[i];
        }
        quantityStackedContainerInventory = new int[100];
        for (int i = 0; i < LS.quantityStackedContainerInventory.Length; i++)
        {
            quantityStackedContainerInventory[i] = LS.quantityStackedContainerInventory[i];
        }
        containerItemsInSlot = new int[100];
        for (int i = 0; i < LS.containerItemsInSlot.Length; i++)
        {
            containerItemsInSlot[i] = LS.containerItemsInSlot[i];
        }
    }
}
