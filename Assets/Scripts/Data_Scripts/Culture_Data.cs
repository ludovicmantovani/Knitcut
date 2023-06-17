using UnityEngine;

[System.Serializable]
public class Culture_Data
{
    public int cropsCount;

    public int[] cropsSeeds;

    public bool[] cropsCultivation;

    public string[] cropsGrowth;
    public string[] cropsState;

    public float[] cropsTimer;

    public int seedIndexInSlot;
    public int seedQuantityInSlot;

    public Culture_Data(CultureManager cultureManager)
    {
        cropsCount = cultureManager.CropsCount;

        cropsSeeds = new int[cropsCount];

        cropsCultivation = new bool[cropsCount];

        cropsGrowth = new string[cropsCount];
        cropsState = new string[cropsCount];

        cropsTimer = new float[cropsCount];

        seedIndexInSlot = cultureManager.SeedIndexInSlot;
        seedQuantityInSlot = cultureManager.SeedQuantityInSlot;

        for (int i = 0; i < cropsCount; i++)
        {
            cropsSeeds[i] = cultureManager.CropsSeeds[i];

            cropsCultivation[i] = cultureManager.CropsCultivation[i];

            cropsGrowth[i] = cultureManager.CropsGrowth[i];
            cropsState[i] = cultureManager.CropsState[i];

            cropsTimer[i] = cultureManager.CropsTimer[i];
        }
    }
}