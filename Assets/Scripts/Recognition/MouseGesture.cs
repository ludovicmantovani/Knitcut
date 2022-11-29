using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Recognition
{
    public class MouseGesture : MonoBehaviour
    {
        [SerializeField] private List<Texture2D> texturesPatterns;
        [SerializeField] private List<Texture2D> texturesToDisplay;
        [SerializeField] private RawImage imageModel;
        [SerializeField] private Text score;

        private int currentPatternIndex = -1;

        public Texture2D CurrentPattern
        {
            get => texturesPatterns[currentPatternIndex];
        }

        public Texture2D CurrentDisplay
        {
            get => texturesToDisplay[currentPatternIndex];
        }

        public Text Score
        {
            get => score;
        }

        private void Start()
        {
            SelectRandomPattern();
            DisplayPattern();
        }

        public void SelectRandomPattern()
        {
            currentPatternIndex = Random.Range(0, texturesPatterns.Count);
        }

        public void DisplayPattern()
        {
            imageModel.texture = CurrentDisplay;
        }

        public void OnGestureCorrect()
        {
            Debug.Log($"Gesture correct !");
        }

        public void OnGestureWrong()
        {
            Debug.Log($"Gesture wrong !");
        }
    }
}