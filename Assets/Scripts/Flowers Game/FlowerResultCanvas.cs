using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FlowerResultCanvas : MonoBehaviour
{
    [SerializeField] private TMP_Text textInfo = null;
    [SerializeField] private string textWin = "Vos animaux s'aiment assez pour shopEnclos reproduire.";
    [SerializeField] private string textLose = "Vos animaux ne s'aiment pas assez pour shopEnclos reproduire.";
    public void SetVictory(bool win, int nbrChildren)
    {
        if (textInfo)
            textInfo.text = win == true ? textWin : textLose;
    }
}