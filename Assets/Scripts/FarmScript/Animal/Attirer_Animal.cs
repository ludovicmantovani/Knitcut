using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Attirer_Animal : MonoBehaviour
{
    // Start is called before the first frame update
    public PlayerInput pI;
    public GameObject[] Fruit;
    public bool FruitPoser = false;
    public GameObject interaction;
    public GameObject interactionFruit;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Player" && FruitPoser == false)
        {
            interaction.GetComponentInChildren<Text>().text = "Use " + pI.actions["Intercation_Environnements"].GetBindingDisplayString() + " to put a fruit";
            interaction.SetActive(true);
            if (pI.actions["Intercation_Environnements"].triggered)
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
