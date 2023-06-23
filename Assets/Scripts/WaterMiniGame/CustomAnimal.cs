using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class CustomAnimal : MonoBehaviour
{
    [System.Serializable]
    public class SpriteAnimalPair
    {
        public AnimalType animalType;
        public Sprite sprite;
        public string displayName;
        public bool male;
    }

    [SerializeField] private SpriteRenderer spriteRenderer = null;
    [SerializeField] private RawImage rawImage = null;
    [SerializeField] private ResultCanvas resultCanvas = null;
    [SerializeField] private AnimalType defaultAnimalType = AnimalType.Lama;
    [SerializeField] private List<SpriteAnimalPair> spriteStringPairs = new List<SpriteAnimalPair>();

    private string _currentAnimalName = null;
    private Sprite _currentAnimalSprite = null;
    private AnimalType _currentAnimalType = AnimalType.None;
    private bool _isMale = true;

    void Start()
    {
        findCurrentAnimal();
        if (spriteStringPairs.Count > 0)
        {
            if (spriteRenderer && _currentAnimalSprite != null)
                spriteRenderer.sprite = _currentAnimalSprite;
            if (rawImage && _currentAnimalSprite != null)
                rawImage.texture = _currentAnimalSprite.texture;
            if (resultCanvas && _currentAnimalName != null && _currentAnimalName.Length > 0)
                resultCanvas.SetAnimal(_currentAnimalType, _currentAnimalName, _isMale);
        }
    }

    private void findCurrentAnimal()
    {
        AnimalType animalType = defaultAnimalType;
        List<object> dataToKeep = GameManager.DataToKeep;

        if (dataToKeep != null && dataToKeep.Count > 1)
        {
            Debug.Log($"nom : {dataToKeep[0]} => type : {dataToKeep[1]}");
            animalType = (AnimalType)dataToKeep[1];
        }

        if (animalType != AnimalType.None)
        {
            SpriteAnimalPair tempSpriteAnimalPair = null;
            for (int i = 0; i < spriteStringPairs.Count; i++)
            {
                tempSpriteAnimalPair = spriteStringPairs[i];
                if (tempSpriteAnimalPair.animalType != AnimalType.None &&
                    tempSpriteAnimalPair.animalType == animalType)
                {
                    if (tempSpriteAnimalPair.displayName.Length > 0)
                        _currentAnimalName = tempSpriteAnimalPair.displayName;
                    if (tempSpriteAnimalPair.sprite != null)
                        _currentAnimalSprite = tempSpriteAnimalPair.sprite;
                    _isMale = tempSpriteAnimalPair.male;
                    _currentAnimalType = animalType;
                    break;
                }
            }
        }
    }
}
