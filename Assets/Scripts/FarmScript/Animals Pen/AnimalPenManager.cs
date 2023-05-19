using System;
using System.Collections.Generic;
using UnityEngine;

public class AnimalPenManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private List<AnimalPen> animalPenList;
    [SerializeField] private List<GameObject> animals;
    [SerializeField] private List<GameObject> animalsChildren;

    // AnimalData Pen
    private int totalAnimalPen;
    private int[] animalPenLevels;
    private string[] animalPenTypes;

    // Captured Animals
    private int[] totalAnimalsAdults;
    private int[] totalAnimalsChildren;

    [Serializable]
    public class AnimalPen
    {
        public GameObject animalPenInScene;
        public GameObject currentAnimalPenState;
        public List<AnimalPenStates> animalPenStates;
        public AnimalType animalType;
        public int animalPenLevel = 1;
    }

    [Serializable]
    public class AnimalPenStates
    {
        public GameObject animalPenObject;
        public int levelRequired;
        public int maxAdultsRestriction = 1;
        public int maxChildrenRestriction = 0;
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

    public int[] TotalAnimalsAdults
    {
        get { return totalAnimalsAdults; }
        set { totalAnimalsAdults = value; }
    }

    public int[] TotalAnimalsChildren
    {
        get { return totalAnimalsChildren; }
        set { totalAnimalsChildren = value; }
    }

    private void Start()
    {
        InitializeData();

        LoadAnimalPenData();
    }

    private void InitializeData()
    {
        totalAnimalPen = animalPenList.Count;

        animalPenLevels = new int[totalAnimalPen];
        animalPenTypes = new string[totalAnimalPen];

        totalAnimalsAdults = new int[totalAnimalPen];
        totalAnimalsChildren = new int[totalAnimalPen];

        for (int i = 0; i < totalAnimalPen; i++)
        {
            animalPenLevels[i] = animalPenList[i].animalPenLevel;
            animalPenTypes[i] = animalPenList[i].animalType.ToString();

            totalAnimalsAdults[i] = 0;
            totalAnimalsChildren[i] = 0;
        }
    }

    public void SaveAnimalPenData()
    {
        SaveSystem.Save(SaveSystem.SaveType.Save_AnimalPen, this);
    }

    private void LoadAnimalPenData()
    {
        AnimalPen_Data data = (AnimalPen_Data)SaveSystem.Load(SaveSystem.SaveType.Save_AnimalPen, this);

        if (data == null)
        {
            LoadAnimalPenData();
            return;
        }

        animalPenLevels = data.animalPenLevels;
        animalPenTypes = data.animalPenTypes;

        totalAnimalsAdults = data.totalAnimalsAdults;
        totalAnimalsChildren = data.totalAnimalsChildren;

        if (MinigameManager.AnimalPenIndexToUpgrade.Count > 0)
        {
            for (int i = 0; i < MinigameManager.AnimalPenIndexToUpgrade.Count; i++)
            {
                animalPenLevels[MinigameManager.AnimalPenIndexToUpgrade[i]]++;
            }

            MinigameManager.AnimalPenIndexToUpgrade = new List<int>();

            SaveAnimalPenData();
        }

        //HandleStates();

        LoadAllAnimals(totalAnimalsAdults, animals);
        LoadAllAnimals(totalAnimalsChildren, animalsChildren);

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
                HandleCurrentAnimalPenState(animalPen, animalPen.animalPenStates[j]);
            }
        }
    }

    private void HandleCurrentAnimalPenState(AnimalPen animalPen, AnimalPenStates currentState)
    {
        if (currentState.levelRequired == animalPen.animalPenLevel)
        {
            animalPen.currentAnimalPenState = currentState.animalPenObject;

            currentState.animalPenObject.SetActive(true);

            ActualizeAnimals(animalPen, animalPen.animalPenInScene);
        }
        else
            currentState.animalPenObject.SetActive(false);
    }

    private void ActualizeAnimals(AnimalPen animalPen, GameObject currentAnimalPenState)
    {
        for (int i = 0; i < currentAnimalPenState.transform.childCount; i++)
        {
            if (currentAnimalPenState.transform.GetChild(i).CompareTag("Animal"))
            {
                AnimalData animalData = currentAnimalPenState.transform.GetChild(i).GetComponent<AnimalData>();

                animalData.CurrentAnimalPen = animalPen.currentAnimalPenState;
            }
        }
    }

    private void LoadAllAnimals(int[] totalAnimals, List<GameObject> animalsList)
    {
        int totalAnimalsLoaded = 0;

        for (int i = 0; i < totalAnimals.Length; i++)
        {
            int totalAnimalsInPen = totalAnimals[i];

            for (int k = 0; k < totalAnimalsInPen; k++)
            {
                GameObject animal = GetAnimalWithType(animalsList, animalPenList[i].animalType);

                totalAnimalsLoaded++;

                InstantiateAnimal(animal, animalPenList[i].animalPenInScene.transform);
            }
        }

        SaveAnimalPenData();
    }

    private GameObject GetAnimalWithType(List<GameObject> animalsList, AnimalType animalType)
    {
        for (int i = 0; i < animalsList.Count; i++)
        {
            if (animalsList[i].GetComponent<AnimalData>().AnimalType == animalType)
            {
                return animalsList[i];
            }
        }

        return null;
    }

    public void InstantiateTamedAnimalInAnimalPen(bool isChildren = false)
    {
        GameObject tamedAnimal = null;

        List<GameObject> animalsList;

        if (!isChildren)
            animalsList = animals;
        else
            animalsList = animalsChildren;

        if (animalsList == null) return;

        for (int i = 0; i < animalsList.Count; i++)
        {
            if (animalsList[i].GetComponent<AnimalData>().AnimalType == MinigameManager.AnimalTypeToKeep)
            {
                tamedAnimal = animalsList[i];
            }
        }

        if (tamedAnimal == null) return;

        int animalPenOfAnimalIndex = -1;

        for (int i = 0; i < animalPenList.Count; i++)
        {
            if (animalPenList[i].animalType == MinigameManager.AnimalTypeToKeep)
            {
                animalPenOfAnimalIndex = i;
            }
        }

        if (animalPenOfAnimalIndex == -1) return;

        if (!isChildren)
            totalAnimalsAdults[animalPenOfAnimalIndex]++;
        else
            totalAnimalsChildren[animalPenOfAnimalIndex]++;

        MinigameManager.AnimalTypeToKeep = AnimalType.None;

        InstantiateAnimal(tamedAnimal, animalPenList[animalPenOfAnimalIndex].animalPenInScene.transform);
    }

    private void InstantiateAnimal(GameObject animalObject, Transform animalPen)
    {
        GameObject animal = Instantiate(animalObject, animalPen);

        SaveAnimalPenData();

        HandleStates();
    }

    #region Animal Pen Restrictions

    public bool CheckAnimalPenRestrictions(AnimalAI animal, bool checkChild = false)
    {
        AnimalPen linkedAnimalPen = GetLinkedAnimalPen(animal.AnimalType);

        bool restrictionOK = CheckingRestrictions(linkedAnimalPen, checkChild);

        return restrictionOK;
    }

    public bool CheckAnimalPenRestrictions(AnimalType animalType, bool checkChild = false)
    {
        AnimalPen linkedAnimalPen = GetLinkedAnimalPen(animalType);

        bool restrictionOK = CheckingRestrictions(linkedAnimalPen, checkChild);

        return restrictionOK;
    }

    private bool CheckingRestrictions(AnimalPen linkedAnimalPen, bool checkChild)
    {
        bool restrictionOK = false;

        if (linkedAnimalPen == null) return false;

        AnimalPenStates currentRestrictions = GetCurrentRestrictions(linkedAnimalPen);

        if (currentRestrictions == null) return false;

        int[] animalsCount = GetAnimalsCount(linkedAnimalPen.animalPenInScene.transform);

        if (!checkChild && animalsCount[0] < currentRestrictions.maxAdultsRestriction) restrictionOK = true;
        if (checkChild && animalsCount[1] < currentRestrictions.maxChildrenRestriction) restrictionOK = true;

        return restrictionOK;
    }

    public AnimalPen GetLinkedAnimalPen(AnimalType animalType)
    {
        AnimalPen linkedAnimalPen = null;

        for (int i = 0; i < animalPenList.Count; i++)
        {
            if (animalPenList[i].animalType == animalType)
            {
                linkedAnimalPen = animalPenList[i];
            }
        }

        return linkedAnimalPen;
    }

    public AnimalPenStates GetCurrentRestrictions(AnimalPen animalPen)
    {
        AnimalPenStates currentAnimalStates = null;

        for (int i = 0; i < animalPen.animalPenStates.Count; i++)
        {
            if (animalPen.animalPenStates[i].levelRequired == animalPen.animalPenLevel)
            {
                currentAnimalStates = animalPen.animalPenStates[i];
            }
        }

        return currentAnimalStates;
    }

    public int[] GetAnimalsCount(Transform animalPen)
    {
        int countAdults = 0;
        int countChildren = 0;

        for (int i = 0; i < animalPen.childCount; i++)
        {
            if (animalPen.GetChild(i).CompareTag("Animal"))
            {
                AnimalStates animalStates = animalPen.GetChild(i).GetComponent<AnimalStates>();

                if (animalStates.IsChild)
                    countChildren++;
                else
                    countAdults++;
            }
        }

        return new int[2] { countAdults, countChildren };
    }

    #endregion
}