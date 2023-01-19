using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseOverColor : MonoBehaviour
{
    public void SetRed() {GetComponent<Image>().color = Color.red;}
    public void SetGreen() {GetComponent<Image>().color = Color.green;}
}
