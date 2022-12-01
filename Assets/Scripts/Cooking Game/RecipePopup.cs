using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class RecipePopup : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Recipe recipe;
    public GameObject popup;

    [SerializeField] private bool popupOpened;

    private void Start()
    {
        popupOpened = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!popupOpened)
        {
            popupOpened = true;

            popup.SetActive(true);
            popup.transform.position = eventData.pointerCurrentRaycast.worldPosition;
            popup.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = recipe.GetInfosConsumablesRequired();
        }        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        popupOpened = false;

        popup.SetActive(false);
    }
}