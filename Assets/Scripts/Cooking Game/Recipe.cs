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
        public Consumable consumable;
        public int quantity;
    }

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

            builder.Append($"x{consumablesRequired[i].quantity} {consumablesRequired[i].consumable.consumableName}</color>").AppendLine();
        }

        return builder.ToString();
    }

    private bool CanUseConsumable(ConsumablesRequired consumableRequired)
    {
        bool canUse = false;

        List<Consumable> consumablesPossessed = FindObjectOfType<Cooking>().ConsumablesPossessed;

        for (int i = 0; i < consumablesPossessed.Count; i++)
        {
            if (consumablesPossessed[i] == consumableRequired.consumable && consumablesPossessed[i].quantity >= consumableRequired.quantity)
            {
                canUse = true;
            }
        }

        return canUse;
    }
}