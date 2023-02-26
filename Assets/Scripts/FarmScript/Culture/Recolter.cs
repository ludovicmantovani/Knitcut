using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Recolter : MonoBehaviour
{
    public PlayerInput pI;
    Planter planter;
    Pousse pousse;
    // Start is called before the first frame update
    void Start()
    {
        planter = GetComponentInParent<Planter>();
        pousse = GetComponentInParent<Pousse>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player_Farm" && pI.actions["Intercation_Environnements"].triggered)
        {
            planter.Vide = false;
            Destroy(pousse.GraineX);
        }
    }
}
