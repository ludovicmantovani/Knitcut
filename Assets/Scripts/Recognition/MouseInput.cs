using System.Collections.Generic;
using UnityEngine;

namespace Recognition
{
    public class MouseInput : MonoBehaviour
    {
        MouseGesture mouseGesture;

        [Header("Gesture Properties")]
        [Tooltip("The distance of the rayCast to see if a object is Interactive")]
        [SerializeField] private float distanceRayCast = 2.5f;
        [SerializeField] private GestureClass gesture = new GestureClass();
        [SerializeField] private float correctRate;
        [SerializeField] private Vector2 resolution;
        [Range(0.01f, 1f)][SerializeField] private float percentageExtraLimit = 20;

        private RaycastHit hit;

        public GestureClass Gesture => gesture;

        [Header("Line Properties")]
        [SerializeField] private Transform area;
        [SerializeField] private LineRenderer lineModel;
        [Range(0.001f, 0.1f)][SerializeField] private float lineWidth = 0.2f;
        [Tooltip("The distance that the lineModel is being create from the camera")]
        [SerializeField] private float zline = 0.5f;
        [Tooltip("Material used in the lineModel")]
        [SerializeField] private Material lineMaterial;
        [Tooltip("The width of the line that is created when you do the gesture, We recommend one unit more than the brush used in the creation of the texture")]
        [SerializeField] private int widthTextLine = 3;

        private int index;
        private LineRenderer lineToDraw = null;
        private bool canCreateLine;

        private void Start()
        {
            mouseGesture = FindObjectOfType<MouseGesture>();

            canCreateLine = false;

            lineToDraw = Instantiate(lineModel, area);

            gesture.SetTextWidht(widthTextLine);
        }

        private void SetupLine()
        {
            lineModel.material = lineMaterial;
            lineModel.startWidth = lineWidth;

            if (lineMaterial == null)
            {
                Debug.LogError("<b>Mouse Gesture Interpretation:</b> Line Material need a material to display colors in the drawning line");
            }
        }

        private void Update()
        {
            SetupLine();

            DetectGesture();
        }

        private void FixedUpdate()
        {
            HandleArea();
        }

        #region Gesture

        private void DetectGesture()
        {
            // If do not release button
            if (!Input.GetMouseButtonUp(0))
            {
                return;
            }

            // If button released

            if ((canCreateLine && gesture.GetIsGesturing()) || (!canCreateLine && gesture.GetIsGesturing()))
                lineToDraw = Instantiate(lineModel, area);

            index = 0;
            gesture.SetIsGesturing(b: false);
        }

        public void CompareGestureToModel()
        {
            if (gesture.mouseData.Count <= 0) return;

            index = 0;
            gesture.SetIsGesturing(b: false);

            Texture2D texture2D = gesture.ConvertToTexture2D(resolution);

            if ((bool)texture2D)
            {
                float[] results = gesture.CompareDrawingWithPattern(texture2D, mouseGesture.CurrentPattern);
                float score = results[0];
                float percentageExtra = results[1];

                if (percentageExtra >= percentageExtraLimit)
                {
                    Debug.Log($"Not enough resemblance ({percentageExtra * 100}% not same) : not calculating score");
                    mouseGesture.OnGestureWrong();

                    mouseGesture.Score.text = $"Not enough resemblance";
                }
                else
                {
                    if (score >= correctRate)
                    {
                        mouseGesture.OnGestureCorrect();
                    }
                    else
                    {
                        mouseGesture.OnGestureWrong();
                    }

                    mouseGesture.Score.text = $"{score * 100:0.00}%";
                }
            }

            Clear();
        }

        #endregion

        #region Line Drawing

        private void HandleArea()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (!Physics.Raycast(ray, out hit, distanceRayCast))
            {
                canCreateLine = false;
                return;
            }

            HandleInteractiveArea();
        }

        private void HandleInteractiveArea()
        {
            if (hit.collider.gameObject.CompareTag("Interactive"))
            {
                canCreateLine = true;

                DrawLine();
            }
        }

        private void DrawLine()
        {
            while (Input.GetMouseButton(0) && !gesture.mouseData.Contains(Input.mousePosition))
            {
                if (lineToDraw == null) return;

                mouseGesture.Score.text = "Score ?";
                gesture.SetIsGesturing(b: true);
                gesture.mouseData.Add(Input.mousePosition);

                lineToDraw.positionCount = index + 1;

                Vector3 position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, zline);

                lineToDraw.SetPosition(index, Camera.main.ScreenToWorldPoint(position));
                index++;
            }
        }

        public void Clear()
        {
            gesture.mouseData = new List<Vector3>();

            for (int i = 0; i < area.childCount; i++)
            {
                Destroy(area.GetChild(i).gameObject);
            }

            lineToDraw = Instantiate(lineModel, area);
        }

        #endregion
    }
}