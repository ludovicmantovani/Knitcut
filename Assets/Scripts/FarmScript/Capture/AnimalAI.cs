using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class AnimalAI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AnimalType animalType;
    [SerializeField] private GameObject area;
    [SerializeField] private Item favoriteFruit;

    [Header("Datas")]
    [SerializeField] private float speed = 3.5f;
    [SerializeField] private float stoppingDistance = 0f;
    [SerializeField] private float refreshRate = 0.1f;
    [SerializeField] private float distanceMinToChange = 2f;
    [SerializeField] private float timeBeforeMoving = 3f;
    [SerializeField] private float timeAnimalLife = 20f;
    [SerializeField] private Vector3 destination;

    private GameObject currentFruitPlaced;
    private bool isMoving = false;
    private float distance;

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
    }

    private void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        animator.SetBool("Walking", isMoving);

        if (CaptureManager.instance.FruitPlaced != currentFruitPlaced)
            currentFruitPlaced = CaptureManager.instance.FruitPlaced;

        if (agent == null || isMoving) return;

        Item currentItemFruitPlaced = currentFruitPlaced.GetComponent<KeepItem>().Item;

        HandleDestination(currentItemFruitPlaced);

        StartCoroutine(GoToDestination(currentItemFruitPlaced));

        ActualizeDirection();
    }

    private void HandleDestination(Item itemFruitPlaced)
    {
        if (currentFruitPlaced == null || itemFruitPlaced != favoriteFruit)
            destination = SearchDestination();
        else if (currentFruitPlaced != null && itemFruitPlaced == favoriteFruit)
            destination = currentFruitPlaced.transform.position;
    }

    private IEnumerator GoToDestination(Item itemFruitPlaced)
    {
        isMoving = true;

        Debug.Log($"Go to destination");

        Item itemPlaced = CaptureManager.instance.FruitPlaced.GetComponent<KeepItem>().Item;

        while (distance > distanceMinToChange && itemPlaced == itemFruitPlaced)
        {
            agent.SetDestination(destination);

            yield return new WaitForSeconds(refreshRate);
        }

        isMoving = false;

        Debug.Log($"Arrived at destination");

        int random = Random.Range(0, 3);

        if (random == 0 | random == 1)
        {
            yield return new WaitForSeconds(timeBeforeMoving);
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

    private IEnumerator AnimalLife()
    {
        yield return new WaitForSeconds(timeAnimalLife);

        StopAllCoroutines();

        Destroy(gameObject);
    }
}