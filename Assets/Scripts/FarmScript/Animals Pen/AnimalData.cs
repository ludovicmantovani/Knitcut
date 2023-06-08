using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class AnimalData : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AnimalType animalType;
    [SerializeField] private GameObject currentAnimalPen;

    [Header("Datas")]
    [SerializeField] private float speed = 3.5f;
    [SerializeField] private float stoppingDistance = 0f;
    [SerializeField] private float refreshRate = 0.1f;
    [SerializeField] private float distanceMinToChange = 2f;
    [SerializeField] private float timeBeforeMoving = 3f;
    [SerializeField] private Vector3 destination;

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

    public GameObject CurrentAnimalPen
    {
        get { return currentAnimalPen; }
        set { currentAnimalPen = value; }
    }

    #endregion

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

        agent.speed = speed;
        agent.stoppingDistance = stoppingDistance;
    }

    private void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        animator.SetBool("Walking", isMoving);

        ActualizeDirection();

        if (agent == null || isMoving) return;

        SearchDestination();

        StartCoroutine(GoToDestination());
    }

    #region Direction & Distance

    private void SearchDestination()
    {
        if (currentAnimalPen == null) return;

        Transform surface = currentAnimalPen.GetComponent<AnimalPenRef>().Surface.transform;

        if (surface == null) return;

        float xLimit = surface.GetComponent<Renderer>().bounds.size.x;
        float zLimit = surface.GetComponent<Renderer>().bounds.size.z;

        float randomX = Random.Range(-xLimit / 2, xLimit / 2);
        float randomZ = Random.Range(-zLimit / 2, zLimit / 2);

        Vector3 randomPosition = surface.position + new Vector3(randomX, 0f, randomZ);

        destination = randomPosition;
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

        while (distance > distanceMinToChange)
        {
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
}