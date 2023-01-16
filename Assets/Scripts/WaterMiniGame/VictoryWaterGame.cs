using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryWaterGame : MonoBehaviour
{
    public bool win = false;
    //[SerializeField] private GameObject WinGame;
    
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            win = true;
            Debug.Log("You Win");
            //WinGame.SetActive(true);
        }
    }
}
