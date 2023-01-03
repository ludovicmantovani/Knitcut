using UnityEngine;
using UnityEngine.EventSystems;

public class BladeUIDetection : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        FindObjectOfType<Blade>().CanCut = false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        FindObjectOfType<Blade>().CanCut = true;
    }
}