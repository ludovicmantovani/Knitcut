using System.Collections;
using UnityEngine;

public class AnimalStates : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Feeder feeder;

    [Header("Hunger parameters")]
    [SerializeField] private float hunger = 0;
    [SerializeField] private float maxHunger = 300;
    [SerializeField] private float hungerToDecrease = 40;
    [SerializeField] private float timeDecreaseHunger = 60;

    private bool decreasing;
    private bool searchingFood;
    private bool feeding;

    [Header("Happiness parameters")]
    [SerializeField] private int happiness;
    [SerializeField] private float requiredHungerForHappiness = 0.6f;

    void Start()
    {
        InitializeHunger();
    }

    void Update()
    {
        HandleHunger();

        HandleHappiness();
    }

    #region Hunger

    private void InitializeHunger()
    {
        hunger = maxHunger;

        decreasing = false;
        searchingFood = false;
        feeding = false;
    }

    private void HandleHunger()
    {
        if (!decreasing && hunger > 0)
        {
            StartCoroutine(DecreaseHungerOverTime());
        }

        if (!searchingFood && hunger < maxHunger)
        {
            searchingFood = true;

            SearchFood();
        }
    }

    private void SearchFood()
    {
        Item food = feeder.GetFood();

        if (!feeding && food != null && maxHunger - hunger >= food.itemPrice)
        {
            feeding = true;

            Feed(food.itemPrice);

            feeder.RemoveItem(food);
        }
        else
        {
            searchingFood = false;
        }
    }

    private void Feed(float hungerToFeed)
    {
        hunger += (int)hungerToFeed;

        if (hunger > maxHunger) hunger = maxHunger;

        feeding = false;
        searchingFood = false;
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

        Debug.Log($"{percentageHunger} vs {requiredHungerForHappiness}");

        if (percentageHunger >= requiredHungerForHappiness)
        {
            happiness = 1;
        }
        else
        {
            happiness = 0;
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