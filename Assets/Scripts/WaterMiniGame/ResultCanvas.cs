using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResultCanvas : MonoBehaviour
{
    [SerializeField] TMP_Text textInfo;
    [SerializeField] TMP_Text textButton;

    [SerializeField] private List<GameObject> desactiveGameObject;

    public void SetData(bool win = false, string animal = "lama")
    {
        string info = win ? "Vous avez attrapé un " + animal + " !": "Un " + animal + " vous a échappé !";
        if (textInfo) textInfo.text = info;

        info = win ? "Choisir un enclos" : "Retour à la ferme";
        if (textButton) textButton.text = info;
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
