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

    private string _animalName = "";
    private bool _isMale = true;

    public void SetAnimal(string name, bool male = true)
    {
        if (name.Length > 0) _animalName = name;
        _isMale = male;
    }

    public void SetData(bool win = false)
    {
        string sexWord = _isMale ? "un" : "une";
        string winString = "Vous avez attrapé " + sexWord + " " + _animalName + " !";
        string loseString = char.ToUpper(sexWord[0]) + sexWord.Substring(1) + " " + _animalName + " vous a échappé !";
        string info = win ? winString : loseString;
        if (textInfo) textInfo.text = info;

        info = "Retour à la ferme";
        if (textButton) textButton.text = info;

        MinigameManager.AnimalToKeep = win ? _animalName : "";
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
