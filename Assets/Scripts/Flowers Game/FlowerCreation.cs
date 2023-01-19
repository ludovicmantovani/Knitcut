using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class FlowerCreation : MonoBehaviour
{
    [SerializeField] private GameObject petalPrefab;
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private int minPetales = 5;
    [SerializeField] private int maxPetales = 10;
    [SerializeField] private int colorVersion = 5;

    private string _pathFlowersResources = "Flower/Petal/color";
    private int _totalPetales;

    private void Start()
    {
        _totalPetales = Random.Range(minPetales, maxPetales);

        string colorPath = _pathFlowersResources + Random.Range(1, colorVersion + 1);
        Debug.Log(colorPath);

        Object[] sameColorPetalSprites = Resources.LoadAll(colorPath, typeof(Sprite));

        for (int i = 0; i < _totalPetales; i++)
        {
            CreatePetal(_totalPetales, i, (Sprite)sameColorPetalSprites[Random.Range(0, sameColorPetalSprites.Length)]);
        }
        Instantiate(buttonPrefab, transform);
    }

    private void CreatePetal(int totalPetales, int index, Sprite sprite)
    {
        Transform petale = Instantiate(petalPrefab, transform).transform;
        petale.name = index.ToString();

        petale.SetAsFirstSibling();

        petale.GetComponent<Image>().sprite = sprite;

        PlacePetal(petale, totalPetales, index);
    }

    private void PlacePetal(Transform petale, int totalPetales, int index)
    {
        int zRotation = index * (360 / totalPetales);

        Vector3 localEulerAngle = new Vector3(0, 0, zRotation);

        petale.localEulerAngles = localEulerAngle;
    }
}