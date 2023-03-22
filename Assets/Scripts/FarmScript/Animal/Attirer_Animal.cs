using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Attirer_Animal : MonoBehaviour
{
    // Start is called before the first frame update
    public PlayerInput playerInput;
    public GameObject[] Fruit;
    public bool FruitPoser = false;
    public GameObject interaction;
    public GameObject interactionFruit;
    void Start()
    {
        playerInput = FindObjectOfType<PlayerInput>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Player" && FruitPoser == false)
        {
            interaction.GetComponentInChildren<Text>().text = "Use " + playerInput.InteractionAction.GetBindingDisplayString() + " to put a fruit";
            interaction.SetActive(true);
            if (playerInput.InteractionAction.triggered)
            {
                (Instantiate(Fruit[0]) as GameObject).transform.parent = this.transform;
                FruitPoser = true;
                interaction.SetActive(false);
            }
            
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player")
        {
            interaction.SetActive(false);
        }
    }
}
