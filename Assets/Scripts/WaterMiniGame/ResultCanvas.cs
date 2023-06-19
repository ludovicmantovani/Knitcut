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
    private AnimalType _animalType = AnimalType.None;

    public void SetAnimal(AnimalType aimalType, string name, bool male = true)
    {
        _animalType = aimalType;
        if (name.Length > 0) _animalName = name;
        _isMale = male;
    }

    public void SetData(bool win = false)
    {
        string sexWord = _isMale ? "un" : "une";
        string winString = "Vous avez attrap� " + sexWord + " " + _animalName + " !";
        string loseString = char.ToUpper(sexWord[0]) + sexWord.Substring(1) + " " + _animalName + " vous a �chapp� !";
        string info = win ? winString : loseString;
        if (textInfo) textInfo.text = info;

        info = "Retour � la ferme";
        if (textButton) textButton.text = info;

        GameManager.AnimalTypeToKeep = win ? _animalType : AnimalType.None;
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
