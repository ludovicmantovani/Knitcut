using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class Shop_Interaction : MonoBehaviour
{
    [SerializeField] private PlayerInput pI;
    [SerializeField] private GameObject interactionPanel;
    private bool ontrigger = false;
    // Start is called before the first frame update
    void Start()
    {
        pI = FindObjectOfType<PlayerInput>();
        interactionPanel.GetComponentInChildren<TMP_Text>().text = "Use " + pI.InteractionAction.GetBindingDisplayString() + " to trade";
    }

    // Update is called once per frame
    void Update()
    {
        OnTriggerVerifShop();
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
    void OnTriggerVerifShop()
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
