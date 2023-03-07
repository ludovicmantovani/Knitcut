[System.Serializable]
public class ContainerInventory_Data
{
    //container
    public int[] containerItemsInSlot;
    public bool[] containerSlotsObjIn;
    public int[] quantityStackedContainerInventory;

    public ContainerInventory_Data(List_Slots LS, int nbSlots)
    {
        //container
        containerItemsInSlot = new int[nbSlots];
        containerSlotsObjIn = new bool[nbSlots];
        quantityStackedContainerInventory = new int[nbSlots];

        for (int i = 0; i < nbSlots; i++)
        {
            containerItemsInSlot[i] = LS.ContainerItemsInSlot[i];
            containerSlotsObjIn[i] = LS.ContainerSlotsObjIn[i];
            quantityStackedContainerInventory[i] = LS.QuantityStackedContainerInventory[i];
        }
    }
}