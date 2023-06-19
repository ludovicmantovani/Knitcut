using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AnimalStates : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Feeder feeder;
    [SerializeField] private GameObject animalCanvas;
    [SerializeField] private Text animalNameText;
    [SerializeField] private Slider animalHungerSlider;
    [SerializeField] private Image animalHappinessImage;
    [SerializeField] private GameObject woolPrefab;
    [SerializeField] private float timeWoolProduction = 20f;
    [SerializeField] private float timeAutoDestroy = 10f;

    [Header("Animal Data")]
    [SerializeField] private string animalName;
    [SerializeField] private string animalID;
    [SerializeField] private bool isChild;

    private bool canProduceWool = false;
    private bool producingWool = false;

    [Header("Hunger parameters")]
    [SerializeField] private float hunger = 0;
    [SerializeField] private float maxHunger = 300;
    [SerializeField] private float hungerToDecrease = 40;
    [SerializeField] private float timeDecreaseHunger = 60;

    private bool decreasing;
    private bool feeding;

    [Header("Happiness parameters")]
    [SerializeField] private int happiness;
    [SerializeField] private float requiredHungerForHappiness = 0.6f;
    [SerializeField] private Color happyColor;
    [SerializeField] private Color sadColor;

    #region Getters / Setters

    public string AnimalName
    {
        get { return animalName; }
        set { animalName = value; }
    }

    public string AnimalID
    {
        get { return animalID; }
        set { animalID = value; }
    }

    public bool IsChild
    {
        get { return isChild; }
        set { isChild = value; }
    }

    public float Hunger
    {
        get { return hunger; }
        set { hunger = value; }
    }

    public Feeder CurrentFeeder
    {
        get { return feeder; }
        set { feeder = value; }
    }

    #endregion

    private void Awake()
    {
        if (!isChild) InitializeHunger();
    }

    private void Update()
    {
        animalCanvas.transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
        
        if (!isChild)
        {
            animalNameText.text = $"{animalName}";

            HandleHunger();

            HandleHappiness();

            HandleWoolProduction();
        }
        else
            animalNameText.text = $"{animalName} (B)";
    }

    #region Wool

    private void HandleWoolProduction()
    {
        if (!producingWool)
        {
            producingWool = true;

            StartCoroutine(ProducingWool());
        }
    }

    private IEnumerator ProducingWool()
    {
        while (canProduceWool)
        {
            if (!canProduceWool) break;

            yield return new WaitForSeconds(timeWoolProduction);

            if (!canProduceWool) break;

            GameObject wool = Instantiate(woolPrefab, transform.position, Quaternion.identity);

            Destroy(wool, timeAutoDestroy);
        }

        producingWool = false;
    }

    #endregion

    #region Hunger

    private void InitializeHunger()
    {
        hunger = maxHunger;

        decreasing = false;
        feeding = false;

        animalHungerSlider.maxValue = maxHunger;
        animalHungerSlider.value = hunger;
    }

    private void HandleHunger()
    {
        if (!decreasing && hunger > 0)
            StartCoroutine(DecreaseHungerOverTime());

        if (hunger < maxHunger)
            SearchFood();

        animalHungerSlider.value = hunger;
    }

    private void SearchFood()
    {
        if (feeder == null) return;

        Item food = feeder.GetFood();

        if (!feeding && food != null && maxHunger - hunger >= food.itemValue)
        {
            feeding = true;

            Feed(food.itemValue);

            feeder.RemoveItem(food);
        }
    }

    private void Feed(float hungerToFeed)
    {
        Debug.Log($"Feed {hungerToFeed}");
        hunger += (int)hungerToFeed;

        if (hunger > maxHunger) hunger = maxHunger;

        feeding = false;
    }

    private IEnumerator DecreaseHungerOverTime()
    {
        decreasing = true;

        yield return new WaitForSeconds(timeDecreaseHunger);

        hunger -= hungerToDecrease;

        if (hunger < 0) hunger = 0;

        decreasing = false;
    }

    #endregion

    #region Happiness

    private void HandleHappiness()
    {
        float percentageHunger = hunger / maxHunger;

        if (percentageHunger >= requiredHungerForHappiness)
        {
            happiness = 1;
            animalHappinessImage.color = happyColor;
            canProduceWool = true;
        }
        else
        {
            happiness = 0;
            animalHappinessImage.color = sadColor;
            canProduceWool = false;
        }

        //CheckBreeding();
    }

    private void CheckBreeding()
    {
        if (happiness == 1)
        {
            Debug.Log($"happiness enough -> can breed");
        }
        else
        {
            Debug.Log($"happiness not enough -> can not breed");
        }
    }

    #endregion
}