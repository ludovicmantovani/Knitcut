using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PageRecipe : MonoBehaviour
{
    [SerializeField] private Recipe recipe;
    [SerializeField] private TextMeshProUGUI recipeName;
    [SerializeField] private Image recipeImage;
    [SerializeField] private TextMeshProUGUI recipeDescription;
    [SerializeField] private TextMeshProUGUI recipeList;

    public Recipe Recipe
    {
        get { return recipe; }
        set { recipe = value; }
    }

    public TextMeshProUGUI RecipeName
    {
        get { return recipeName; }
        set { recipeName = value; }
    }

    public Image RecipeImage
    {
        get { return recipeImage; }
        set { recipeImage = value; }
    }

    public TextMeshProUGUI RecipeDescription
    {
        get { return recipeDescription; }
        set { recipeDescription = value; }
    }

    public TextMeshProUGUI RecipeList
    {
        get { return recipeList; }
        set { recipeList = value; }
    }
}