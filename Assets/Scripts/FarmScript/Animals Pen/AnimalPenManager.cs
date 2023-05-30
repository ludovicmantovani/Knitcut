using System;
using System.Collections.Generic;
using System.Linq;
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

    private Dictionary<string, float> animalsHunger; // string = animal ID | float = animal hunger

    // Captured Animals
    private int[] totalAnimalsAdults;
    private int[] totalAnimalsChildren;

    [Serializable]
    public class AnimalPen
    {
        public GameObject animalPenInScene;
        public GameObject currentAnimalPenState;
        public GameObject currentFeeder;
        public GameObject currentBell;
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

    #region Getters / Setters

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

    public Dictionary<string, float> AnimalsHunger
    {
        get { return animalsHunger; }
        set { animalsHunger = value; }
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

    #endregion

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

        animalsHunger = new Dictionary<string, float>();

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
        ActualizeAnimalsHunger();

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

        animalsHunger = data.animalsHunger;

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
            animalPen.currentFeeder = currentState.animalPenObject.GetComponent<AnimalPenRef>().Feeder;
            animalPen.currentBell = currentState.animalPenObject.GetComponent<AnimalPenRef>().Bell;

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

                InstantiateAnimal(animal, animalPenList[i]);
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

        // Adult or Child
        if (!isChildren)
            animalsList = animals;
        else
            animalsList = animalsChildren;

        if (animalsList == null) return;

        // Get animal prefab
        for (int i = 0; i < animalsList.Count; i++)
        {
            if (animalsList[i].GetComponent<AnimalData>().AnimalType == MinigameManager.AnimalTypeToKeep)
            {
                tamedAnimal = animalsList[i];
            }
        }

        if (tamedAnimal == null) return;

        // Get current animal pen
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

        InstantiateAnimal(tamedAnimal, animalPenList[animalPenOfAnimalIndex]);

        ActualizeAnimals(animalPenList[animalPenOfAnimalIndex], animalPenList[animalPenOfAnimalIndex].animalPenInScene);
    }

    private void InstantiateAnimal(GameObject animalObject, AnimalPen currentAnimalPen)
    {
        Transform currentAnimalPenInScene = currentAnimalPen.animalPenInScene.transform;

        AnimalStates animal = Instantiate(animalObject, currentAnimalPenInScene).GetComponent<AnimalStates>();

        string animalID = GenerateAnimalID(animal.GetComponent<AnimalData>());

        animal.AnimalID = animalID;

        if (animalsHunger.TryGetValue(animalID, out float hungerSaved))
        {
            animal.Hunger = hungerSaved;
        }
        else if (!animalsHunger.ContainsKey(animalID))
        {
            animalsHunger.Add(animalID, animal.Hunger);
        }

        // Random Position
        Transform surface = currentAnimalPenInScene.GetComponentInChildren<AnimalPenRef>().Surface.transform;

        Vector3 randomPosition = GetAnimalPenRandomPos(surface);

        if (randomPosition == Vector3.zero) return;

        animal.transform.position = randomPosition;
    }

    private Vector3 GetAnimalPenRandomPos(Transform surface)
    {
        if (surface == null) return Vector3.zero;

        float xLimit = surface.GetComponent<Renderer>().bounds.size.x;
        float zLimit = surface.GetComponent<Renderer>().bounds.size.z;

        float randomX = UnityEngine.Random.Range(-xLimit / 2, xLimit / 2);
        float randomZ = UnityEngine.Random.Range(-zLimit / 2, zLimit / 2);

        Vector3 randomPosition = surface.position + new Vector3(randomX, transform.position.y, randomZ);

        return randomPosition;
    }

    private void ActualizeAnimalsHunger()
    {
        foreach (string key in animalsHunger.Keys.ToList())
        {
            AnimalStates animal = GetAnimalByID(key);

            if (animal == null) return;

            animalsHunger[key] = animal.Hunger;
        }
    }

    private string GenerateAnimalID(AnimalData animal)
    {
        string id = "";

        for (int i = 0; i < animals.Count; i++)
        {
            if (animals[i].GetComponent<AnimalData>().AnimalType == animal.AnimalType) id = $"0{i}";
        }

        AnimalPen animalPen = GetLinkedAnimalPen(animal.AnimalType);

        int[] animalInPenCount = GetAnimalsCount(animalPen.animalPenInScene.transform);

        int totalAnimalsInPen = animalInPenCount[0] + animalInPenCount[1];

        id += $"0{totalAnimalsInPen}";

        if (animal.GetComponent<AnimalStates>().IsChild)
            id += $"01";
        else
            id += $"02";

        return id;
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

    private AnimalStates GetAnimalByID(string id)
    {
        AnimalStates animal = null;

        AnimalStates[] animalsInScene = FindObjectsOfType<AnimalStates>();

        for (int i = 0; i < animalsInScene.Length; i++)
        {
            if (!animalsInScene[i].IsChild && animalsInScene[i].AnimalID == id) animal = animalsInScene[i];
        }

        return animal;
    }

    #endregion
}