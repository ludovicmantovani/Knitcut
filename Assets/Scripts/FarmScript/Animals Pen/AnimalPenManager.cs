using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class AnimalPenManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private List<AnimalPen> animalPenList;
    [SerializeField] private List<string> animalPenPictos;
    [SerializeField] private List<GameObject> animals;
    [SerializeField] private List<GameObject> animalsChildren;

    private ListSlots listSlots;
    private CaptureManager captureManager;

    // AnimalData Pen
    private int totalAnimalPen;
    private int[] animalPenLevels;
    private string[] animalPenTypes;

    private Dictionary<string, float> animalsHunger;

    // Captured Animals
    private int[] totalAnimalsAdults;
    private int[] totalAnimalsChildren;

    // Feeders
    private Dictionary<string, int> feedersItems;
    private Dictionary<string, int> feedersItemsQuantities;
    private Dictionary<string, int> feedersItemsFeederIndex;

    // Pedestal Capture Area
    private int pedestalItemIndex;
    private int pedestalItemQuantity;

    [Serializable]
    public class AnimalPen
    {
        public GameObject animalPenInScene;
        public GameObject currentAnimalPenState;
        public GameObject currentFeeder;
        public GameObject currentBell;
        public GameObject currentPanel;
        public Material animalPanelMat;
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

    public List<string> AnimalPenPictos
    {
        get { return animalPenPictos; }
        set { animalPenPictos = value; }
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

    public Dictionary<string, int> FeedersItems
    {
        get { return feedersItems; }
        set { feedersItems = value; }
    }

    public Dictionary<string, int> FeedersItemsQuantities
    {
        get { return feedersItemsQuantities; }
        set { feedersItemsQuantities = value; }
    }

    public Dictionary<string, int> FeedersItemsFeederIndex
    {
        get { return feedersItemsFeederIndex; }
        set { feedersItemsFeederIndex = value; }
    }

    public int PedestalItemIndex
    {
        get { return pedestalItemIndex; }
        set { pedestalItemIndex = value; }
    }

    public int PedestalItemQuantity
    {
        get { return pedestalItemQuantity; }
        set { pedestalItemQuantity = value; }
    }

    #endregion

    private void Start()
    {
        listSlots = FindObjectOfType<ListSlots>();
        captureManager = FindObjectOfType<CaptureManager>();

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

        feedersItems = new Dictionary<string, int>();
        feedersItemsQuantities = new Dictionary<string, int>();
        feedersItemsFeederIndex = new Dictionary<string, int>();

        pedestalItemIndex = -1;
        pedestalItemQuantity = 0;

        for (int i = 0; i < totalAnimalPen; i++)
        {
            animalPenLevels[i] = animalPenList[i].animalPenLevel;
            animalPenTypes[i] = animalPenList[i].animalType.ToString();

            totalAnimalsAdults[i] = 0;
            totalAnimalsChildren[i] = 0;
        }
    }

    public Transform GetAnimalPenWithPicto(string pictoName)
    {
        for (int i = 0; i < animalPenPictos.Count; i++)
        {
            if (animalPenPictos[i] == pictoName) return animalPenList[i].animalPenInScene.transform;
        }

        return null;
    }

    private void SaveCaptureFruitPlaced()
    {
        ItemHandler itemHandler = captureManager.GetItemData();

        if (itemHandler == null)
        {
            pedestalItemIndex = -1;
            pedestalItemQuantity = 0;

            return;
        }

        pedestalItemIndex = listSlots.GetItemIndex(itemHandler.Item);
        pedestalItemQuantity = itemHandler.QuantityStacked;
    }

    public void SaveAnimalPenData()
    {
        ActualizeAnimalsHunger();

        SaveFeedersContent();

        SaveCaptureFruitPlaced();

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

        feedersItems = data.feedersItems;
        feedersItemsQuantities = data.feedersItemsQuantities;
        feedersItemsFeederIndex = data.feedersItemsFeederIndex;

        pedestalItemIndex = data.pedestalItemIndex;
        pedestalItemQuantity = data.pedestalItemQuantity;

        captureManager.LoadFruitPlaced(pedestalItemIndex, pedestalItemQuantity);

        HandleStates();

        LoadFeedersContent();

        LoadAllAnimals(totalAnimalsAdults, animals);
        LoadAllAnimals(totalAnimalsChildren, animalsChildren);

        SaveAnimalPenData();
    }

    #region Handle Animal Pen Feeders

    private void SaveFeedersContent()
    {
        for (int indexAnimalPen = 0; indexAnimalPen < totalAnimalPen; indexAnimalPen++)
        {
            AnimalPen animalPen = animalPenList[indexAnimalPen];

            Feeder currentFeeder = animalPen.currentFeeder.GetComponent<Feeder>();

            if (currentFeeder == null) return;

            GetItemsAndQuantityOfFeeder(currentFeeder, indexAnimalPen);
        }
    }

    private void GetItemsAndQuantityOfFeeder(Feeder currentFeeder, int indexAnimalPen)
    {
        for (int i = 0; i < currentFeeder.Items.Length; i++)
        {
            string indexString = $"{indexAnimalPen}_{i}";

            Item item = currentFeeder.Items[i];
            int quantity = currentFeeder.ItemsQuantities[i];

            if (item != null && quantity > 0)
            {
                // Item
                if (!feedersItems.ContainsKey(indexString))
                    feedersItems.Add(indexString, listSlots.GetItemIndex(item));
                else if (feedersItems.ContainsKey(indexString))
                    feedersItems[indexString] = listSlots.GetItemIndex(item);

                // Quantity
                if (!feedersItemsQuantities.ContainsKey(indexString))
                    feedersItemsQuantities.Add(indexString, quantity);
                else if (feedersItemsQuantities.ContainsKey(indexString))
                    feedersItemsQuantities[indexString] = quantity;

                // Index in feeder
                if (!feedersItemsFeederIndex.ContainsKey(indexString))
                    feedersItemsFeederIndex.Add(indexString, i);
                else if (feedersItemsFeederIndex.ContainsKey(indexString) && i != feedersItemsFeederIndex[indexString])
                    feedersItemsFeederIndex[indexString] = i;
            }
        }
    }

    private void LoadFeedersContent()
    {
        foreach (string animalPenIndexString in feedersItemsFeederIndex.Keys)
        {            
            int feederItemIndex = feedersItems[animalPenIndexString];
            int feederItemQuantity = feedersItemsQuantities[animalPenIndexString];

            int animalPenIndex = int.Parse(animalPenIndexString.Substring(0, 1));

            if (feederItemIndex != -1 && feederItemQuantity > 0)
            {
                Feeder currentFeeder = animalPenList[animalPenIndex].currentFeeder.GetComponent<Feeder>();

                if (currentFeeder == null) return;
                
                ScriptableObject scriptObject = listSlots.GetItemByIndex(feederItemIndex);

                if (scriptObject.GetType() == typeof(Item))
                {
                    int feederItemFeederIndex = feedersItemsFeederIndex[animalPenIndexString];

                    currentFeeder.Items[feederItemFeederIndex] = (Item)scriptObject;
                    currentFeeder.ItemsQuantities[feederItemFeederIndex] = feederItemQuantity;
                }
            }
        }
    }

    #endregion

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
            Feeder oldFeeder = currentState.animalPenObject.GetComponent<AnimalPenRef>().Feeder.GetComponent<Feeder>();

            animalPen.currentAnimalPenState = currentState.animalPenObject;

            animalPen.currentFeeder = currentState.animalPenObject.GetComponent<AnimalPenRef>().Feeder;

            animalPen.currentFeeder.GetComponent<Feeder>().TransferItems(oldFeeder);

            animalPen.currentBell = currentState.animalPenObject.GetComponent<AnimalPenRef>().Bell;

            animalPen.currentPanel = currentState.animalPenObject.GetComponent<AnimalPenRef>().Panel;

            animalPen.currentPanel.GetComponentInChildren<Renderer>().sharedMaterial = animalPen.animalPanelMat;

            animalPen.currentPanel.GetComponentInChildren<Text>().text = $"Niveau {animalPen.animalPenLevel}";

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