using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class FlowerCreation : MonoBehaviour
{
    [SerializeField] private int minPetales = 5;
    [SerializeField] private int maxPetales = 10;
    [SerializeField] private int totalPetales;
    [SerializeField] private GameObject petalePrefab;
    [SerializeField] private string pathFlowersResources = "Resources/Flower/petal";

    private void Start()
    {
        totalPetales = Random.Range(minPetales, maxPetales);

        string colorPath = GetRandomFlowerColorPath();

        for (int i = 0; i < totalPetales; i++)
        {
            CreateFlower(totalPetales, i, colorPath);
        }
    }

    private void CreateFlower(int totalPetales, int index, string colorPath)
    {
        Transform petale = Instantiate(petalePrefab, transform).transform;

        petale.SetAsFirstSibling();

        petale.GetChild(0).GetComponent<Image>().sprite = GetRandomPetalForm(colorPath);

        PlaceFlower(petale, totalPetales, index);
    }

    private void PlaceFlower(Transform petale, int totalPetales, int index)
    {
        int zRotation = index * (360 / totalPetales);

        Vector3 localEulerAngle = new Vector3(0, 0, zRotation);

        petale.localEulerAngles = localEulerAngle;
    }

    #region Handle Petal PNG/Sprite

    private string GetRandomFlowerColorPath()
    {
        // Get path  
        string path = $"{Application.dataPath}/{pathFlowersResources}";

        // Get total of differents colors
        int totalColors = 0;
        for (int i = 0; i < Directory.GetFiles(path, "*.meta").Length; i++)
        {
            totalColors++;
        }

        // Get random color
        int randomColorIndex = Random.Range(0, totalColors);
        string pathColor = path += $"/color{randomColorIndex + 1}";

        return pathColor;
    }

    private Sprite GetRandomPetalForm(string pathColor)
    {
        // Get total of differents forms
        int totalForms = 0;
        for (int i = 0; i < Directory.GetFiles(pathColor, "*.png").Length; i++)
        {
            totalForms++;
        }

        // Get random form
        int randomFormIndex = Random.Range(0, totalForms);
        string randomFormPath = Directory.GetFiles(pathColor, "*.png")[randomFormIndex];

        Sprite randomPetal = LoadSprite(randomFormPath);

        return randomPetal;
    }

    private Sprite LoadSprite(string path)
    {
        if (string.IsNullOrEmpty(path)) return null;
        if (File.Exists(path))
        {
            byte[] bytes = File.ReadAllBytes(path);
            Texture2D texture = new Texture2D(1, 1);
            texture.LoadImage(bytes);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            return sprite;
        }
        return null;
    }

    #endregion
}