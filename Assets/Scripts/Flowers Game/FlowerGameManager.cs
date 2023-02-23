using System.Collections.Generic;
using UnityEngine;


public class FlowerGameManager : MonoBehaviour
{
    public enum State
    {
        BEFORE_GAME, IN_GAME, AFTER_GAME, FINISHED
    }

    [SerializeField] private GameObject gameCanvas;
    [SerializeField] private GameObject resultCanvas;
    [SerializeField] private FlowerCreation flowerCreationScript;
    [SerializeField] private float waitingChangeStateTime = 3f;
    [SerializeField] private RelationCanvas relationCanvas;

    private int _nbPetals = 0;
    private int _turn = 1;
    private Queue<int> _sequence;
    private State gameState = State.BEFORE_GAME;
    private float _stateTime = 0f;
    private bool _win = false;
    private int _nbCanvasText = -1;

    #region UNITY_METHOD
    void Start()
    {
        if (relationCanvas)
            _nbCanvasText = relationCanvas.GetTextCount();
        if (flowerCreationScript)
        {
            _nbPetals = flowerCreationScript.MakeFlower();
            foreach (Transform item in flowerCreationScript.GetRandomPetals())
                item.gameObject.GetComponent<PetalInput>().OnChange += HandleChange;
            _sequence = new Queue<int>();
            for (int i = 0; i < _turn; i++) _sequence.Enqueue(i);
            flowerCreationScript.ShowSequence(1f, _turn);
            gameState = State.IN_GAME;
        }
    }

    void Update()
    {
        if ((gameState == State.AFTER_GAME)
            && Time.time >= _stateTime + waitingChangeStateTime)
        {
            if (gameCanvas) gameCanvas.SetActive(false);
            if (resultCanvas)
            {
                resultCanvas.GetComponent<FlowerResultCanvas>().SetVictory(_win);
                resultCanvas.SetActive(true);
            };
            gameState = State.FINISHED;
        }
    }
    #endregion

    #region SPECIFIC_METHOD
    private void HandleChange(string name)
    {
        string rightPetalName = _sequence.Dequeue().ToString();
        if (name == rightPetalName)
        {
            if (_turn == _nbPetals && _sequence.Count == 0)
            {
                flowerCreationScript.FallPetals();
                _stateTime = Time.time;
                _win = true;
                gameState = State.AFTER_GAME;
            }
            else if (_sequence.Count == 0)
            {
                _turn++;
                _sequence.Clear();
                for (int i = 0; i < _turn; i++) _sequence.Enqueue(i);
                flowerCreationScript.ShowSequence(1f, _turn);
            }
        }
        else
        {
            flowerCreationScript.FallPetals();
            _stateTime = Time.time;
            gameState = State.AFTER_GAME;
        }
    }
    #endregion
}
