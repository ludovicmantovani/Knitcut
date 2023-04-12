using System.Collections;
using UnityEngine;

public class SeedGrowth : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject adultPlant;
    [SerializeField] private GameObject sprout;
    [SerializeField] private GameObject seed;
    [SerializeField] private GameObject dehydratedPlant;
    [SerializeField] private GameObject sickedPlant;
    [SerializeField] private ProductState productState;

    public enum ProductState
    {
        InGrowth,
        Sick,
        Dehydrated
    }

    private CropPlot cropPlot;

    public CropPlot CropPlot
    {
        get { return cropPlot; }
        set { cropPlot = value; }
    }

    [Header("Datas")]
    [SerializeField] private bool isGrowing;
    [SerializeField] private bool isSick;
    [SerializeField] private bool isDehydrated;
    [SerializeField] private float timeOfGrowthSeed = 10f;
    [SerializeField] private float timeOfGrowthSprout = 10f;

    private void Start()
    {
        productState = ProductState.InGrowth;
    }

    private void Update()
    {
        HandleGrowth();
    }

    private void HandleGrowth()
    {
        switch (productState)
        {
            case ProductState.InGrowth:
                InGrowth();
                break;
            case ProductState.Sick:
                Sick();
                break;
            case ProductState.Dehydrated:
                Dehydrated();
                break;
            default:
                break;
        }
    }

    private void InGrowth()
    {
        if (!isGrowing)
        {
            isGrowing = true;

            StartCoroutine(Growing());
        }
    }

    private IEnumerator Growing()
    {
        yield return new WaitForSeconds(timeOfGrowthSeed);

        GameObject sproutObject = Instantiate(sprout, transform);

        Destroy(seed);

        yield return new WaitForSeconds(timeOfGrowthSprout);

        GameObject adultObject = Instantiate(adultPlant, transform);

        Destroy(sproutObject);

        Debug.Log($"{adultObject.name} is ready");

        cropPlot.Product = adultObject;
    }

    private void Sick()
    {

    }

    private void Dehydrated()
    {

    }
}