using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Cooking : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject cookingUI;
    [SerializeField] private GameObject resultUI;

    [Header("Saving References")]
    [SerializeField] private string sceneToSave;
    [SerializeField] private GameObject dishes;

    [Header("References")]
    [SerializeField] private GameObject recipeUI;
    [SerializeField] private GameObject recipeInfos;
    [SerializeField] private GameObject instruction;
    [SerializeField] private GameObject timer;
    [SerializeField] private TextMeshProUGUI consumablesList;
    [SerializeField] private Recipe currentRecipe;
    [SerializeField] private Transform contentRecipes;

    [Header("Parameters")]
    [SerializeField] private float consumables3Dsliced = 0;
    [SerializeField] private float totalConsumablesRequired = 0;
    [SerializeField] private float timeBeforeCutting = 5f;
    [SerializeField] private float timeToCreateFinalProduct = 3f;

    [Header("Lists")]
    [SerializeField] private List<Recipe> recipes = new List<Recipe>();
    [SerializeField] private List<Consumable> consumablesPossessed = new List<Consumable>();

    private bool recipeInPreparation;

    public List<Consumable> ConsumablesPossessed
    {
        get { return consumablesPossessed; }
    }

    Consumable3DSpawner consumable3DSpawner;

    private void Start()
    {
        consumable3DSpawner = FindObjectOfType<Consumable3DSpawner>();

        HandleGameStart();

        AddRecipesAtStart();
    }

    private void HandleGameStart()
    {
        cookingUI.gameObject.SetActive(true);
        resultUI.gameObject.SetActive(false);

        timer.SetActive(false);

        recipeInfos.SetActive(false);

        instruction.SetActive(true);
        instruction.transform.GetComponentInChildren<Text>().text = "S�lectionner une recette";

        recipeInPreparation = false;

        InitializeConsumablesPossessed();

        // Reset UIs
        recipeInfos.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "";
        recipeInfos.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = "";

        consumablesList.text = "";

        resultUI.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = "";
    }

    private void Update()
    {
        HandleRecipesPossessed();
    }

    public void AddSlicedConsumablesToCount()
    {
        consumables3Dsliced++;
    }

    #region Final Product

    public IEnumerator HandleFinalProduct()
    {
        instruction.SetActive(true);
        instruction.GetComponentInChildren<Text>().text = "En attente du r�sultat...";

        yield return new WaitForSeconds(timeToCreateFinalProduct);

        instruction.SetActive(false);

        CreateFinalProduct();
    }

    public void CreateFinalProduct()
    {
        recipeInPreparation = false;

        float finalPrice = currentRecipe.basePrice * (1 + (consumables3Dsliced / totalConsumablesRequired));

        // Create "Final Product" Item
        Item finalProductItem = ScriptableObject.CreateInstance(typeof(Item)) as Item;

        finalProductItem.name = finalProductItem.itemName = currentRecipe.finalProduct.GetComponent<DishInfos>().dishName;
        finalProductItem.itemDescription = currentRecipe.finalProduct.GetComponent<DishInfos>().dishDescription;
        finalProductItem.itemPrice = finalPrice;
        finalProductItem.itemType = ItemType.Consumable;
        finalProductItem.itemSprite = currentRecipe.finalProduct.GetComponent<DishInfos>().dishSprite;

        // Add to items to save
        dishes.GetComponent<DontDestroyOnLoad>().SaveItem(finalProductItem);

        // Rich Text
        StringBuilder builder = new StringBuilder();

        builder.Append($"Plat <color=orange>'{finalProductItem.name}'</color> pr�par�  avec succ�s !").AppendLine().AppendLine();
        builder.Append($"Prix du plat = <color=green>{finalPrice}</color> ({consumables3Dsliced}/{totalConsumablesRequired})");

        resultUI.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = builder.ToString();

        resultUI.gameObject.SetActive(true);
    }

    public void Restart()
    {
        HandleGameStart();
    }

    public void Quit()
    {
        Debug.Log($"Quitting... return to game");

        SceneManager.LoadScene(sceneToSave);
    }

    #endregion

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

            recipeInfos.transform.GetChild(1).GetComponent<Button>().interactable = recipeCanBeCooked;
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

        recipe.GetComponent<Button>().onClick.AddListener(delegate { SelectRecipe(recipeToAdd.recipeName); });

        recipe.GetComponent<Image>().sprite = recipeToAdd.recipeSprite;

        recipe.GetComponent<KeepRecipe>().recipe = recipeToAdd;
        //recipe.GetComponent<RecipePopup>().popup = popup;
    }

    public void RemoveRecipeFromList(Recipe recipeToRemove)
    {
        recipes.Remove(recipeToRemove);
    }

    #endregion

    public void SelectRecipe(string recipeName)
    {
        Recipe recipe = GetRecipeItem(recipeName);

        ShowRecipeInfos(recipe);
    }

    private void ShowRecipeInfos(Recipe recipe)
    {
        consumablesList.text = recipe.GetInfosConsumablesRequired();

        recipeInfos.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = recipe.recipeName;
        recipeInfos.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = recipe.recipeDescription;

        recipeInfos.SetActive(true);

        // Save current recipe
        currentRecipe = recipe;
    }

    public void CookingRecipe()
    {
        if (recipeInPreparation || currentRecipe == null) return;

        cookingUI.gameObject.SetActive(false);

        FindObjectOfType<Blade>().CanCut = true;

        instruction.transform.GetComponentInChildren<Text>().text = "Couper les diff�rents ingr�dients";

        recipeInPreparation = true;

        if (currentRecipe.canBeCooked)
        {
            // Get total of required consumables (differents consumables with their quantity)
            totalConsumablesRequired = 0;
            for (int i = 0; i < currentRecipe.consumablesRequired.Count; i++)
            {
                for (int j = 0; j < currentRecipe.consumablesRequired[i].quantity; j++)
                {
                    totalConsumablesRequired++;
                }
            }

            // Create temporary array to stock consumables objects
            GameObject[] consumablesRequired = new GameObject[(int)totalConsumablesRequired];

            // Get consumables objects of each required consumables with quantity
            int index = 0;
            for (int i = 0; i < currentRecipe.consumablesRequired.Count; i++)
            {
                UpdateConsumable(currentRecipe.consumablesRequired[i].consumable, currentRecipe.consumablesRequired[i].consumable.quantity - currentRecipe.consumablesRequired[i].quantity);

                for (int j = 0; j < currentRecipe.consumablesRequired[i].quantity; j++)
                {
                    consumablesRequired[index] = currentRecipe.consumablesRequired[i].consumable.consumableObject;
                    index++;
                }
            }

            consumables3Dsliced = 0;

            // Transfer temporary array to Consumable3DSpawner
            StartCoroutine(TransferConsumablesToSpawn(consumablesRequired));
        }
    }

    IEnumerator TransferConsumablesToSpawn(GameObject[] consumablesRequired)
    {
        yield return new WaitForSeconds(1f);

        instruction.SetActive(false);
        timer.SetActive(true);

        float timeToWait = timeBeforeCutting;

        while (timeToWait > -1)
        {
            timer.GetComponentInChildren<Text>().text = timeToWait.ToString();

            yield return new WaitForSeconds(1f);

            timeToWait--;
        }

        timer.SetActive(false);

        consumable3DSpawner.AddConsumablesToSpawn(consumablesRequired);
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

    private void InitializeConsumablesPossessed()
    {
        for (int i = 0; i < consumablesPossessed.Count; i++)
        {
            consumablesPossessed[i].InitializeQuantity();
        }
    }

    private void UpdateConsumable(Consumable consumableToUpdateQuantity, int newQuantity)
    {
        if (newQuantity < 0)
            newQuantity = 0;

        consumableToUpdateQuantity.quantity = newQuantity;
    }

    #endregion
}