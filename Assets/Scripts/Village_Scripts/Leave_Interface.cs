using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class Leave_Interface : MonoBehaviour
{
    [SerializeField] private PlayerInput pI;
    [SerializeField] private GameObject interactionPanel;

    void Start()
    {
        pI = FindObjectOfType<PlayerInput>();
        interactionPanel.GetComponentInChildren<TMP_Text>().text = "Use " + pI.InteractionAction.GetBindingDisplayString() + " to travel back to the Farm";
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Player")
        {
            interactionPanel.SetActive(true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            interactionPanel.SetActive(false);
        }
    }
}