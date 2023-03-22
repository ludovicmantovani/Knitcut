using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Pousse : MonoBehaviour
{
    public PlayerInput playerInput;
    public float tempsPousse = 10f;
    public bool croissance = true;
    public GameObject planteAdulte;
    public GameObject planteSoif;
    public GameObject planteMalade;
    public GameObject Graine;
    public GameObject poussePlante;
    public bool affection = false;
    public Planter planter;
    public int nbrandom;
    public GameObject GraineX;
    // Start is called before the first frame update
    void Start()
    {
        playerInput = FindObjectOfType<PlayerInput>();

        planter = GetComponentInParent<Planter>();
        GraineX = this.gameObject;
        transform.localPosition = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        CroissanceDeLaPlante();
    }
    void CroissanceDeLaPlante()
    {
        if(croissance == true)
        {
            tempsPousse = tempsPousse - Time.deltaTime;
            if (tempsPousse <= 5 && affection == false)
            {
                RandomAfectionPousse();
            }
            if(tempsPousse <= 0)
            {
                croissance = false;
                tempsPousse = 0;
                Graine.SetActive(false);
                poussePlante.SetActive(false);
                planteSoif.SetActive(false);
                planteMalade.SetActive(false);
                planteAdulte.SetActive(true);
                
            }
        }
    }
    void RandomAfectionPousse()
    {
        
        nbrandom = Random.Range(0,3);
        if(nbrandom == 0)
        {
            croissance = true;
            affection = true;
            Graine.SetActive(false);
            poussePlante.SetActive(true);
        }
        if (nbrandom == 1)
        {
            croissance = false;
            affection = true;
            Graine.SetActive(false);
            planteSoif.SetActive(true);
            
        }
        if (nbrandom == 2)
        {
            croissance = false;
            affection = true;
            Graine.SetActive(false);
            planteMalade.SetActive(true);
            
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player_Farm" && nbrandom == 1 && playerInput.HydrateAction.triggered)
        {
            poussePlante.SetActive(true);
            planteSoif.SetActive(false);
            croissance = true;
        }
        if (other.tag == "Player_Farm" && nbrandom == 2 && playerInput.HealAction.triggered)
        {
            poussePlante.SetActive(true);
            planteMalade.SetActive(false);
            croissance = true;
        }
        
    }
}
