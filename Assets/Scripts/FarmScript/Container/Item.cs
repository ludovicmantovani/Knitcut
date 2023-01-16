using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObject/Item")]
public class Item : ScriptableObject
{
    public string itemName;
    public string itemDescription;
    public float itemPrice;
    public ItemType itemType;
    public Sprite itemSprite;
    public ScriptableObject itemSpecifities;
    public bool isStackable;
    public int maxStackSize = 1;
}

public enum ItemType
{
    Consumable,
    Recipe,
    Tool,
    Other
}