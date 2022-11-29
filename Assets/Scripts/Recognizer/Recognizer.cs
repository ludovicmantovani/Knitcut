using System.Collections.Generic;
using UnityEngine;
using PDollarGestureRecognizer;
using System;
using System.IO;
using UnityEngine.UI;

namespace Minigame_Drawing_Recognier
{
    public class Recognizer : MonoBehaviour
    {
        [Header("Draw Parameters")]
        [SerializeField] private Transform gestureOnScreenPrefab;
        [SerializeField] private Transform drawsParent;
        [SerializeField] private float scoreMin = 0.8f;
        [SerializeField] private int totalVertexLimit = 10000;

        private List<Gesture> trainingSet = new List<Gesture>();
        private List<Point> points = new List<Point>();
        private List<LineRenderer> gestureLinesRenderer = new List<LineRenderer>();

        private LineRenderer currentGestureLineRenderer;

        private Vector3 virtualKeyPosition = Vector2.zero;

        private int strokeId = -1;
        private int vertexCount = 0;

        [Header("Price Modifier")]
        [SerializeField] private float basePrice;
        [SerializeField] private float bonus;
        [SerializeField] private float finalPrice;
        [SerializeField] private Text priceText;
        [SerializeField] private GameObject pricePanel;

        [Header("UI")]
        [SerializeField] private Transform drawingArea;
        [SerializeField] private InputField result;
        [SerializeField] private InputField newModelName;
        [SerializeField] private Color correctColor;
        [SerializeField] private Color wrongColor;

        [Header("Model(s)")]
        [SerializeField] private List<Sprite> modelsSprite;
        [SerializeField] private Image modelSurface;

        private string message;
        private string newGestureName = "";
        private bool recognized;

        void Start()
        {
            LoadGestures();

            LoadModel();

            pricePanel.SetActive(false);
        }

        private void LoadGestures()
        {
            //Load pre-made gestures
            /*TextAsset[] gesturesXml = Resources.LoadAll<TextAsset>("GestureSet/10-stylus-MEDIUM/");
            foreach (TextAsset gestureXml in gesturesXml)
                trainingSet.Add(GestureIO.ReadGestureFromXML(gestureXml.text));*/

            //Load user custom gestures
            string[] filePaths = Directory.GetFiles(Application.dataPath + "/Resources/Recognizer/", "*.xml");

            // Choose one random model from list
            int randomFilePathIndex = UnityEngine.Random.Range(0, filePaths.Length);
            string randomFilePath = filePaths[randomFilePathIndex];
            Debug.Log($"Random model : {GestureIO.ReadGestureFromFile(randomFilePath).Name}");

            trainingSet.Add(GestureIO.ReadGestureFromFile(randomFilePath));
        }

        void Update()
        {
            if (CountTotalPoint() <= totalVertexLimit)
            {
                DrawUI();
            }
            else
            {
                Debug.Log($"Not enough sewing thread");
            }
        }

        private void DrawUI()
        {
            // Get position of mouse button click
            if (Input.GetMouseButton(0))
            {
                virtualKeyPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y);
            }

            // If cursor is in drawing area
            if (DrawOverUI.InDrawingArea)
            {
                // If mouse button 0 is hold down
                if (Input.GetMouseButtonDown(0))
                {
                    if (recognized)
                    {
                        ResetLinesRenderer();
                    }

                    ++strokeId;
                    Transform temporaryGesture = Instantiate(gestureOnScreenPrefab, transform.position, transform.rotation);
                    temporaryGesture.SetParent(drawsParent);
                    currentGestureLineRenderer = temporaryGesture.GetComponent<LineRenderer>();

                    gestureLinesRenderer.Add(currentGestureLineRenderer);

                    vertexCount = 0;
                }

                // Stop drawing
                if (Input.GetMouseButton(0) && currentGestureLineRenderer != null)
                {
                    /*Point point = new Point(virtualKeyPosition.x, -virtualKeyPosition.y, strokeId);

                    if (!points.Contains(point))
                    {
                        points.Add(point);

                        currentGestureLineRenderer.positionCount = ++vertexCount;
                        currentGestureLineRenderer.SetPosition(vertexCount - 1, Camera.main.ScreenToWorldPoint(new Vector3(virtualKeyPosition.x, virtualKeyPosition.y, 10)));
                    }*/
                    points.Add(new Point(virtualKeyPosition.x, -virtualKeyPosition.y, strokeId));

                    currentGestureLineRenderer.positionCount = ++vertexCount;
                    currentGestureLineRenderer.SetPosition(vertexCount - 1, Camera.main.ScreenToWorldPoint(new Vector3(virtualKeyPosition.x, virtualKeyPosition.y, 10)));
                }
            }
        }

        private void ResetLinesRenderer()
        {
            recognized = false;
            strokeId = -1;

            points.Clear();

            foreach (LineRenderer lineRenderer in gestureLinesRenderer)
            {
                lineRenderer.positionCount = 0;
                Destroy(lineRenderer.gameObject);
            }

            gestureLinesRenderer.Clear();
        }

        private void CreateShapeModelFile(List<Point> pointsList)
        {
            string fileName = String.Format("{0}/{1}-{2}.xml", Application.dataPath + "/Resources/Recognizer/", newGestureName, DateTime.Now.ToFileTime());

#if !UNITY_WEBPLAYER
            GestureIO.WriteGesture(pointsList.ToArray(), newGestureName, fileName);
#endif

            trainingSet.Add(new Gesture(pointsList.ToArray(), newGestureName));

            Debug.Log($"New model created : {newGestureName}");

            newGestureName = "";
        }

        private int CountTotalPoint()
        {
            int totalCount = 0;

            foreach (LineRenderer gestureLineRenderer in gestureLinesRenderer)
            {
                totalCount += gestureLineRenderer.positionCount;
            }

            return totalCount;
        }

        #region Handle Options

        public void Recognize()
        {
            if (points.Count == 0) return;

            recognized = true;

            Gesture candidate = new Gesture(points.ToArray());
            Result gestureResult = PointCloudRecognizer.Classify(candidate, trainingSet.ToArray());

            Debug.Log($"Current score : {gestureResult.Score}");

            if (gestureResult.Score >= scoreMin)
            {
                result.textComponent.color = correctColor;
                message = $"{gestureResult.GestureClass} : {(gestureResult.Score * 100).ToString("0.00")}%";
            }
            else
            {
                result.textComponent.color = wrongColor;
                message = "Retry !";
            }

            result.text = message;

            HandlePrice(gestureResult.Score);
        }

        #region Price & Next

        private void HandlePrice(float score)
        {
            pricePanel.SetActive(true);

            float percentage = float.Parse((score * 100).ToString("0.00"));

            if (score >= scoreMin)
            {
                finalPrice = (basePrice + bonus) + (basePrice / 4 * (1 + (percentage/100)));
                //Debug.Log($"Base:{basePrice} + Bonus:{bonus}  + {basePrice / 4 * (1 + (percentage / 100))}");
            }
            else
            {
                finalPrice = basePrice + (basePrice / 4 * (1 + (percentage / 100)));
                //Debug.Log($"Base:{basePrice} + {basePrice / 4 * (1 + (percentage / 100))}");
            }

            priceText.text = $"Price to pay? {finalPrice}";
        }

        public void Retry()
        {
            ResetLinesRenderer();

            pricePanel.SetActive(false);

            Debug.Log($"Try to get a better bonus !");
        }

        public void Continue()
        {
            pricePanel.SetActive(false);

            Debug.Log($"You have paid {finalPrice} for this product !");
        }

        #endregion

        public void AddModel()
        {
            if (newModelName.text == "") return;

            newGestureName = newModelName.text;

            CreateShapeModelFile(points);
        }

        private void LoadModel()
        {
            for (int i = 0; i < modelsSprite.Count; i++)
            {
                if (modelsSprite[i].name == trainingSet[0].Name)
                {
                    modelSurface.sprite =modelsSprite[i];
                    modelSurface.preserveAspect = true;
                }
            }
        }

        public void Clear()
        {
            ResetLinesRenderer();
        }

        #endregion
    }
}