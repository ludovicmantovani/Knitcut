using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetalInput : MonoBehaviour
{
    public delegate void OnChangeEvent(string value);
    public event OnChangeEvent OnChange;

    public void Change()
    {
        GetComponent<Animator>().SetTrigger("BalanceTrigger");
        if (OnChange != null)
        {
            OnChange(gameObject.name);
        }
    }

}
