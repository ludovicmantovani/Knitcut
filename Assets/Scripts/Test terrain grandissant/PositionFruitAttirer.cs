using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PositionFruitAttirer : MonoBehaviour
{
    public float seFaitManger = 90f;
    private Attirer_Animal attirer_Animal;
    // Start is called before the first frame update
    void Start()
    {
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
        
        if(other.gameObject.tag == "Animal" && Input.GetKey(KeyCode.E))
        {
            Debug.Log("Contact");
            SceneManager.LoadScene(3);
        }
        
    }
}
