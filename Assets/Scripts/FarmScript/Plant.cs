using UnityEngine;

[CreateAssetMenu(fileName = "Plant", menuName = "ScriptableObject/Plant")]
[System.Serializable]
public class Plant : ScriptableObject
{
    [Header("Culture")]
    public GameObject seed;
    public GameObject sprout;
    public GameObject plant;
    public GameObject readyPlant;

    [Header("Cooking")]
    public GameObject fruit;
    public GameObject fruitSliced;
}