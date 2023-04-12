using UnityEngine;

public class SeedGrowth : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject adultPlant;
    [SerializeField] private GameObject dehydratedPlant;
    [SerializeField] private GameObject sickedPlant;
    [SerializeField] private ProductState productState;

    public enum ProductState
    {
        Growing,
        Sick,
        Dehydrated
    }

    [Header("Datas")]
    [SerializeField] private bool isGrowing;
    [SerializeField] private bool isSick;
    [SerializeField] private bool isDehydrated;

    private void Start()
    {
        isGrowing = true;

        productState = ProductState.Growing;
    }

    private void Update()
    {
        HandleGrowth();
    }

    private void HandleGrowth()
    {
        switch (productState)
        {
            case ProductState.Growing:
                Growing();
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

    private void Growing()
    {

    }

    private void Sick()
    {

    }

    private void Dehydrated()
    {

    }
}