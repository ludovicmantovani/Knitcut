using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class Cooking : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private Canvas resultUI;
    [SerializeField] private Canvas gameUI;
    [SerializeField] private Canvas tutorialUI;

    [Header("References")]
    [SerializeField] private GameObject recipeUI;
    [SerializeField] private Recipe currentRecipe;
    [SerializeField] private Transform contentRecipes;

    [Header("Parameters")]
    [SerializeField] private float consumables3Dsliced = 0;
    [SerializeField] private float totalConsumablesRequired = 0;
    [SerializeField] private float timeBeforeStarting = 0.3f;
    [SerializeField] private float timeBeforeCutting = 5f;
    [SerializeField] private float timeToCreateFinalProduct = 3f;

    [Header("Lists")]
    [SerializeField] private List<Recipe> recipes = new List<Recipe>();

    private bool recipeInPreparation;
    private CookingGameCanvas _cookingGameCanvas = null;

    private Consumable3DSpawner consumable3DSpawner;
    private Blade blade;

    private List<MinigameManager.PlayerItem> consumablesInInventory = new List<MinigameManager.PlayerItem>();

    private VideoPlayer videoPlayer;

    private void Start()
    {
        consumable3DSpawner = FindObjectOfType<Consumable3DSpawner>();
        blade = FindObjectOfType<Blade>();

        tutorialUI.gameObject.SetActive(false);

        HandleGameStart();

        AddRecipesAtStart();

        HandlePlayerConsumables();

        PlayTutorial();
    }

    #region Tutorial

    public void PlayTutorial()
    {
        tutorialUI.gameObject.SetActive(true);

        videoPlayer = tutorialUI.GetComponent<VideoPlayer>();

        videoPlayer.loopPointReached += StopTutorial;

        videoPlayer.Play();
    }

    public void SkipTutorial()
    {
        StopTutorial(videoPlayer);
    }

    public void StopTutorial(VideoPlayer videoPlayer)
    {
        videoPlayer.Stop();

        tutorialUI.gameObject.SetActive(false);
    }

    #endregion

    private void HandleGameStart()
    {
        recipeInPreparation = false;

        // Reset UIs
        
        if (gameUI && gameUI.TryGetComponent<CookingGameCanvas>(out _cookingGameCanvas))
            _cookingGameCanvas.SetRecipeData();
    }

    private void Update()
    {
        HandleRecipesPossessed();
    }

    public void AddSlicedConsumablesToCount()
    {
        consumables3Dsliced++;
    }

    private void HandlePlayerConsumables()
    {
        consumablesInInventory.Clear();

        for (int i = 0; i < MinigameManager.PlayerItems.Count; i++)
        {
            if (MinigameManager.PlayerItems[i].item.itemType == ItemType.Consumable)
                consumablesInInventory.Add(MinigameManager.PlayerItems[i]);
        }
    }

    #region Final Product

    public IEnumerator HandleFinalProduct()
    {
        blade.CanCut = false;

        yield return new WaitForSeconds(timeToCreateFinalProduct);

        CreateFinalProduct();
    }

    public void CreateFinalProduct()
    {
        recipeInPreparation = false;

        float finalPrice = currentRecipe.basePrice * (1 + (consumables3Dsliced / totalConsumablesRequired));

        // Save datas
        MinigameManager.FinalizeMG(MinigameManager.MGType.Cooking, currentRecipe, finalPrice);

        CookingResultCanvas cookingResultCanvas = null;

        if (resultUI && resultUI.TryGetComponent<CookingResultCanvas>(out cookingResultCanvas))
        {
            cookingResultCanvas.SetData(
                Mathf.FloorToInt(finalPrice).ToString(),
                Mathf.FloorToInt(consumables3Dsliced).ToString() + "/" + Mathf.FloorToInt(totalConsumablesRequired).ToString(),
                currentRecipe.finalProduct.GetComponent<DishInfos>().dishName + " préparé(e)  avec succès !",
                currentRecipe.finalProduct.GetComponent<DishInfos>().dishSprite
                );
            resultUI.gameObject.SetActive(true);
        }
    }

    public void Quit()
    {
        MinigameManager.SwitchScene();
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
    }

    #region Options

    private void AddRecipesAtStart()
    {
        // Get All recipes
        recipes = MinigameManager.RecipesPossessed;

        if (recipes.Count == 0) return;

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

        if (_cookingGameCanvas)
            _cookingGameCanvas.CanInteract(recipe.canBeCooked);
    }

    private void ShowRecipeInfos(Recipe recipe)
    {
        if (_cookingGameCanvas)
            _cookingGameCanvas.SetRecipeData(
                recipe.recipeName,
                recipe.recipeDescription,
                recipe.GetInfosConsumablesRequired(consumablesInInventory),
                true
                );

        // Save current recipe
        currentRecipe = recipe;
    }

    public void CookingRecipe()
    {
        if (recipeInPreparation || currentRecipe == null) return;

        if (!currentRecipe.canBeCooked) return;

        if (_cookingGameCanvas)
        {
            _cookingGameCanvas.HideRecipeSelector();
            _cookingGameCanvas.SetRecipeData("Coupez les différents ingrédients");
        }
        
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
                MinigameManager.RemovePlayerItem(currentRecipe.consumablesRequired[i].consumable, currentRecipe.consumablesRequired[i].quantity);

                for (int j = 0; j < currentRecipe.consumablesRequired[i].quantity; j++)
                {
                    consumablesRequired[index] = currentRecipe.consumablesRequired[i].consumable.itemObject;
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
        yield return new WaitForSeconds(timeBeforeStarting);

        float timeToWait = timeBeforeCutting;

        blade.CanCut = true;

        if (_cookingGameCanvas)
        {
            _cookingGameCanvas.ReSizeDescriptionText(4f);

            while (timeToWait > -1)
            {
                _cookingGameCanvas.SetRecipeData(
                    "Coupez les différents ingrédients",
                    timeToWait.ToString());
                yield return new WaitForSeconds(1f);
                timeToWait--;
            }

            _cookingGameCanvas.HideRecipeInfo();
        }

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
}