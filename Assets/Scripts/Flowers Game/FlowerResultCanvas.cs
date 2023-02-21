using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FlowerResultCanvas : MonoBehaviour
{
    [SerializeField] private TMP_Text textInfo = null;
    public void SetVictory(bool win)
    {
        if (textInfo)
            textInfo.text = win == true ? "C'est gagné" : "C'est perdu";
    }
}
