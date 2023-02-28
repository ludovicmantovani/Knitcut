using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_animal : MonoBehaviour
{
    [SerializeField] private float LifeTimer = 20f;
    private NavMeshAgent agent;
    public float radius;
    private Attirer_Animal attirer_Animal;
    public GameObject capturePosition;

    private void Start()
    {
        attirer_Animal = FindObjectOfType<Attirer_Animal>();
        agent = GetComponent<NavMeshAgent>();
    }
    private void Update()
    {
        LifeTimer -= Time.deltaTime;
        if (LifeTimer < 0 )
        {
            Destroy(this.gameObject);
        }
        if(!agent.hasPath || !attirer_Animal.FruitPoser)
        {
            agent.SetDestination(GetPoint.instance.GetRandomPoint());
        }
        else if (attirer_Animal.FruitPoser)
        {
            bool verif = true;
            if( verif)
            {
                capturePosition = GameObject.FindGameObjectWithTag("FP");
                verif = false;
            }
            agent.destination = capturePosition.transform.position;
        }
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
    
#endif
}
