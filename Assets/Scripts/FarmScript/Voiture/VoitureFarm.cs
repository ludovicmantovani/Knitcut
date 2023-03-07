using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class VoitureFarm : MonoBehaviour
{
    [SerializeField] private GameObject InteractionUI;
    public PlayerInput pI;
    private bool RetourVillage = false;
    // Start is called before the first frame update
    private void Awake()
    {
        pI = GetComponent<PlayerInput>();
        RetourVillage = false;
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            InteractionUI.SetActive(true);
        }

    }
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player" && pI.actions["Intercation_Environnements"].triggered && RetourVillage == false)
        {
            Debug.Log("Retour a la ferme");
            RetourVillage = true;

            FindObjectOfType<playerController>().SavePlayerPos();

            FindObjectOfType<List_Slots>().SaveData();

            SceneManager.LoadScene(3);
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
