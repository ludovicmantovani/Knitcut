using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CookingResultCanvas : MonoBehaviour
{
    [SerializeField] private TMP_Text textPrice = null;
    [SerializeField] private TMP_Text textScore = null;
    [SerializeField] private TMP_Text textInfo = null;

    public void SetData(string finalPrice = "", string score = "", string info = "")
    {
        if (textPrice) textPrice.text = finalPrice;
        if (textScore) textScore.text = score;
        if (textInfo) textInfo.text = info;
    }
}
