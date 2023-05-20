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
        nbAnimalPen = animalPenManager.TotalAnimalPen;

        animalPenLevels = new int[nbAnimalPen];
        animalPenTypes = new string[nbAnimalPen];

        if (animalPenManager.AnimalsHunger != null)
            animalsHunger = animalPenManager.AnimalsHunger;
        else
            animalsHunger = new Dictionary<string, float>();

        totalAnimalsAdults = new int[nbAnimalPen];
        totalAnimalsChildren = new int[nbAnimalPen];

        for (int i = 0; i < nbAnimalPen; i++)
        {
            animalPenLevels[i] = animalPenManager.AnimalPenLevels[i];
            animalPenTypes[i] = animalPenManager.AnimalPenTypes[i];

            totalAnimalsAdults[i] = animalPenManager.TotalAnimalsAdults[i];
            totalAnimalsChildren[i] = animalPenManager.TotalAnimalsChildren[i];
        }
    }
}