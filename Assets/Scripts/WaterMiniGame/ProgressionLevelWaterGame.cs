using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ProgressionLevelWaterGame : MonoBehaviour
{
    public NavMeshAgent NMA;
    public GameObject EndOfGameW;
    [SerializeField]private float tempsBeforeStoping = 0f;
    private bool startMoving = false;
    private bool Moving = false;
    private float tempsSecurity = 0f;
    private VictoryWaterGame victoryWaterGame;
    private bool Failed = false;
    public bool StopMoving = false;
    //[SerializeField] private GameObject Lost;
    [SerializeField] private ResultCanvas canvasResult;
    [SerializeField] private GameObject[] Pieces;
    private bool _running;

    void Start()
    {
        victoryWaterGame = FindObjectOfType<VictoryWaterGame>();
        _running = true;
    }

    void FixedUpdate()
    {

        if (_running) StopMiniGameWater();

    }
    void MovingWaterStart()
    {
        NMA.isStopped = false;
        //tempsBeforeMoving = tempsBeforeMoving + Time.deltaTime;
        if (Moving == true)
        {
            NMA.SetDestination(EndOfGameW.transform.position);
            //tempsBeforeMoving = 5f;
            startMoving = true;
            StopMoving = true;
            for (int i = 0; i < Pieces.Length; i++)
            {
                Pieces[i].layer = LayerMask.NameToLayer("Ignore Raycast");
            }
        }

    }
    void StopMovingVerification()
    {
        
        if (startMoving == true)
        {
            
            tempsSecurity = tempsSecurity + Time.deltaTime;
            if(tempsSecurity >= 3f)
            {
                tempsSecurity = 3f;
                if (NMA.velocity.x <= 0.1f && NMA.velocity.z <= 0.1f && NMA.velocity.x >= -0.1f && NMA.velocity.z >= -0.1f)
                {
                    
                    tempsBeforeStoping = tempsBeforeStoping - Time.deltaTime;
                    if(tempsBeforeStoping <= 0)
                    {
                        Debug.Log("Failed");
                        //Lost.SetActive(true);
                        Failed = true;
                    }

                }
                else
                {
                    tempsBeforeStoping = 2;
                }
                
            }
        }
        
    }
    void StopMiniGameWater()
    {
        if (victoryWaterGame.win == true || Failed == true)
        {
            NMA.isStopped = true;
            if (canvasResult && canvasResult.transform.gameObject)
            {
                canvasResult.SetData(victoryWaterGame.win);
                canvasResult.Display();
            }
            _running = false;
        }
        else
        {
            MovingWaterStart();
            StopMovingVerification();
        }
    }
    public void MovingWater()
    {
        Moving = true;
    }

    
       
}
