using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Voiture_Interaction : MonoBehaviour
{
    [SerializeField] private GameObject InteractionUI;
    private bool RetourFerme = false;
    // Start is called before the first frame update
    private void Awake()
    {
        RetourFerme = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            InteractionUI.SetActive(true);
        }
        
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player" && Input.GetKey(KeyCode.E) && RetourFerme == false)
        {
            Debug.Log("Retour a la ferme");
            RetourFerme = true;
            SceneManager.LoadScene(1);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            InteractionUI.SetActive(false);
        }
    }
}
