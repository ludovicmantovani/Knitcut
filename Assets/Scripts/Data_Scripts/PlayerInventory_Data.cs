[System.Serializable]
public class PlayerInventory_Data
{
    //player
    public int[] quantityStackedPlayerInventory;
    public int[] itemsInSlot;
    public bool[] playerSlotsObjIn;

    public PlayerInventory_Data(List_Slots LS)
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
    }
}
