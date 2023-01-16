using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResultCanvas : MonoBehaviour
{
    [SerializeField] TMP_Text textInfo;

    [SerializeField] private List<GameObject> desactiveGameObject;

    public void SetData(bool win = false, string animal = "animal")
    {
        string info = win ? "Vous avez attrapé un " + animal + "!": "Un " + animal + " vous a échappé !";
        if (textInfo) textInfo.SetText(info);
    }

    public void Display(bool hideObjects = true)
    {
        if (hideObjects)
        {
            foreach (GameObject go in desactiveGameObject)
            {
                if (go) go.SetActive(false);
            }
        }
        gameObject.SetActive(true);
    }
}
