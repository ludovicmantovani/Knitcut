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
    public class TextureStringPair
    {
        public string name;
        public Texture texture;
    }

    [SerializeField]
    private List<TextureStringPair> textureStringPairs = new List<TextureStringPair>();

    private string _currentAnimalName = null;
    private Texture _currentAnimalTexture = null;

    private SpriteRenderer spriteRenderer = null;
    private RawImage rawImage = null;
    void Start()
    {
        TryGetComponent<SpriteRenderer>(out spriteRenderer);
        TryGetComponent<RawImage>(out rawImage);
    }

    //private Texture GetAnimalTexture(string mane)
    //{
    //    if (textureTable.Count <= 0) return null;
    //    return textureTable[0].Item2;
    //}

    public string GetCurrentAnimalName()
    {
        if (_currentAnimalName != null || findCurrentAnimal()) return _currentAnimalName;
        return null;
    }

    public Texture GetCurrentAnimalTexture()
    {
        if (_currentAnimalTexture != null || findCurrentAnimal()) return _currentAnimalTexture;
        return null;
    }

    private bool findCurrentAnimal()
    {
        bool find = false;
        string animalNameFromFarm = MinigameManager.AnimalToKeep;

        if (animalNameFromFarm.Length > 0)
        {
            TextureStringPair tempTextureStringPair = null;
            for (int i = 0; i < textureStringPairs.Count; i++)
            {
                tempTextureStringPair = textureStringPairs[i];
                if (tempTextureStringPair.name.Length > 0 && animalNameFromFarm.Contains(tempTextureStringPair.name))
                {
                    _currentAnimalName = tempTextureStringPair.name;
                    if (tempTextureStringPair.texture != null) _currentAnimalTexture = tempTextureStringPair.texture;
                    break;
                }
            }
        }
        return find;
    }
    void OnGUI()
    {
        SerializedObject serializedObject = new SerializedObject(this);

        SerializedProperty textureStringPairsProperty = serializedObject.FindProperty("textureStringPairs");

        serializedObject.Update();

        EditorGUILayout.PropertyField(textureStringPairsProperty, true);

        serializedObject.ApplyModifiedProperties();
    }
}
