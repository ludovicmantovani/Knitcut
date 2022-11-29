using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class RecipePopup : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Recipe recipe;
    public GameObject popup;

    public void OnPointerEnter(PointerEventData eventData)
    {
        popup.SetActive(true);
        popup.transform.position = eventData.pointerCurrentRaycast.worldPosition;
        popup.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = recipe.GetInfosConsumablesRequired();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        popup.SetActive(false);
    }
}