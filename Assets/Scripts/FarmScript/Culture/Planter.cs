using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Planter : MonoBehaviour
{
    public PlayerInput pI;
    public GameObject infoZone;
    public GameObject graines;
    public GameObject[] PrefabsGraine;
    public bool Vide = false;
    
    // Start is called before the first frame update
    void Start()
    {

        pI = GetComponent<PlayerInput>();
    }

    // Update is called once per frame
    void Update()
    {
       
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player_Farm")
        {
            Debug.Log("touche");
            infoZone.SetActive(true);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Player_Farm" && pI.actions["Intercation_Environnements"].triggered && Vide == false)
        {
            graines.SetActive(true);

            /*(Instantiate(PrefabsGraine[0]) as GameObject).transform.parent = this.transform;
            Vide = true;
            
        }
        if (other.tag == "Player" && Input.GetKey(KeyCode.R) && Vide == false)
        {
            (Instantiate(PrefabsGraine[1]) as GameObject).transform.parent = this.transform;
            Vide = true;
            
        }
        if (other.tag == "Player" && Input.GetKey(KeyCode.T) && Vide == false)
        {
            (Instantiate(PrefabsGraine[2]) as GameObject).transform.parent = this.transform;
            Vide = true;*/
            
        }
    }
    public void Graine1()
    {
        (Instantiate(PrefabsGraine[0]) as GameObject).transform.parent = this.transform;
        Vide = true;
    }
    public void Graine2()
    {
        (Instantiate(PrefabsGraine[1]) as GameObject).transform.parent = this.transform;
        Vide = true;
    }
    public void Graine3()
    {
        (Instantiate(PrefabsGraine[2]) as GameObject).transform.parent = this.transform;
        Vide = true;
    }
    public void Cancel()
    {
        graines.SetActive(false);
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player_Farm")
        {
            infoZone.SetActive(false);
        }
    }
}
