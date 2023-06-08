using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CookingGameCanvas : MonoBehaviour
{
    [SerializeField] private TMP_Text infoTitle = null;
    [SerializeField] private string defaultInfoTitle = "Sélectionnez une recette.";
    [SerializeField] private TMP_Text infoDescription = null;
    [SerializeField] private TMP_Text infoConsumables = null;
    [SerializeField] private Button actionCooking = null;
    [SerializeField] private RawImage rawImageRecipes = null;
    [SerializeField] private RawImage rawImageRecipesInfo = null;
    [SerializeField] private Button quitButton = null;

    public Button ActionCooking
    {
        get { return actionCooking; }
        set { actionCooking = value; }
    }

    public void ReSizeDescriptionText(float coef)
    {
        if (infoDescription) infoDescription.fontSize *= coef;
    }

    public void SetRecipeData(
        string title = "",
        string description = "",
        string consumables = "",
        bool canSee = false)
    {
        if (infoTitle) infoTitle.text = title.Length > 0 ? title : defaultInfoTitle;
        if (infoDescription) infoDescription.text = description;
        if (infoConsumables) infoConsumables.text = consumables;
        SeeAction(canSee);
    }

    public void SeeAction(bool canSee = false)
    {
        if (actionCooking)
            actionCooking.gameObject.SetActive(canSee);
    }

    public void CanInteract(bool canInteract = true)
    {
        if (actionCooking)
            actionCooking.interactable = canInteract;
    }

    public void HideRecipeSelector()
    {
        if (rawImageRecipes)
            rawImageRecipes.gameObject.SetActive(false);

        if (quitButton)
            quitButton.gameObject.SetActive(false);
    }

    public void HideRecipeInfo()
    {
        if (rawImageRecipesInfo)
            rawImageRecipesInfo.gameObject.SetActive(false);
    }

}
