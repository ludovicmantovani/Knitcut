using UnityEngine;
using UnityEngine.UI;

public class PlayerRecipesInventory : MonoBehaviour
{
    [SerializeField] private GameObject recipeUI;
    [SerializeField] private Transform recipesParent;

    private bool isOpen = true;
    private PlayerController player;

    private void Start()
    {
        player = FindObjectOfType<PlayerController>();

        gameObject.SetActive(false);
    }

    private void Update()
    {
        HandleInventoryUI();
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

        recipeObject.GetChild(0).GetComponent<Image>().sprite = recipe.recipeSprite;
        recipeObject.GetChild(1).GetComponent<Text>().text = recipe.recipeName;
    }
}