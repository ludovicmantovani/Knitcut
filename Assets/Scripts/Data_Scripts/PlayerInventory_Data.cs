using System;

[System.Serializable]
public class PlayerInventory_Data
{
    //player
    public int[] itemsInSlot;
    public bool[] playerSlotsObjIn;
    public int[] quantityStackedPlayerInventory;

    //recipes
    public int[] recipesIndex;

    public PlayerInventory_Data(List_Slots listSlots, int nbSlots)
    {
        //player
        itemsInSlot = new int[nbSlots];
        playerSlotsObjIn = new bool[nbSlots];
        quantityStackedPlayerInventory = new int[nbSlots];

        for (int i = 0; i < nbSlots; i++)
        {
            itemsInSlot[i] = listSlots.ItemsInSlots[i];
            playerSlotsObjIn[i] = listSlots.PlayerSlotsObjIn[i];
            quantityStackedPlayerInventory[i] = listSlots.QuantityStackedPlayerInventory[i];
        }
    }

    public PlayerInventory_Data(PlayerRecipesInventory playerRecipesInventory)
    {
        recipesIndex = Array.Empty<int>();

        recipesIndex = playerRecipesInventory.RecipesIndex.ToArray();
    }
}