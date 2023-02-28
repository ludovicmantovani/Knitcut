using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PositionFruitAttirer : MonoBehaviour
{
    public PlayerInput pI;
    public float seFaitManger = 90f;
    private Attirer_Animal attirer_Animal;
    private bool verif = false;
    // Start is called before the first frame update
    void Start()
    {
        pI = GetComponent<PlayerInput>();
        attirer_Animal = GetComponentInParent<Attirer_Animal>();
        this.transform.localPosition = new Vector3(0, 0.75f, 0);
    }

    // Update is called once per frame
    void Update()
    {
        seFaitManger = seFaitManger - Time.deltaTime;
        if(seFaitManger <= 0)
        {
            attirer_Animal.FruitPoser = false;
            Destroy(this.gameObject);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {

            attirer_Animal.interactionFruit.GetComponentInChildren<Text>().text = "Use " + pI.actions["Intercation_Environnements"].GetBindingDisplayString() + " to capture the animal";
            attirer_Animal.interactionFruit.SetActive(true);
            verif = true;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Animal")
        {

            Debug.Log("Contact");
            if (pI.actions["Intercation_Environnements"].triggered && verif ==true)
            {
                SceneManager.LoadScene("WaterMiniGameScene3");
            }

        }
        
        
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player")
        {
            attirer_Animal.interactionFruit.SetActive(false);
            verif = false;
        }
    }
}
