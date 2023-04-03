using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ErrorIndicator : MonoBehaviour
{
    [SerializeField] private RawImage rawImage = null;

    public void DisplayErrorCount(int nbr = 1)
    {
        for (int i = 0; i < transform.childCount; i++)
            Destroy(transform.GetChild(i).gameObject);
        
        if (rawImage && nbr >= 0)
        {
            for (int i = 0; i < nbr; i++)
                Instantiate(rawImage, transform);
        }
    }
}
