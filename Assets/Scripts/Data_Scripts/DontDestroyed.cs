using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyed : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < Object.FindObjectsOfType<DontDestroyed>().Length; i++)
        {
            if (Object.FindObjectsOfType<DontDestroyed>()[i] != this)
            {
                if (Object.FindObjectsOfType<DontDestroyed>()[i].name == gameObject.name)
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
