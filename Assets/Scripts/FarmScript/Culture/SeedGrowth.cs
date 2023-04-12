using UnityEngine;

public class SeedGrowth : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject adultPlant;
    [SerializeField] private GameObject sprout;
    [SerializeField] private GameObject seed;
    [SerializeField] private GameObject dehydratedPlant;
    [SerializeField] private GameObject sickedPlant;
    [SerializeField] private ProductGrowth productGrowth;
    [SerializeField] private ProductState productState;

    private GameObject adultObject;
    private GameObject sproutObject;

    [SerializeField]
    private MeshRenderer plantRenderer;

    public enum ProductGrowth
    {
        Seed,
        Sprout,
        Fruit,
        End
    }

    public enum ProductState
    {
        InGrowth,
        Sick,
        Dehydrated
    }

    private CropPlot cropPlot;

    [Header("Datas")]
    [SerializeField] private float timeOfGrowthSeed = 10f;
    [SerializeField] private float timeOfGrowthSprout = 10f;
    [SerializeField] private float timeOfGrowthFruit = 2f;
    [SerializeField] private float currentTime = 0f;
    [SerializeField] private float yPositionFix = 0.2f;

    [Header("Material")]
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material sickMaterial;
    [SerializeField] private Material dehydratedMaterial;

    #region Getters / Setters

    public MeshRenderer PlantRenderer
    {
        get { return plantRenderer; }
        set { plantRenderer = value; }
    }

    public CropPlot CropPlot
    {
        get { return cropPlot; }
        set { cropPlot = value; }
    }

    public ProductGrowth GetProductGrowth
    {
        get { return productGrowth; }
        set { productGrowth = value; }
    }

    public ProductState GetProductStates
    {
        get { return productState; }
        set { productState = value; }
    }

    #endregion

    private void Start()
    {
        plantRenderer = transform.GetComponentInChildren<MeshRenderer>();
        defaultMaterial = plantRenderer.material;

        productGrowth = ProductGrowth.Seed;
        productState = ProductState.InGrowth;
    }

    private void Update()
    {
        HandleTimer();

        HandleProductGrowth();
        HandleProductStates();
    }

    private void HandleTimer()
    {
        if (productState == ProductState.InGrowth)
        {
            currentTime -= Time.deltaTime;

            if (currentTime <= 0)
            {
                currentTime = 0;
            }
        }
    }

    #region Handle Product Growth

    private void HandleProductGrowth()
    {
        switch (productGrowth)
        {
            case ProductGrowth.Seed:
                SeedToSprout();
                break;
            case ProductGrowth.Sprout:
                SproutToFruit();
                break;
            case ProductGrowth.Fruit:
                FruitToFinal();
                break;
            case ProductGrowth.End:
                cropPlot.Product = adultObject;
                break;
            default:
                break;
        }
    }

    private void SeedToSprout()
    {
        currentTime = timeOfGrowthSeed;

        GetRandomState();

        productGrowth = ProductGrowth.Sprout;
    }

    private void SproutToFruit()
    {
        if (currentTime > 0) return;

        sproutObject = Instantiate(sprout, transform);

        Destroy(seed);

        plantRenderer = sproutObject.transform.GetComponentInChildren<MeshRenderer>();
        defaultMaterial = plantRenderer.material;

        currentTime = timeOfGrowthSprout;

        GetRandomState();

        productGrowth = ProductGrowth.Fruit;
    }

    private void FruitToFinal()
    {
        if (currentTime > 0) return;

        adultObject = Instantiate(adultPlant, transform);

        Destroy(sproutObject);

        plantRenderer = adultObject.transform.GetComponentInChildren<MeshRenderer>();
        defaultMaterial = plantRenderer.material;

        Vector3 productPosition = transform.position;
        productPosition.y += yPositionFix;
        transform.position = productPosition;

        currentTime = timeOfGrowthFruit;

        GetRandomState();

        productGrowth = ProductGrowth.End;
    }

    #endregion

    #region Handle Product States

    private void HandleProductStates()
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
        if (plantRenderer == null) return;

        plantRenderer.material = defaultMaterial;
    }

    private void Sick()
    {
        if (plantRenderer == null) return;

        plantRenderer.material = sickMaterial;
    }

    private void Dehydrated()
    {
        if (plantRenderer == null) return;

        plantRenderer.material = dehydratedMaterial;
    }

    private void GetRandomState()
    {
        int random = Random.Range(0, 3);

        if (random == 0)
        {
            productState = ProductState.InGrowth;
        }
        else if (random == 1)
        {
            productState = ProductState.Sick;
        }
        else if (random == 2)
        {
            productState = ProductState.Dehydrated;
        }
    }

    #endregion
}