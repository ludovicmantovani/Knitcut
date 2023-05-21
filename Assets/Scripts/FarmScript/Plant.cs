using UnityEngine;

[CreateAssetMenu(fileName = "Plant", menuName = "ScriptableObject/Plant")]
[System.Serializable]
public class Plant : ScriptableObject
{
    public GameObject seed;
    public GameObject sprout;
    public GameObject plant;
    public GameObject readyPlant;
    public GameObject fruit;
    public GameObject fruitSliced;
}