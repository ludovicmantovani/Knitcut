[System.Serializable]
public class PlayerInventory_Data
{
    //player
    public int[] itemsInSlot;
    public bool[] playerSlotsObjIn;
    public int[] quantityStackedPlayerInventory;

    public PlayerInventory_Data(List_Slots LS, int nbSlots)
    {
        //player
        itemsInSlot = new int[nbSlots];
        playerSlotsObjIn = new bool[nbSlots];
        quantityStackedPlayerInventory = new int[nbSlots];

        for (int i = 0; i < nbSlots; i++)
        {
            itemsInSlot[i] = LS.ItemsInSlots[i];
            playerSlotsObjIn[i] = LS.PlayerSlotsObjIn[i];
            quantityStackedPlayerInventory[i] = LS.QuantityStackedPlayerInventory[i];
        }
    }
}