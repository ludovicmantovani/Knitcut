using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attirer_Animal : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject[] Fruit;
    public bool FruitPoser = false;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Player" && Input.GetKey(KeyCode.E) && FruitPoser == false)
        {
            (Instantiate(Fruit[0]) as GameObject).transform.parent = this.transform;
            FruitPoser = true;
        }
    }
}
