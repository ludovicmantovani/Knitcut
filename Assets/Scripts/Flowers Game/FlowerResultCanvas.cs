using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FlowerResultCanvas : MonoBehaviour
{
    [SerializeField] private TMP_Text textInfo = null;
    [SerializeField] private string textWin = "Vos animaux s'aiment assez pour se reproduire.";
    [SerializeField] private string textLose = "Vos animaux ne s'aiment pas assez pour se reproduire.";
    public void SetVictory(bool win)
    {
        if (textInfo)
            textInfo.text = win == true ? textWin : textLose;
    }
}