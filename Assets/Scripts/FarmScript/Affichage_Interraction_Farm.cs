using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class Affichage_Interraction_Farm : MonoBehaviour
{
    [SerializeField] private PlayerInput pI;
    [SerializeField] private GameObject interactionPanel;
    private bool ontrigger = false;
    // Start is called before the first frame update
    void Start()
    {
        pI = GetComponent<PlayerInput>();
        interactionPanel.GetComponentInChildren<Text>().text = "Use " + pI.actions["Intercation_Environnements"].GetBindingDisplayString() + " to use";
    }

    // Update is called once per frame
    void Update()
    {
        OnTriggerVerif();
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            ontrigger = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            ontrigger = false;
        }
    }
    void OnTriggerVerif()
    {
        if (ontrigger == true)
        {
            interactionPanel.SetActive(true);
        }
        else if (ontrigger == false)
        {
            interactionPanel.SetActive(false);
        }
    }
}
