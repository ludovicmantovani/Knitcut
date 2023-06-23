using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.Video;

public class ProgressionLevelWaterGame : MonoBehaviour
{
    public NavMeshAgent NMA;
    public GameObject EndOfGameW;
    [SerializeField]private float tempsBeforeStoping = 0f;
    [SerializeField]private float speedModifier = 2f;
    private bool startMoving = false;
    private bool Moving = false;
    private float tempsSecurity = 0f;
    private VictoryWaterGame victoryWaterGame;
    private bool Failed = false;
    public bool StopMoving = false;
    //[SerializeField] private GameObject Lost;
    [SerializeField] private Canvas tutorialCanvas;
    [SerializeField] private ResultCanvas canvasResult;
    [SerializeField] private GameObject[] Pieces;
    private bool _running;

    private VideoPlayer videoPlayer;
    private bool initialize = false;
    private float defaultSpeed = 0f;

    void Start()
    {
        PlayTutorial();
    }

    private void InitializeWaterGame()
    {
        initialize = true;

        victoryWaterGame = FindObjectOfType<VictoryWaterGame>();
        _running = true;

        defaultSpeed = NMA.speed;
    }

    #region Tutorial

    public void PlayTutorial()
    {
        tutorialCanvas.gameObject.SetActive(true);

        videoPlayer = tutorialCanvas.GetComponent<VideoPlayer>();

        videoPlayer.loopPointReached += StopTutorial;

        videoPlayer.Play();
    }

    public void SkipTutorial()
    {
        StopTutorial(videoPlayer);
    }

    public void StopTutorial(VideoPlayer videoPlayer)
    {
        videoPlayer.Stop();

        tutorialCanvas.gameObject.SetActive(false);

        if (!initialize) InitializeWaterGame();
    }

    #endregion

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

    public void SpeedModifier()
    {
        TMP_Text buttonText = EventSystem.current.currentSelectedGameObject.GetComponentInChildren<TMP_Text>();

        if (NMA.speed == defaultSpeed)
        {
            NMA.speed *= speedModifier;
            buttonText.text = $"Ralentir (x1)";
        }
        else if (NMA.speed > defaultSpeed)
        {
            NMA.speed /= speedModifier;
            buttonText.text = $"Accélerer (x2)";
        }
    }
}