using System.Collections.Generic;
using UnityEngine;


public class FlowerGameManager : MonoBehaviour
{
    public enum State
    {
        BEFORE_GAME, WIN_GAME, LOSE_GAME, AFTER_GAME
    }

    [SerializeField] private GameObject gameCanvas;
    [SerializeField] private GameObject winCanvas;
    [SerializeField] private GameObject loseCanvas;
    [SerializeField] private FlowerCreation flowerCreationScript;
    [SerializeField] private float waitingChangeStateTime = 3f;

    private int _nbPetals = 0;
    private int _turn = 1;
    private Queue<int> _sequence;
    private State gameState = State.BEFORE_GAME;
    private float _stateTime = 0f;

    #region UNITY_METHOD
    void Start()
    {
        if (flowerCreationScript)
        {
            _nbPetals = flowerCreationScript.MakeFlower();
            foreach (Transform item in flowerCreationScript.GetRandomPetals())
                item.gameObject.GetComponent<PetalInput>().OnChange += HandleChange;
            _sequence = new Queue<int>();
            for (int i = 0; i < _turn; i++) _sequence.Enqueue(i);
            flowerCreationScript.ShowSequence(1f, _turn);
        }
    }

    void Update()
    {
        if ((gameState == State.WIN_GAME || gameState == State.LOSE_GAME)
            && Time.time >= _stateTime + waitingChangeStateTime)
        {
            if (gameCanvas) gameCanvas.SetActive(false);
            if (gameState == State.WIN_GAME && winCanvas) winCanvas.SetActive(true);
            if (gameState == State.LOSE_GAME && loseCanvas) loseCanvas.SetActive(true);
            gameState = State.AFTER_GAME;
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
                gameState = State.WIN_GAME;
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
            gameState = State.LOSE_GAME;
        }
    }
    #endregion
}
