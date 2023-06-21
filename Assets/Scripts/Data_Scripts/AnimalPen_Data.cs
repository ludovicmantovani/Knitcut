using System.Collections.Generic;

[System.Serializable]
public class AnimalPen_Data
{
    public int nbAnimalPen;

    public int[] animalPenLevels;
    public string[] animalPenTypes;

    public Dictionary<string, float> animalsHunger;

    public int[] totalAnimalsAdults;
    public int[] totalAnimalsChildren;

    public Dictionary<string, int> feedersItems;
    public Dictionary<string, int> feedersItemsQuantities;
    public Dictionary<string, int> feedersItemsFeederIndex;

    public int pedestalItemIndex;

    public AnimalPen_Data(AnimalPenManager animalPenManager)
    {
        Builder(animalPenManager.TotalAnimalPen, 
            animalPenManager.AnimalPenLevels, 
            animalPenManager.AnimalPenTypes, 
            animalPenManager.AnimalsHunger, 
            animalPenManager.TotalAnimalsAdults, 
            animalPenManager.TotalAnimalsChildren,
            animalPenManager.FeedersItems,
            animalPenManager.FeedersItemsQuantities,
            animalPenManager.FeedersItemsFeederIndex,
            animalPenManager.PedestalItemIndex);
    }

    public AnimalPen_Data(AnimalPen_Data animalPen_Data)
    {
        Builder(animalPen_Data.nbAnimalPen, 
            animalPen_Data.animalPenLevels, 
            animalPen_Data.animalPenTypes, 
            animalPen_Data.animalsHunger, 
            animalPen_Data.totalAnimalsAdults,
            animalPen_Data.totalAnimalsChildren,
            animalPen_Data.feedersItems,
            animalPen_Data.feedersItemsQuantities,
            animalPen_Data.feedersItemsFeederIndex,
            animalPen_Data.pedestalItemIndex);
    }

    private void Builder(int total, int[] levels, string[] types, Dictionary<string, float> hungers, int[] totalAdults, int[] totalChildren, 
        Dictionary<string, int> feedersItemsList, Dictionary<string, int> feedersItemsQuantitiesList, Dictionary<string, int> feedersItemsFeederIndexList,
        int pedestalIndex)
    {
        nbAnimalPen = total;

        animalPenLevels = new int[nbAnimalPen];
        animalPenTypes = new string[nbAnimalPen];

        if (hungers != null)
            animalsHunger = hungers;
        else
            animalsHunger = new Dictionary<string, float>();

        totalAnimalsAdults = new int[nbAnimalPen];
        totalAnimalsChildren = new int[nbAnimalPen];

        if (feedersItemsList != null)
            feedersItems = feedersItemsList;
        else
            feedersItems = new Dictionary<string, int>();

        if (feedersItemsQuantitiesList != null)
            feedersItemsQuantities = feedersItemsQuantitiesList;
        else
            feedersItemsQuantities = new Dictionary<string, int>();

        if (feedersItemsFeederIndexList != null)
            feedersItemsFeederIndex = feedersItemsFeederIndexList;
        else
            feedersItemsFeederIndex = new Dictionary<string, int>();

        pedestalItemIndex = pedestalIndex;

        for (int i = 0; i < nbAnimalPen; i++)
        {
            animalPenLevels[i] = levels[i];
            animalPenTypes[i] = types[i];

            totalAnimalsAdults[i] = totalAdults[i];
            totalAnimalsChildren[i] = totalChildren[i];
        }
    }
}