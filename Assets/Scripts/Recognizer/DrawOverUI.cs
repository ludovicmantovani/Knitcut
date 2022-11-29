using UnityEngine;
using UnityEngine.EventSystems;

namespace Minigame_Drawing_Recognier
{
    public class DrawOverUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Drawing Area Detection")]
        [SerializeField] private static bool inDrawingArea;

        public static bool InDrawingArea
        {
            get { return inDrawingArea; }
        }

        // Pointer enter drawing area
        public void OnPointerEnter(PointerEventData eventData)
        {
            inDrawingArea = true;
        }

        // Pointer exit drawing area
        public void OnPointerExit(PointerEventData eventData)
        {
            inDrawingArea = false;
        }
    }
}