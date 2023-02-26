using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Voiture_Interaction : MonoBehaviour
{
    [SerializeField] private GameObject InteractionUI;
    private playerController PC;
    private bool RetourFerme = false;
    private Shop_Enclos se;
    // Start is called before the first frame update
    private void Awake()
    {
        RetourFerme = false;
        se = FindObjectOfType<Shop_Enclos>();
        PC = FindObjectOfType<playerController>();
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
            se.SaveEncloslevel();
            PC.farm = false;

            FindObjectOfType<List_Slots>().AutoSavePlayerInventory();

            SceneManager.LoadScene(2);
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
