using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObject/Item")]
[System.Serializable]
public class Item : ScriptableObject
{
    public string itemName;
    public string itemDescription;
    public float itemBaseValue;
    public float itemValue;
    public ItemType itemType;
    public Sprite itemSprite;
    public GameObject itemObject;
    public bool isStackable;
    public int maxStackSize = 1;

    public void InitializeValue()
    {
        itemValue = itemBaseValue;
    }
}

public enum ItemType
{
    Consumable,
    Recipe,
    Dish,
    Product,
    Seed,
    Other
}