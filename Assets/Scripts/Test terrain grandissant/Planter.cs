using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planter : MonoBehaviour
{
    public GameObject infoZone;
    public GameObject[] PrefabsGraine;
    public bool Vide = false;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("touche");
            infoZone.SetActive(true);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Player" && Input.GetKey(KeyCode.E) && Vide == false)
        {
            (Instantiate(PrefabsGraine[0]) as GameObject).transform.parent = this.transform;
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
            Vide = true;
            
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            infoZone.SetActive(false);
        }
    }
}
