using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AnimalSpawnPos : MonoBehaviour
{
    [SerializeField] private float LifeTimer = 20f;
    [SerializeField] private GameObject[] points;
    public GameObject fruit_position;
    private int destPoint = 0;
    public NavMeshAgent navMeshAgent;
    private Attirer_Animal attirer_Animal;
    void Start()
    {
        attirer_Animal = FindObjectOfType<Attirer_Animal>();
        points = GameObject.FindGameObjectsWithTag("Destpoint");
        fruit_position = GameObject.FindGameObjectWithTag("FP");
        this.transform.localPosition = new Vector3(0, 0, 0);
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.autoBraking = false;
        GotoNextPoint();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance < 0.5f)
        {
            GotoNextPoint();
        }
        LifeTimer = LifeTimer - Time.deltaTime;

        if (LifeTimer <= 0)
        {
            Destroy(this.gameObject);
        }
        changePos();
    }
    void GotoNextPoint()
    {
        if (points.Length == 0) return;

        navMeshAgent.destination = points[destPoint].transform.position;

        destPoint = Random.Range(0, points.Length);
    }
    
    void changePos()
    {
        if(attirer_Animal.FruitPoser == true)
        {
            navMeshAgent.destination = fruit_position.transform.position;
        }
    }
}
