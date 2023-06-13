using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class FlowerGameManager : MonoBehaviour
{
    public enum State
    {
        BEFORE_GAME, IN_GAME, AFTER_GAME, FINISHED
    }

    [SerializeField] private GameObject gameCanvas;
    [SerializeField] private GameObject resultCanvas;
    [SerializeField] private GameObject tutorialCanvas;
    [SerializeField] private FlowerCreation flowerCreationScript;
    [SerializeField] private float displaySequenceSpeed = 0.5f;
    [SerializeField] private float waitingChangeStateTime = 3f;
    [SerializeField] private RelationCanvas relationCanvas;
    [SerializeField] private int nbrErrorAllowed = 3;
    [SerializeField] private ErrorIndicator errorIndicator = null;

    private VideoPlayer videoPlayer;
    private bool initialize = false;

    private int _turn = 1;
    private Queue<int> _sequence;
    private State gameState = State.BEFORE_GAME;
    private float _stateTime = 0f;
    private bool _win = false;
    private int _nbCanvasText = -1;
    private int _nbToNextStep = -1;
    private int _nbCurrentStep = 0;
    private int _coutnError = 0;
    private string _rightPetalName = "";

    #region UNITY_METHOD
    void Start()
    {
        PlayTutorial();
    }

    private void InitializeFlower()
    {
        if (flowerCreationScript)
        {
            initialize = true;

            if (relationCanvas)
            {
                _nbCanvasText = relationCanvas.GetTextCount();
                _nbToNextStep = 1;
            }
            flowerCreationScript.MakeFlower(_nbCanvasText);
            foreach (Transform item in flowerCreationScript.GetRandomPetals())
                item.gameObject.GetComponent<PetalInput>().OnChange += HandleChange;
            _sequence = new Queue<int>();
            for (int i = 0; i < _turn; i++) _sequence.Enqueue(i);
            flowerCreationScript.ShowSequence(displaySequenceSpeed, _turn);
            if (errorIndicator != null) errorIndicator.DisplayErrorCount(nbrErrorAllowed);
            //gameState = State.IN_GAME;
        }
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

        if (!initialize) InitializeFlower();
    }

    #endregion

    void Update()
    {
        if ((gameState == State.AFTER_GAME)
            && Time.time >= _stateTime + waitingChangeStateTime)
        {
            if (gameCanvas) gameCanvas.SetActive(false);
            if (resultCanvas)
            {
                int nbChildren = _turn - 1;
                MinigameManager.FinalizeMG(MinigameManager.MGType.Breeding, nbChildren);

                resultCanvas.GetComponent<FlowerResultCanvas>().SetVictory(_win, nbChildren);
                resultCanvas.SetActive(true);
            };
            gameState = State.FINISHED;
        }
    }
    #endregion

    #region SPECIFIC_METHOD
    private void HandleChange(string name)
    {
        _rightPetalName = _sequence.Dequeue().ToString();
        if (name == _rightPetalName)
        {
            if (_turn == relationCanvas.GetTextCount() - 1 && _sequence.Count == 0)
            {
                relationCanvas.Next();
                flowerCreationScript.CelebrateLove();
                _stateTime = Time.time;
                _win = true;
                gameState = State.AFTER_GAME;
            }
            else if (_sequence.Count == 0)
            {
                _turn++;
                _sequence.Clear();
                for (int i = 0; i < _turn; i++) _sequence.Enqueue(i);

                _nbCurrentStep++;
                if (_nbCurrentStep == _nbToNextStep)
                {
                    if (relationCanvas)
                        relationCanvas.Next();
                    _nbCurrentStep = 0;
                }

                flowerCreationScript.ShowSequence(displaySequenceSpeed, _turn);
            }
        }
        else
        {
            _turn = Mathf.Max(1, _turn - 1);
            _sequence.Clear();
            for (int i = 0; i < _turn; i++) _sequence.Enqueue(i);
            
            if (_coutnError >= nbrErrorAllowed)
            {
                if (_turn < 3)
                    flowerCreationScript.FallPetals();
                else
                {
                    flowerCreationScript.CelebrateLove();
                    _win = true;
                }
                _stateTime = Time.time;
                gameState = State.AFTER_GAME;
            }
            else
            {
                if (relationCanvas) relationCanvas.Previous();
                _coutnError++;
                if (errorIndicator != null)
                    errorIndicator.DisplayErrorCount(nbrErrorAllowed - _coutnError);
                flowerCreationScript.ShowSequence(displaySequenceSpeed, _turn);
            }
        }
    }
    
    public void Quit()
    {
        MinigameManager.SwitchScene();
    }
    #endregion
}
