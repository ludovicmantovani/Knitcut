using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerRecipesInventory : MonoBehaviour
{
    [SerializeField] private GameObject recipeUI;
    [SerializeField] private Transform recipesParent;

    private bool isOpen = true;
    private PlayerController player;
    private List_Slots listSlots;

    private List<int> recipesIndex;

    public List<int> RecipesIndex
    {
        get { return recipesIndex; }
    }

    private void Start()
    {
        player = FindObjectOfType<PlayerController>();
        listSlots = FindObjectOfType<List_Slots>();
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

        for (int i = 0; i < listSlots.stuffs.Length; i++)
        {
            if (listSlots.stuffs[i].GetType() == typeof(Recipe) && !recipesInStuffs.Contains((Recipe)listSlots.stuffs[i]))
            {
                recipesInStuffs.Add((Recipe)listSlots.stuffs[i]);
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
        if (recipesParent == null) return;

        Transform recipeObject = Instantiate(recipeUI, recipesParent).transform;

        recipeObject.GetComponent<KeepRecipe>().recipe = recipe;

        recipeObject.GetChild(0).GetComponent<Image>().sprite = recipe.recipeSprite;
        recipeObject.GetChild(1).GetComponent<Text>().text = recipe.recipeName;

        recipesIndex.Add(int.Parse(recipe.recipeIndex));

        SaveRecipes();
    }

    public bool CheckIfHasRecipe(Recipe recipe)
    {
        for (int i = 0; i < recipesParent.childCount; i++)
        {
            Recipe recipeInInventory = recipesParent.GetChild(i).GetComponent<KeepRecipe>().recipe;

            if (recipeInInventory != null && recipeInInventory == recipe) return true;
        }

        return false;
    }
}