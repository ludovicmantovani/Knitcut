using System.Collections.Generic;

[System.Serializable]
public class AnimalPen_Data
{
    public int nbAnimalPen;

    public int[] animalPenLevels;
    public string[] animalPenTypes;

    public Dictionary<string, float> animalsHunger; // string = animal ID | float = animal hunger

    public int[] totalAnimalsAdults;
    public int[] totalAnimalsChildren;

    public AnimalPen_Data(AnimalPenManager animalPenManager)
    {
        Builder(animalPenManager.TotalAnimalPen, 
            animalPenManager.AnimalPenLevels, 
            animalPenManager.AnimalPenTypes, 
            animalPenManager.AnimalsHunger, 
            animalPenManager.TotalAnimalsAdults, 
            animalPenManager.TotalAnimalsChildren);
    }

    public AnimalPen_Data(AnimalPen_Data animalPen_Data)
    {
        Builder(animalPen_Data.nbAnimalPen, 
            animalPen_Data.animalPenLevels, 
            animalPen_Data.animalPenTypes, 
            animalPen_Data.animalsHunger, 
            animalPen_Data.totalAnimalsAdults, 
            animalPen_Data.totalAnimalsChildren);
    }

    private void Builder(int total, int[] levels, string[] types, Dictionary<string, float> hungers, int[] totalAdults, int[] totalChildren)
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

        for (int i = 0; i < nbAnimalPen; i++)
        {
            animalPenLevels[i] = levels[i];
            animalPenTypes[i] = types[i];

            totalAnimalsAdults[i] = totalAdults[i];
            totalAnimalsChildren[i] = totalChildren[i];
        }
    }
}