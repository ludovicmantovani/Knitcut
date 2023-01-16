using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResultCanvas : MonoBehaviour
{

    [SerializeField] TMP_Text textInfo;

    private bool _winGame = false;

    public void SetData(bool win = false, string animal = "animal")
    {
        string info = win ? "Vous avez attrapé un" + animal + "!": "Un " + animal + " vous a échappé !";
        if (textInfo) textInfo.SetText(info);
    }
}
