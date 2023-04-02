using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AnimalPenManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private List<AnimalPen> animalPenList;
    [SerializeField] private List<GameObject> animals;

    private int totalAnimalPen;
    private int[] animalPenLevels;
    private string[] animalPenTypes;

    [Serializable]
    public class AnimalPen
    {
        public GameObject animalPenInScene;
        public List<AnimalPenStates> animalPenStates;
        public AnimalType animalType;
        public int animalPenLevel = 1;
    }

    [Serializable]
    public class AnimalPenStates
    {
        public GameObject animalPenObject;
        public int levelRequired;
    }

    public int TotalAnimalPen
    {
        get { return totalAnimalPen; }
        set { totalAnimalPen = value; }
    }

    public int[] AnimalPenLevels
    {
        get { return animalPenLevels; }
        set { animalPenLevels = value; }
    }

    public string[] AnimalPenTypes
    {
        get { return animalPenTypes; }
        set { animalPenTypes = value; }
    }

    private void Start()
    {
        InitializeData();

        LoadAnimalPenLevels();
    }

    private void InitializeData()
    {
        totalAnimalPen = animalPenList.Count;

        animalPenLevels = new int[totalAnimalPen];
        animalPenTypes = new string[totalAnimalPen];

        for (int i = 0; i < totalAnimalPen; i++)
        {
            animalPenLevels[i] = animalPenList[i].animalPenLevel;
            animalPenTypes[i] = animalPenList[i].animalType.ToString();
        }
    }

    public void SaveAnimalPenLevels()
    {
        SaveSystem.Save(SaveSystem.SaveType.Save_AnimalPen, this);
    }

    private void LoadAnimalPenLevels()
    {
        AnimalPen_Data data = (AnimalPen_Data)SaveSystem.Load(SaveSystem.SaveType.Save_AnimalPen, this);

        if (data == null)
        {
            LoadAnimalPenLevels();
            return;
        }

        animalPenLevels = data.animalPenLevels;
        animalPenTypes = data.animalPenTypes;

        if (MinigameManager.AnimalPenIndexToUpgrade.Count > 0)
        {
            for (int i = 0; i < MinigameManager.AnimalPenIndexToUpgrade.Count; i++)
            {
                animalPenLevels[MinigameManager.AnimalPenIndexToUpgrade[i]]++;
            }

            MinigameManager.AnimalPenIndexToUpgrade = new List<int>();

            SaveAnimalPenLevels();
        }

        HandleStates();
    }

    private void HandleStates()
    {
        for (int i = 0; i < animalPenList.Count; i++)
        {
            AnimalPen animalPen = animalPenList[i];

            animalPen.animalPenLevel = animalPenLevels[i];

            for (int j = 0; j < animalPen.animalPenStates.Count; j++)
            {
                AnimalPenStates currentState = animalPen.animalPenStates[j];

                if (currentState.levelRequired == animalPen.animalPenLevel)
                    currentState.animalPenObject.SetActive(true);
                else
                    currentState.animalPenObject.SetActive(false);
            }
        }
    }

    public void InstantiateTamedAnimalInAnimalPen()
    {
        GameObject tamedAnimal = null;

        for (int i = 0; i < animals.Count; i++)
        {
            if (animals[i].GetComponent<AnimalStates>().AnimalType == MinigameManager.AnimalTypeToKeep)
            {
                tamedAnimal = animals[i];
            }
        }

        if (tamedAnimal == null) return;

        Transform animalPenOfAnimal = null;

        for (int i = 0; i < animalPenList.Count; i++)
        {
            if (animalPenList[i].animalType == MinigameManager.AnimalTypeToKeep)
            {
                animalPenOfAnimal = animalPenList[i].animalPenInScene.transform;
            }
        }

        if (animalPenOfAnimal == null) return;

        GameObject animal = Instantiate(tamedAnimal, animalPenOfAnimal);

        MinigameManager.AnimalTypeToKeep = AnimalType.None;

        Debug.Log($"{animal.name} placed in {animalPenOfAnimal.name}");
    }
}