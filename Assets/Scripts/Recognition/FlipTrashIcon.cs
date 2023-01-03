using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Recognition
{

    public class FlipTrashIcon : MonoBehaviour
    {
        [SerializeField] private Sprite closedTrash;
        [SerializeField] private Sprite openTrash;

        private Image _image;
        void Start()
        {
            _image = GetComponent<Image>();
        }

        public void OnPointerEnterClear()
        {
            if (_image && openTrash) _image.sprite = openTrash;
        }

        public void OnPointerExitClear()
        {
            if (_image && closedTrash) _image.sprite = closedTrash;
        }
    }
}