using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetalInput : MonoBehaviour
{
    public delegate void OnChangeEvent(string value);
    public event OnChangeEvent OnChange;

    public void Change()
    {
        if (OnChange != null)
            OnChange(gameObject.name);
        gameObject.SetActive(false);
    }

}
