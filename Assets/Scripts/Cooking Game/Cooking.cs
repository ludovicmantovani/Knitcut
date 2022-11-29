using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cooking : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject recipeUI;
    [SerializeField] private GameObject consumableUI;
    [SerializeField] private Transform contentRecipes;
    [SerializeField] private Transform contentConsumables;

    [Header("Lists")]
    [SerializeField] private List<Recipe> recipes = new List<Recipe>();
    [SerializeField] private List<Consumable> consumablesPossessed = new List<Consumable>();
    [SerializeField] private List<GameObject> finalProducts = new List<GameObject>();

    private void Start()
    {
        AddRecipesAtStart();
        AddConsumablesAtStart();
    }

    private void Update()
    {
        HandleRecipesPossessed();
        HandleConsumablesPossessed();
    }

    #region Recipes

    private void HandleRecipesPossessed()
    {
        for (int i = 0; i < contentRecipes.childCount; i++)
        {
            // If recipe in ui but not in list -> Destroy
            if (!recipes.Contains(GetRecipeItem(contentRecipes.GetChild(i).name)))
            {
                Destroy(contentRecipes.GetChild(i).gameObject);
            }
        }

        for (int i = 0; i < recipes.Count; i++)
        {
            // If recipe in list but not in ui -> Add
            if (GetRecipeUI(recipes[i].recipeName) == null)
            {
                AddRecipeFromListToUI(recipes[i]);
            }
        }

        HandleRecipesState();
    }

    private void HandleRecipesState()
    {
        // For each recipe
        foreach (Recipe recipe in recipes)
        {
            bool recipeCanBeCooked = true;

            // For each consumable required for the recipe
            for (int i = 0; i < recipe.consumablesRequired.Count; i++)
            {
                // If player do not possessed the required consumable, can not cook recipe
                if (!consumablesPossessed.Contains(recipe.consumablesRequired[i].consumable))
                {
                    recipeCanBeCooked = false;
                }

                if (consumablesPossessed.Contains(recipe.consumablesRequired[i].consumable) && consumablesPossessed[i].quantity < recipe.consumablesRequired[i].quantity)
                {
                    recipeCanBeCooked = false;
                }
            }

            recipe.canBeCooked = recipeCanBeCooked;

            GetRecipeUI(recipe.recipeName).GetComponent<Button>().interactable = recipeCanBeCooked;
        }
    }

    #region Options

    private void AddRecipesAtStart()
    {
        for (int i = 0; i < recipes.Count; i++)
        {
            AddRecipeFromListToUI(recipes[i]);
        }
    }

    public void AddRecipeToList(Recipe recipeToAdd)
    {
        recipes.Add(recipeToAdd);

        AddRecipeFromListToUI(recipeToAdd);
    }

    private void AddRecipeFromListToUI(Recipe recipeToAdd)
    {
        GameObject recipe = Instantiate(recipeUI, contentRecipes);
        recipe.name = recipeToAdd.recipeName;
        recipe.transform.GetChild(0).GetComponent<Text>().text = recipeToAdd.recipeName;
        recipe.GetComponent<Button>().onClick.AddListener(delegate { CookingRecipe(recipeToAdd.recipeName); });
    }

    public void RemoveRecipeFromList(Recipe recipeToRemove)
    {
        recipes.Remove(recipeToRemove);
    }

    #endregion

    public void CookingRecipe(string recipeName)
    {
        Recipe recipe = GetRecipeItem(recipeName);

        if (recipe.canBeCooked)
        {
            for (int i = 0; i < recipe.consumablesRequired.Count; i++)
            {
                UpdateConsumable(recipe.consumablesRequired[i].consumable, recipe.consumablesRequired[i].consumable.quantity - recipe.consumablesRequired[i].quantity);
            }

            finalProducts.Add(recipe.finalProduct);
        }
    }

    #region Getters

    private Recipe GetRecipeItem(string recipeName)
    {
        for (int i = 0; i < recipes.Count; i++)
        {
            if (recipes[i].recipeName.Equals(recipeName))
            {
                return recipes[i];
            }
        }

        return null;
    }

    private GameObject GetRecipeUI(string recipeName)
    {
        for (int i = 0; i < contentRecipes.childCount; i++)
        {
            if (contentRecipes.GetChild(i).name.Equals(recipeName))
            {
                return contentRecipes.GetChild(i).gameObject;
            }
        }

        return null;
    }

    #endregion

    #endregion

    #region Consumables

    private void HandleConsumablesPossessed()
    {
        for (int i = 0; i < contentConsumables.childCount; i++)
        {
            // If consumable in ui but not in list -> Destroy
            if (!consumablesPossessed.Contains(GetConsumableItem(contentConsumables.GetChild(i).name)))
            {
                Destroy(contentConsumables.GetChild(i).gameObject);
            }
        }

        for (int i = 0; i < consumablesPossessed.Count; i++)
        {
            // If consumable in list but not in ui -> Add
            if (GetConsumableUI(consumablesPossessed[i].consumableName) == null)
            {
                AddConsumableFromListToUI(consumablesPossessed[i]);
            }
        }
    }

    #region Options

    private void AddConsumablesAtStart()
    {
        for (int i = 0; i < consumablesPossessed.Count; i++)
        {
            AddConsumableFromListToUI(consumablesPossessed[i]);
        }
    }

    public void AddConsumableToList(Consumable consumableToAdd)
    {
        consumablesPossessed.Add(consumableToAdd);

        AddConsumableFromListToUI(consumableToAdd);
    }

    private void AddConsumableFromListToUI(Consumable consumableToAdd)
    {
        consumableToAdd.InitializeQuantity();

        GameObject consumable = Instantiate(consumableUI, contentConsumables);
        consumable.name = consumableToAdd.consumableName;
        consumable.transform.GetChild(0).GetComponent<Text>().text = consumableToAdd.consumableName;
        consumable.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = consumableToAdd.quantity.ToString();
    }

    public void RemoveConsumableFromList(Consumable consumableToRemove)
    {
        consumablesPossessed.Remove(consumableToRemove);
    }

    #endregion

    private void UpdateConsumable(Consumable consumableToUpdateQuantity, int newQuantity)
    {
        if (newQuantity < 0)
            newQuantity = 0;

        consumableToUpdateQuantity.quantity = newQuantity;

        GetConsumableUI(consumableToUpdateQuantity.consumableName).transform.GetChild(1).GetChild(0).GetComponent<Text>().text = newQuantity.ToString();
    }

    #region Getters

    private Consumable GetConsumableItem(string consumableName)
    {
        for (int i = 0; i < consumablesPossessed.Count; i++)
        {
            if (consumablesPossessed[i].consumableName.Equals(consumableName))
            {
                return consumablesPossessed[i];
            }
        }

        return null;
    }

    private GameObject GetConsumableUI(string consumableName)
    {
        for (int i = 0; i < contentConsumables.childCount; i++)
        {
            if (contentConsumables.GetChild(i).name.Equals(consumableName))
            {
                return contentConsumables.GetChild(i).gameObject;
            }
        }

        return null;
    }

    #endregion

    #endregion
}