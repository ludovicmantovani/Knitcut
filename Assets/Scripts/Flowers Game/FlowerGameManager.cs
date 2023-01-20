using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FlowerGameManager : MonoBehaviour
{
    public enum State { BEFORE_GAME, AFTER_GAME, PLAY, SHOW}
    [SerializeField] private FlowerCreation flowerCreationScript;

    public State gameState = State.BEFORE_GAME;

    private int _rightPetalIndex = 0;
    private int _nbPetals = 0;

    void Start()
    {
        if (flowerCreationScript)
        {
            flowerCreationScript.SetGameManager(this);
            _nbPetals = flowerCreationScript.MakeFlower();
            foreach (Transform item in flowerCreationScript.GetRandomPetals())
            {
                item.gameObject.GetComponent<PetalInput>().OnChange += HandleChange;
            }
            Debug.Log(_nbPetals);
            gameState = State.SHOW;
            //Debug.Log(flowerCreationScript._randomPetals.Count);
            flowerCreationScript.ShowSequence(1f, 1);
        }
    }

    void Update()
    {
        if (gameState == State.PLAY)
        {
            //Debug.Log("PLAY");
            //gameState = State.AFTER_GAME;
        }
    }

    private void HandleChange(string name)
    {
        Debug.Log(name);
        string rightPetalName = _rightPetalIndex.ToString();
        if (name == rightPetalName)
        {
            if (_rightPetalIndex >= _nbPetals)
            {
                Debug.Log("WIN !");
            }
            else
            {
                _rightPetalIndex++;
                flowerCreationScript.ShowSequence(1f, _rightPetalIndex);
            }
        }
        else
        {
            Debug.Log("LOOSE !");
            gameState = State.AFTER_GAME;
        }
    }

}
