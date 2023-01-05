using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recolter : MonoBehaviour
{
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
        if (other.tag == "Player" && Input.GetKey(KeyCode.A))
        {
            planter.Vide = false;
            Destroy(pousse.GraineX);
        }
    }
}
