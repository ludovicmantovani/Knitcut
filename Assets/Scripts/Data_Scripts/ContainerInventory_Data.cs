using UnityEngine;

[System.Serializable]
public class ContainerInventory_Data
{
    //container
    public int[] containerItemsInSlot;
    public bool[] containerSlotsObjIn;
    public int[] quantityStackedContainerInventory;
    public float[] uniqueValueContainerInventory;

    public ContainerInventory_Data(ListSlots listSlots, int nbSlots)
    {
        //container
        containerItemsInSlot = new int[nbSlots];
        containerSlotsObjIn = new bool[nbSlots];
        quantityStackedContainerInventory = new int[nbSlots];
        uniqueValueContainerInventory = new float[nbSlots];

        for (int i = 0; i < nbSlots; i++)
        {
            containerItemsInSlot[i] = listSlots.ContainerItemsInSlot[i];
            containerSlotsObjIn[i] = listSlots.ContainerSlotsObjIn[i];
            quantityStackedContainerInventory[i] = listSlots.QuantityStackedContainerInventory[i];
            uniqueValueContainerInventory[i] = listSlots.UniqueValueContainerInventory[i];
        }
    }
}