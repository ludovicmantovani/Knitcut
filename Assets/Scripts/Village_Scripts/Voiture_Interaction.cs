using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class Voiture_Interaction : MonoBehaviour
{
    public PlayerInput pI;
    [SerializeField] private GameObject InteractionUI;
    private playerController PC;
    private bool RetourFerme = false;
    private Shop_Enclos se;
    // Start is called before the first frame update
    private void Awake()
    {
        pI = GetComponent<PlayerInput>();
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
        if (other.tag == "Player" && pI.actions["Intercation_Environnements"].triggered && RetourFerme == false)
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
