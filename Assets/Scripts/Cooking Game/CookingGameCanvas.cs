using System.Collections;
using System.Collections.Generic;
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
    }

    public void HideRecipeInfo()
    {
        if (rawImageRecipesInfo)
            rawImageRecipesInfo.gameObject.SetActive(false);
    }

}
