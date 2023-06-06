using System;

[System.Serializable]
public class PlayerInventory_Data
{
    //player
    public int[] itemsInSlot;
    public bool[] playerSlotsObjIn;
    public int[] quantityStackedPlayerInventory;
    public float[] uniqueValuePlayerInventory;

    //recipes
    public int[] recipesIndex;

    public PlayerInventory_Data(ListSlots listSlots, int nbSlots)
    {
        //player
        itemsInSlot = new int[nbSlots];
        playerSlotsObjIn = new bool[nbSlots];
        quantityStackedPlayerInventory = new int[nbSlots];
        uniqueValuePlayerInventory = new float[nbSlots];

        for (int i = 0; i < nbSlots; i++)
        {
            itemsInSlot[i] = listSlots.ItemsInSlots[i];
            playerSlotsObjIn[i] = listSlots.PlayerSlotsObjIn[i];
            quantityStackedPlayerInventory[i] = listSlots.QuantityStackedPlayerInventory[i];
            uniqueValuePlayerInventory[i] = listSlots.UniqueValuePlayerInventory[i];
        }
    }

    public PlayerInventory_Data(PlayerRecipesInventory playerRecipesInventory)
    {
        recipesIndex = Array.Empty<int>();

        recipesIndex = playerRecipesInventory.RecipesIndex.ToArray();
    }
}