using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dont_Destroyed : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < Object.FindObjectsOfType<Dont_Destroyed>().Length; i++)
        {
            if(Object.FindObjectsOfType<Dont_Destroyed>()[i] != this)
            {
                if (Object.FindObjectsOfType<Dont_Destroyed>()[i].name == gameObject.name)
                {
                    Destroy(gameObject);
                }
            }
        }
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
    }
}
