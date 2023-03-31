using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class CustomAnimal : MonoBehaviour
{

    //[SerializeField] List<Tuple<string, Texture>> textureTable;

    //[SerializeField] private List<string> stringList = new List<string>();
    //[SerializeField] private List<Texture> texturesList = new List<Texture>();

    [System.Serializable]
    public class SpriteStringPair
    {
        public string findingName;
        public Sprite sprite;
        public string displayName;
        public bool male;
    }

    [SerializeField] private SpriteRenderer spriteRenderer = null;
    [SerializeField] private RawImage rawImage = null;
    [SerializeField] private ResultCanvas resultCanvas = null;
    [SerializeField] private List<SpriteStringPair> spriteStringPairs = new List<SpriteStringPair>();

    private string _currentAnimalName = null;
    private Sprite _currentAnimalSprite = null;
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
                resultCanvas.SetAnimal(_currentAnimalName, _isMale);
        }
    }

    private void findCurrentAnimal()
    {
        List<object> dataToKeep = MinigameManager.DataToKeep;
        string animalNameFromFarm = dataToKeep.Count > 0 ? (string)dataToKeep[0]: "";

        Debug.Log($"nom : {dataToKeep[0]} => type : {dataToKeep[1]}");

        if (animalNameFromFarm.Length > 0)
        {
            SpriteStringPair tempSpriteStringPair = null;
            for (int i = 0; i < spriteStringPairs.Count; i++)
            {
                tempSpriteStringPair = spriteStringPairs[i];
                if (tempSpriteStringPair.findingName.Length > 0 && animalNameFromFarm.Contains(tempSpriteStringPair.findingName))
                {
                    if (tempSpriteStringPair.displayName.Length > 0) _currentAnimalName = tempSpriteStringPair.displayName;
                    _isMale = tempSpriteStringPair.male;
                    if (tempSpriteStringPair.sprite != null) _currentAnimalSprite = tempSpriteStringPair.sprite;
                    break;
                }
            }
        }
    }
    //void OnGUI()
    //{
    //    SerializedObject serializedObject = new SerializedObject(this);

    //    SerializedProperty textureStringPairsProperty = serializedObject.FindProperty("spriteStringPairs");

    //    serializedObject.Update();

    //    EditorGUILayout.PropertyField(textureStringPairsProperty, true);

    //    serializedObject.ApplyModifiedProperties();
    //}
}
