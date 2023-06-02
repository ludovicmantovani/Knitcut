using UnityEngine;
using UnityEngine.EventSystems;

public class BladeUIDetection : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Blade blade;

    private void Start()
    {
        blade = FindObjectOfType<Blade>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        HandleBladeCut(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HandleBladeCut(false);
    }

    private void HandleBladeCut(bool state)
    {
        if (blade == null) return;

        blade.CutDetect = state;
    }
}