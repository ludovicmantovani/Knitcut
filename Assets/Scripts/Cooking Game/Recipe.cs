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
        public bool ok;
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

    public string GetInfosConsumablesRequired(List<MinigameManager.PlayerItem> consumablesInInventory)
    {
        HandleRecipeConditions(consumablesInInventory);

        StringBuilder builder = new StringBuilder();

        for (int i = 0; i < consumablesRequired.Count; i++)
        {
            if (consumablesRequired[i].ok)
                builder.Append($"<color=green>");
            else
                builder.Append($"<color=red>");

            builder.Append($"{QuantityPossessed(consumablesRequired[i].consumable, consumablesInInventory)}/{consumablesRequired[i].quantity} {consumablesRequired[i].consumable.itemName}</color>").AppendLine();
        }

        return builder.ToString();
    }

    private int QuantityPossessed(Item consumable, List<MinigameManager.PlayerItem>  consumablesInInventory)
    {
        int quantityPossessed = 0;

        for (int i = 0; i < consumablesInInventory.Count; i++)
        {
            if (consumable == consumablesInInventory[i].item) quantityPossessed = consumablesInInventory[i].quantity;
        }

        return quantityPossessed;
    }

    #region Can Be Cooked

    private void HandleRecipeConditions(List<MinigameManager.PlayerItem> consumablesInInventory)
    {
        CheckIfRecipeCanBeCooked(consumablesInInventory);

        canBeCooked = true;

        for (int i = 0; i < consumablesRequired.Count; i++)
        {
            if (!consumablesRequired[i].ok) canBeCooked = false;
        }
    }
    
    private void CheckIfRecipeCanBeCooked(List<MinigameManager.PlayerItem> consumablesInInventory)
    {
        // For Each required consumable of recipe
        for (int i = 0; i < consumablesRequired.Count; i++)
        {
            List<MinigameManager.PlayerItem> playerItems = new List<MinigameManager.PlayerItem>();

            // Check each consumable in inventory
            for (int j = 0; j < consumablesInInventory.Count; j++)
            {
                // If consumable possessed
                if (consumablesRequired[i].consumable == consumablesInInventory[j].item) playerItems.Add(consumablesInInventory[j]);
            }

            if (playerItems.Count > 0)
            {
                int totalQuantity = 0;

                // Check consumable quantity
                for (int k = 0; k < playerItems.Count; k++)
                {
                    totalQuantity += playerItems[k].quantity;
                }

                if (totalQuantity >= consumablesRequired[i].quantity)
                {
                    consumablesRequired[i].ok = true;
                }
                else
                {
                    consumablesRequired[i].ok = false;
                }
            }
            else
            {
                consumablesRequired[i].ok = false;
            }
        }
    }

    #endregion
}