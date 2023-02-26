using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PositionFruitAttirer : MonoBehaviour
{
    public PlayerInput pI;
    public float seFaitManger = 90f;
    private Attirer_Animal attirer_Animal;
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
    private void OnTriggerStay(Collider other)
    {
        
        if(other.gameObject.tag == "Animal" && pI.actions["Intercation_Environnements"].triggered)
        {
            Debug.Log("Contact");
            SceneManager.LoadScene(3);
        }
        
    }
}
