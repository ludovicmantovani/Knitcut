using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cooking : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject recipeUI;
    [SerializeField] private GameObject consumableUI;
    [SerializeField] private GameObject popup;
    [SerializeField] private Recipe currentRecipe;
    [SerializeField] private Transform contentRecipes;
    [SerializeField] private Transform contentConsumables;
    [SerializeField] private float consumables3Dsliced = 0;
    [SerializeField] private float totalConsumablesRequired = 0;

    [Header("Lists")]
    [SerializeField] private List<Recipe> recipes = new List<Recipe>();
    [SerializeField] private List<Consumable> consumablesPossessed = new List<Consumable>();
    [SerializeField] private List<GameObject> finalProducts = new List<GameObject>();

    public List<Consumable> ConsumablesPossessed
    {
        get { return consumablesPossessed; }
    }

    Consumable3DSpawner consumable3DSpawner;

    private void Start()
    {
        consumable3DSpawner = FindObjectOfType<Consumable3DSpawner>();

        popup.SetActive(false);

        AddRecipesAtStart();
        AddConsumablesAtStart();
    }

    private void Update()
    {
        HandleRecipesPossessed();
        HandleConsumablesPossessed();
    }

    public void AddSlicedConsumablesToCount()
    {
        consumables3Dsliced++;
    }

    public void HandleFinalProduct()
    {
        float finalPrice = currentRecipe.basePrice * (1 + (consumables3Dsliced / totalConsumablesRequired));

        GameObject finalProduct = Instantiate(currentRecipe.finalProduct, transform);

        finalProduct.name = currentRecipe.recipeName;
        finalProduct.GetComponent<RecipeInfos>().recipeName = currentRecipe.recipeName;
        finalProduct.GetComponent<RecipeInfos>().price = finalPrice;

        finalProducts.Add(finalProduct);

        Debug.Log($"Recipe '{currentRecipe.finalProduct}' cooked successfully : price = {finalPrice}");
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

        recipe.GetComponent<Image>().sprite = recipeToAdd.recipeSprite;

        recipe.GetComponent<RecipePopup>().recipe = recipeToAdd;
        recipe.GetComponent<RecipePopup>().popup = popup;
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
            // Get total of required consumables (differents consumables with their quantity)
            for (int i = 0; i < recipe.consumablesRequired.Count; i++)
            {
                for (int j = 0; j < recipe.consumablesRequired[i].quantity; j++)
                {
                    totalConsumablesRequired++;
                }
            }

            // Create temporary array to stock consumables objects
            GameObject[] consumablesRequired = new GameObject[(int)totalConsumablesRequired];

            // Get consumables objects of each required consumables with quantity
            int index = 0;
            for (int i = 0; i < recipe.consumablesRequired.Count; i++)
            {
                UpdateConsumable(recipe.consumablesRequired[i].consumable, recipe.consumablesRequired[i].consumable.quantity - recipe.consumablesRequired[i].quantity);

                for (int j = 0; j < recipe.consumablesRequired[i].quantity; j++)
                {
                    consumablesRequired[index] = recipe.consumablesRequired[i].consumable.consumableObject;
                    index++;
                }
            }

            // Save current recipe
            currentRecipe = recipe;

            // Transfer temporary array to Consumable3DSpawner
            consumable3DSpawner.AddConsumablesToSpawn(consumablesRequired);
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

        consumable.GetComponent<Image>().sprite = consumableToAdd.consumableSprite;
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