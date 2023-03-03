using UnityEngine;
using UnityEngine.EventSystems;

public class BladeUIDetection : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (FindObjectOfType<Blade>())
            FindObjectOfType<Blade>().CanCut = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (FindObjectOfType<Blade>())
            FindObjectOfType<Blade>().CanCut = false;
    }
}