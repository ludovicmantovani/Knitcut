using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerRecipesInventory : MonoBehaviour
{
    //[SerializeField] private GameObject recipeUI;
    //[SerializeField] private Transform recipesParent;

    private bool isOpen = true;
    private PlayerController player;
    private List_Slots listSlots;
    private BookRecipes bookRecipes;

    private List<int> recipesIndex;

    public List<int> RecipesIndex
    {
        get { return recipesIndex; }
    }

    private void Start()
    {
        player = FindObjectOfType<PlayerController>();
        listSlots = FindObjectOfType<List_Slots>();
        bookRecipes = GetComponent<BookRecipes>();

        recipesIndex = new List<int>();

        gameObject.SetActive(false);

        LoadRecipes();
    }

    private void Update()
    {
        HandleInventoryUI();
    }

    public void SaveRecipes()
    {
        SaveSystem.Save(SaveSystem.SaveType.Save_PlayerRecipesInventory, this);
    }

    private void LoadRecipes()
    {
        // Load Recipes Data
        PlayerInventory_Data data = (PlayerInventory_Data)SaveSystem.Load(SaveSystem.SaveType.Save_PlayerRecipesInventory, this);

        if (data == null) return;

        List<int> recipesIndexLoaded = data.recipesIndex.ToList();

        // LoadRecipesUI
        List<Recipe> recipes = GetRecipesFromStuffs();

        if (recipes == null || recipes.Count == 0) return;
        
        for (int i = 0; i < recipesIndexLoaded.Count; i++)
        {
            Recipe recipe = recipes[recipesIndexLoaded[i]];

            AddRecipeToInventory(recipe);
        }
    }

    private List<Recipe> GetRecipesFromStuffs()
    {
        List<Recipe> recipesInStuffs = new List<Recipe>();

        for (int i = 0; i < listSlots.Stuffs.Length; i++)
        {
            if (listSlots.Stuffs[i].GetType() == typeof(Recipe) && !recipesInStuffs.Contains((Recipe)listSlots.Stuffs[i]))
            {
                recipesInStuffs.Add((Recipe)listSlots.Stuffs[i]);
            }
        }

        return recipesInStuffs;
    }

    #region Inventory

    public void HandleInventoryUI()
    {
        if (player.PlayerInput.RecipesInventoryAction.triggered && isOpen)
        {
            isOpen = false;
            gameObject.SetActive(true);

            MinigameManager.AddOpenInventory(gameObject);
        }
        else if (player.PlayerInput.RecipesInventoryAction.triggered && !isOpen)
        {
            isOpen = true;
            gameObject.SetActive(false);

            MinigameManager.RemoveOpenInventory(gameObject);
        }
    }

    #endregion

    public void AddRecipeToInventory(Recipe recipe)
    {
        recipesIndex.Add(int.Parse(recipe.recipeIndex));

        bookRecipes.AddPage(recipe);

        SaveRecipes();
    }

    public bool CheckIfHasRecipe(Recipe recipe)
    {
        return bookRecipes.CheckRecipe(recipe);
    }

    public List<Recipe> GetRecipes()
    {
        return bookRecipes.GetRecipes();
    }
}