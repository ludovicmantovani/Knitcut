using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VoitureFarm : MonoBehaviour
{
    [SerializeField] private GameObject InteractionUI;
    private bool RetourVillage = false;
    // Start is called before the first frame update
    private void Awake()
    {
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
        if (other.tag == "Player" && Input.GetKey(KeyCode.E) && RetourVillage == false)
        {
            Debug.Log("Retour a la ferme");
            RetourVillage = true;
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
