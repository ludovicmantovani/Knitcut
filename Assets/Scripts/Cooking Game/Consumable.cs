using UnityEngine;

[CreateAssetMenu(fileName = "Consumable", menuName = "ScriptableObject/Cooking/Consumable")]
public class Consumable : ScriptableObject
{
    public string consumableName;
    public int baseQuantity = 20;
    public int quantity;
    public GameObject consumableObject;

    public void InitializeQuantity()
    {
        quantity = baseQuantity;
    }
}