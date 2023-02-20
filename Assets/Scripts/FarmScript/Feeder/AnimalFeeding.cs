using System.Collections;
using UnityEngine;

public class AnimalFeeding : MonoBehaviour
{
    [Header("Feeder References")]
    [SerializeField] private Feeder feeder;

    [Header("Hunger parameters")]
    [SerializeField] private int hunger = 0;
    [SerializeField] private int maxHunger = 300;
    [SerializeField] private int hungerToDecrease = 40;
    [SerializeField] private float timeDecreaseHunger = 60;

    private bool decreasing;
    private bool searchingFood;
    private bool feeding;

    void Start()
    {
        hunger = maxHunger;

        decreasing = false;
        searchingFood = false;
        feeding = false;
    }

    void Update()
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
}