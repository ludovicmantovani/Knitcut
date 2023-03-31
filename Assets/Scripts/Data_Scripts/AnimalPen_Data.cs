[System.Serializable]
public class AnimalPen_Data
{
    public int nbAnimalPen;

    public int[] animalPenLevels;
    public string[] animalPenTypes;

    public AnimalPen_Data(AnimalPenManager animalPenManager)
    {
        nbAnimalPen = animalPenManager.TotalAnimalPen;

        animalPenLevels = new int[nbAnimalPen];
        animalPenTypes = new string[nbAnimalPen];

        for (int i = 0; i < nbAnimalPen; i++)
        {
            animalPenLevels[i] = animalPenManager.AnimalPenLevels[i];
            animalPenTypes[i] = animalPenManager.AnimalPenTypes[i];
        }
    }
}