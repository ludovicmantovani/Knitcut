using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryWaterGame : MonoBehaviour
{
    public bool win = false;
    [SerializeField] private GameObject WinGame;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            win = true;
            Debug.Log("You Win");
            WinGame.SetActive(true);
        }
    }
}
