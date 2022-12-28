using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObject/Item")]
public class Item : ScriptableObject
{
    public string itemName;
    public ItemType itemType;
    public Sprite itemSprite;
    public ScriptableObject itemSpecifities;
    public bool isStackable;
    public int maxStackSize;
}

public enum ItemType
{
    Consumable,
    Recipe,
    Tool,
    Other
}