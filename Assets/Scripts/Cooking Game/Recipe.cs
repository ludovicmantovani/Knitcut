using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[CreateAssetMenu(fileName = "Recipe", menuName = "ScriptableObject/Cooking/Recipe")]
public class Recipe : ScriptableObject
{
    [Serializable]
    public class ConsumablesRequired
    {
        public Item consumable;
        public int quantity;
    }

    public string recipeIndex;
    public string recipeName;
    public string recipeDescription;
    public List<ConsumablesRequired> consumablesRequired;
    public GameObject finalProduct;
    public float basePrice;
    public float finalPrice;
    public bool canBeCooked;
    public Sprite recipeSprite;

    public string GetInfosConsumablesRequired()
    {
        StringBuilder builder = new StringBuilder();

        for (int i = 0; i < consumablesRequired.Count; i++)
        {
            if (CanUseConsumable(consumablesRequired[i]))
                builder.Append($"<color=green>");
            else
                builder.Append($"<color=red>");

            builder.Append($"x{consumablesRequired[i].quantity} {consumablesRequired[i].consumable.itemName}</color>").AppendLine();
        }

        return builder.ToString();
    }

    private bool CanUseConsumable(ConsumablesRequired consumableRequired)
    {
        bool canUse = false;

        List<Item> consumablesPossessed = FindObjectOfType<Cooking>().ConsumsPossessed;

        for (int i = 0; i < consumablesPossessed.Count; i++)
        {
            if (consumablesPossessed[i] == consumableRequired.consumable && consumablesPossessed[i].itemValue >= consumableRequired.quantity)
            {
                canUse = true;
            }
        }

        return canUse;
    }
}