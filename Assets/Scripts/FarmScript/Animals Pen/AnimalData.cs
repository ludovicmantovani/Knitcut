using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class AnimalData : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AnimalType animalType;
    [SerializeField] private GameObject currentAnimalPen;
    [SerializeField] private Transform body;

    [Header("Datas")]
    [SerializeField] private float speed = 3.5f;
    [SerializeField] private float stoppingDistance = 0f;
    [SerializeField] private float timeToWaitBeforeMoving = 3f;
    [SerializeField] private float distanceMinToChange = 4.2f;
    [SerializeField] private Vector3 destination;

    private bool animalCanMove = true;
    private float distance;
    private NavMeshAgent agent;

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

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        agent.speed = speed;
        agent.stoppingDistance = stoppingDistance;
    }

    private void Update()
    {
        if (animalCanMove) Move();

        Vector3 direction = transform.position - destination;
        distance = direction.magnitude;
    }

    private void Move()
    {
        animalCanMove = false;

        SearchDestination();

        if (distance > distanceMinToChange)
        {
            //body.LookAt(destination);
            agent.SetDestination(destination);
        }

        StartCoroutine(Moving());
    }

    private void SearchDestination()
    {
        float xLimit = currentAnimalPen.GetComponent<AnimalPenRef>().Surface.GetComponent<Renderer>().bounds.size.x;
        float zLimit = currentAnimalPen.GetComponent<AnimalPenRef>().Surface.GetComponent<Renderer>().bounds.size.z;

        float randomX = Random.Range(-xLimit / 2, xLimit / 2);
        float randomZ = Random.Range(-zLimit / 2, zLimit / 2);

        destination = currentAnimalPen.transform.position + new Vector3(randomX, transform.position.y, randomZ);
    }

    private IEnumerator Moving()
    {
        yield return new WaitForSeconds(timeToWaitBeforeMoving);

        SearchDestination();

        animalCanMove = true;
    }
}