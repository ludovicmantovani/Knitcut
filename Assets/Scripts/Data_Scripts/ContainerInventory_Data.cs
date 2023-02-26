[System.Serializable]
public class ContainerInventory_Data
{
    //container
    public int[] quantityStackedContainerInventory;
    public int[] containerItemsInSlot;
    public bool[] containerSlotsObjIn;

    public ContainerInventory_Data(List_Slots LS)
    {
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
