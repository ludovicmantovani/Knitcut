using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BookRecipes : MonoBehaviour
{
    [SerializeField] private float pageSpeed = 0.5f;
    [SerializeField] private GameObject pagePrefab;
    [SerializeField] private Transform pagesParent;
    [SerializeField] private List<Transform> pages;
    [SerializeField] private GameObject forwardButton;
    [SerializeField] private GameObject backButton;

    private List<MinigameManager.PlayerItem> consumablesInInventory = new List<MinigameManager.PlayerItem>();

    private int index = -1;
    private bool rotate = false;

    private void Start()
    {
        InitialState();
    }

    public void InitialState()
    {
        for (int i = 0; i < pages.Count; i++)
        {
            pages[i].transform.rotation = Quaternion.identity;
        }

        pages[0].SetAsLastSibling();

        backButton.SetActive(false);
    }

    public void RotateForward()
    {
        if (rotate) return;

        index++;
        float angle = 180f;

        ForwardButtonActions();

        pages[index].SetAsLastSibling();
        StartCoroutine(Rotate(angle, true));
    }

    public void ForwardButtonActions()
    {
        if (!backButton.activeSelf) backButton.SetActive(true);

        if (index == pages.Count - 1) forwardButton.SetActive(false);
    }

    public void RotateBack()
    {
        if (rotate) return;

        float angle = 0;

        BackButtonActions();

        pages[index].SetAsLastSibling();
        StartCoroutine(Rotate(angle, false));
    }

    public void BackButtonActions()
    {
        if (!forwardButton.activeSelf) forwardButton.SetActive(true);

        if (index - 1 == -1) backButton.SetActive(false);
    }

    private IEnumerator Rotate(float angle, bool forward)
    {
        float value = 0f;

        while (true)
        {
            rotate = true;

            Quaternion targetRotation = Quaternion.Euler(0, angle, 0);

            value += Time.deltaTime * pageSpeed;

            pages[index].rotation = Quaternion.Lerp(pages[index].rotation, targetRotation, value);

            float angle1 = Quaternion.Angle(pages[index].rotation, targetRotation);

            if (angle1 < 0.1f)
            {
                if (!forward)
                {
                    index--;
                }
                rotate = false;

                break;
            }

            yield return null;
        }
    }

    public void AddPage(Recipe recipe)
    {
        GameObject page = Instantiate(pagePrefab, pagesParent);

        pages.Add(page.transform);

        PageRecipe pageRecipe = page.GetComponent<PageRecipe>();

        pageRecipe.Recipe = recipe;
        pageRecipe.RecipeName.text = recipe.recipeName;
        pageRecipe.RecipeImage.sprite = recipe.recipeSprite;
        pageRecipe.RecipeDescription.text = recipe.recipeDescription;
        pageRecipe.RecipeList.text = recipe.GetInfosConsumablesRequired(consumablesInInventory);

        pageRecipe.RecipeDescription.enabled = false;
        pageRecipe.RecipeList.enabled = false;

        pageRecipe.RecipeDescription.enabled = true;
        pageRecipe.RecipeList.enabled = true;

        pageRecipe.transform.SetAsFirstSibling();
    }

    public void HandlePlayerConsumables(PlayerInventory inventory)
    {
        consumablesInInventory.Clear();

        if (inventory == null) return;

        for (int i = 0; i < inventory.SearchItemsPossessed().Count; i++)
        {
            ItemHandler itemHandler = inventory.SearchItemsPossessed()[i];

            if (itemHandler.Item.itemType == ItemType.Consumable)
            {
                MinigameManager.PlayerItem playerItem = new MinigameManager.PlayerItem();
                playerItem.item = itemHandler.Item;
                playerItem.quantity = itemHandler.QuantityStacked;

                consumablesInInventory.Add(playerItem);
            }            
        }

        for (int i = 0; i < pages.Count; i++)
        {
            PageRecipe pageRecipe = pages[i].GetComponent<PageRecipe>();

            if (pageRecipe != null)
            {
                Debug.Log(pageRecipe.Recipe.GetInfosConsumablesRequired(consumablesInInventory));

                pageRecipe.RecipeList.text = pageRecipe.Recipe.GetInfosConsumablesRequired(consumablesInInventory);
            }
        }
    }

    public bool CheckRecipe(Recipe recipe)
    {
        for (int i = 1; i < pages.Count; i++)
        {
            Recipe recipeInInventory = pages[i].GetComponent<PageRecipe>().Recipe;

            if (recipeInInventory != null && recipeInInventory == recipe) return true;
        }

        return false;
    }

    public List<Recipe> GetRecipes()
    {
        List<Recipe> recipes = new List<Recipe>();

        for (int i = 1; i < pages.Count; i++)
        {
            PageRecipe pageRecipe = pages[i].GetComponent<PageRecipe>();

            if (!recipes.Contains(pageRecipe.Recipe)) recipes.Add(pageRecipe.Recipe);
        }

        return recipes;
    }
}