using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class AnimalAI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AnimalType animalType;
    [SerializeField] private GameObject area;
    [SerializeField] private Item favoriteFruit;
    [SerializeField] private GameObject animalCanvas;
    [SerializeField] private Image animalFruitImage;
    [SerializeField] private Text animalNameText;
    [SerializeField] private string animalName;

    [Header("Datas")]
    [SerializeField] private float speed = 3.5f;
    [SerializeField] private float stoppingDistance = 0f;
    [SerializeField] private float refreshRate = 0.1f;
    [SerializeField] private float distanceMinToChange = 2f;
    [SerializeField] private float timeBeforeMoving = 3f;
    [SerializeField] private float timeBeforeEatingFruit = 5f;
    [SerializeField] private Vector2 timeAnimalLife = new Vector2(15, 30);
    [SerializeField] private bool isMoving = false;
    [SerializeField] private bool nearFruit = false;
    [SerializeField] private Vector3 destination;

    private GameObject currentFruitPlaced;
    private float distance;
    private float timeRemaining = 0f;
    private bool timerStarted = false;

    private NavMeshAgent agent;
    private Animator animator;

    #region Getters / Setters

    public AnimalType AnimalType
    {
        get { return animalType; }
        set { animalType = value; }
    }

    public GameObject Area
    {
        get { return area; }
        set { area = value; }
    }

    public GameObject CurrentFruitPlaced
    {
        get { return currentFruitPlaced; }
        set { currentFruitPlaced = value; }
    }

    #endregion

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

        agent.speed = speed;
        agent.stoppingDistance = stoppingDistance;

        StartCoroutine(AnimalLife());

        animalNameText.text = animalName;
        animalFruitImage.sprite = favoriteFruit.itemSprite;
    }

    private void Update()
    {
        animalCanvas.transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);

        HandleMovement();
    }

    #region Movement

    private void HandleMovement()
    {
        animator.SetBool("Walking", isMoving);

        if (CaptureManager.instance.FruitPlaced != null)
            currentFruitPlaced = CaptureManager.instance.FruitPlaced;

        ActualizeDirection();

        HandleFruit();

        if (agent == null || isMoving) return;

        HandleDestination();

        StartCoroutine(GoToDestination());
    }

    #region Direction & Distance

    private void HandleDestination()
    {
        if (currentFruitPlaced == null)
            destination = SearchDestination();
        else
        {
            Item itemFruitPlaced = currentFruitPlaced.GetComponent<KeepItem>().Item;

            if (itemFruitPlaced == favoriteFruit)
                destination = currentFruitPlaced.transform.position;
            else
                destination = SearchDestination();
        }
    }

    private Vector3 SearchDestination()
    {
        if (area == null) return Vector3.zero;

        float xLimit = area.GetComponent<Renderer>().bounds.size.x;
        float zLimit = area.GetComponent<Renderer>().bounds.size.z;

        float randomX = Random.Range(-xLimit / 2, xLimit / 2);
        float randomZ = Random.Range(-zLimit / 2, zLimit / 2);

        Vector3 randomPosition = area.transform.position + new Vector3(randomX, 0f, randomZ);

        return randomPosition;
    }

    private void ActualizeDirection()
    {
        Vector3 direction = transform.position - destination;
        distance = direction.magnitude;
    }

    #endregion

    private IEnumerator GoToDestination()
    {
        isMoving = true;

        Vector3 currentFruitPosition = Vector3.zero;

        if (currentFruitPlaced != null)
            currentFruitPosition = currentFruitPlaced.transform.position;

        bool canContinue = true;

        while (distance > distanceMinToChange && canContinue)
        {
            if (!canContinue) yield break;

            Item itemPlaced = null;
            if (currentFruitPlaced != null) itemPlaced = currentFruitPlaced.GetComponent<KeepItem>().Item;

            if ((destination != currentFruitPosition && itemPlaced == favoriteFruit)
                || (destination == currentFruitPosition && (itemPlaced == null || itemPlaced != favoriteFruit)))
                canContinue = false;

            agent.SetDestination(destination);

            yield return new WaitForSeconds(refreshRate);
        }

        yield return new WaitForSeconds(TimeToWaitAtEnd());

        isMoving = false;
    }

    private float TimeToWaitAtEnd()
    {
        float timeToWait = timeBeforeMoving;

        int random = Random.Range(0, 3);

        if (random == 2) timeToWait = 0f;

        return timeToWait;
    }

    #endregion

    private void HandleFruit()
    {
        if (currentFruitPlaced == null) return;

        if (destination == currentFruitPlaced.transform.position && distance <= distanceMinToChange && !timerStarted)
        {
            nearFruit = true;
            timeRemaining = timeBeforeEatingFruit;
        }

        if (nearFruit)
        {
            if (timeRemaining > 0)
            {
                timerStarted = true;

                timeRemaining -= Time.deltaTime;
            }
            else
            {
                if (currentFruitPlaced != null)
                {
                    CaptureManager.instance.RemoveItem();
                    currentFruitPlaced = null;
                }

                nearFruit = false;

                timerStarted = false;
            }
        }
    }

    private IEnumerator AnimalLife()
    {
        float randomTime = Random.Range(timeAnimalLife.x, timeAnimalLife.y);

        yield return new WaitForSeconds(randomTime);

        StopAllCoroutines();

        Destroy(gameObject);
    }
}