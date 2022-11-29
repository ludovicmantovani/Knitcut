using UnityEngine;

public class FlowerCreation : MonoBehaviour
{
    [SerializeField] private int minPetales = 5;
    [SerializeField] private int maxPetales = 10;
    [SerializeField] private int totalPetales;
    [SerializeField] private GameObject petalePrefab;

    private void Start()
    {
        totalPetales = Random.Range(minPetales, maxPetales);

        for (int i = 0; i < totalPetales; i++)
        {
            CreateFlower(totalPetales, i);
        }
    }

    private void CreateFlower(int totalPetales, int index)
    {
        Transform petale = Instantiate(petalePrefab, transform).transform;

        petale.SetAsFirstSibling();

        PlaceFlower(petale, totalPetales, index);
    }

    private void PlaceFlower(Transform petale, int totalPetales, int index)
    {
        int zRotation = index * (360 / totalPetales);

        Vector3 localEulerAngle = new Vector3(0, 0, zRotation);

        petale.localEulerAngles = localEulerAngle; 
    }
}