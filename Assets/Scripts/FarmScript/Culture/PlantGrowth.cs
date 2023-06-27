using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlantGrowth : MonoBehaviour
{
    #region Parameters

    [Header("References")]
    [SerializeField] private Plant plant;
    [SerializeField] private TMP_Text plantText;
    [SerializeField] private GameObject plantCanvas;
    [SerializeField] private Image plantFill;
    [SerializeField] private ProductGrowth productGrowth;
    [SerializeField] private ProductState productState;
    [SerializeField] private MeshRenderer plantRenderer;
    [SerializeField] private SkinnedMeshRenderer plantSkinRenderer;
    [SerializeField] private bool loadGrowthState = false;

    private float maxTime = 0f;
    
    private CropPlot cropPlot;
    private GameObject stateObject;

    public enum ProductGrowth
    {
        Seed,
        Sprout,
        Flower,
        FlowerFruit,
        End
    }

    public enum ProductState
    {
        InGrowth,
        Sick,
        Dehydrated
    }

    [Header("Datas")]
    [SerializeField] private float currentTime = 0f;
    [SerializeField] private float yPositionFix = 0.2f;

    [Header("Material")]
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material sickMaterial;
    [SerializeField] private Material dehydratedMaterial;

    #region Getters / Setters

    public Plant CurrentPlant
    {
        get { return plant; }
        set { plant = value; }
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

    public ProductState GetProductState
    {
        get { return productState; }
        set { productState = value; }
    }

    public MeshRenderer PlantRenderer
    {
        get { return plantRenderer; }
        set { plantRenderer = value; }
    }

    public bool LoadGrowthState
    {
        get { return loadGrowthState; }
        set { loadGrowthState = value; }
    }

    public float CurrentTimer
    {
        get { return currentTime; }
        set { currentTime = value; }
    }

    #endregion

    #endregion

    private void Start()
    {
        if (!loadGrowthState)
        {
            ActualizePlant(plant.seed);

            productGrowth = ProductGrowth.Seed;
            productState = ProductState.InGrowth;
        }

        plantText.text = plant.plantName;
    }

    private void Update()
    {
        plantCanvas.transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
        
        HandleTimer();

        HandleProductGrowth();
        HandleProductStates();
    }

    private void HandleTimer()
    {
        if (productState == ProductState.InGrowth)
        {
            currentTime -= Time.deltaTime;

            //plantFill.fillAmount -= 1.0f / currentTime * Time.deltaTime;
            plantFill.fillAmount = currentTime / maxTime;

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
                SeedGrowth();
                break;
            case ProductGrowth.Sprout:
                SproutGrowth();
                break;
            case ProductGrowth.Flower:
                FlowerGrowth();
                break;
            case ProductGrowth.FlowerFruit:
                FlowerReadyGrowth();
                break;
            case ProductGrowth.End:
                End();
                break;
            default:
                break;
        }
    }

    private void ActualizePlant(GameObject plantStateObject)
    {
        Destroy(stateObject);

        stateObject = Instantiate(plantStateObject, transform);

        plantRenderer = stateObject.transform.GetComponentInChildren<MeshRenderer>();

        if (plantRenderer == null)
        {
            plantSkinRenderer = stateObject.transform.GetComponentInChildren<SkinnedMeshRenderer>();

            if (plantSkinRenderer == null)
            {
                Debug.Log($"Error plant renderer");
            }
            else
            {
                defaultMaterial = plantSkinRenderer.material;
            }
        }
        else
        {
            defaultMaterial = plantRenderer.material;
        }
    }

    private void SeedGrowth()
    {
        currentTime = plant.timeOfGrowthSeed;
        maxTime = plant.timeOfGrowthSeed;

        productGrowth = ProductGrowth.Sprout;
    }

    private void SproutGrowth()
    {
        if (currentTime > 0) return;

        ActualizePlant(plant.sprout);

        currentTime = plant.timeOfGrowthSprout;
        maxTime = plant.timeOfGrowthSprout;

        productGrowth = ProductGrowth.Flower;
    }

    private void FlowerGrowth()
    {
        if (currentTime > 0) return;

        ActualizePlant(plant.plant);

        currentTime = plant.timeOfGrowthFlower;
        maxTime = plant.timeOfGrowthFlower;

        GetRandomState();

        productGrowth = ProductGrowth.FlowerFruit;
    }

    private void FlowerReadyGrowth()
    {
        if (currentTime > 0) return;

        ActualizePlant(plant.readyPlant);

        Vector3 productPosition = transform.position;
        productPosition.y += yPositionFix;
        transform.position = productPosition;

        currentTime = plant.timeOfGrowthFruit;
        maxTime = plant.timeOfGrowthFruit;

        productGrowth = ProductGrowth.End;
    }

    private void End()
    {
        cropPlot.Product = plant.fruit;
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
        if (plantRenderer == null && plantSkinRenderer == null) return;

        if (plantRenderer != null) plantRenderer.material = defaultMaterial;
        else if (plantSkinRenderer != null) plantSkinRenderer.material = defaultMaterial;
    }

    private void Sick()
    {
        if (plantRenderer == null && plantSkinRenderer == null) return;

        if (plantRenderer != null) plantRenderer.material = sickMaterial;
        else if (plantSkinRenderer != null) plantSkinRenderer.material = sickMaterial;
    }

    private void Dehydrated()
    {
        if (plantRenderer == null && plantSkinRenderer == null) return;

        if (plantRenderer != null) plantRenderer.material = dehydratedMaterial;
        else if (plantSkinRenderer != null) plantSkinRenderer.material = dehydratedMaterial;
    }

    private void GetRandomState()
    {
        int random = UnityEngine.Random.Range(0, 3);

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

    public void SetGrowthState(string growth, string state, Plant plant, float timeSkip)
    {
        productGrowth = (ProductGrowth)Enum.Parse(typeof(ProductGrowth), growth);
        productState = (ProductState)Enum.Parse(typeof(ProductState), state);

        loadGrowthState = true;

        object[] datas = plant.CurrentPlantGrowthState(productGrowth);

        if (datas == null) return;

        GameObject objectToPlant = (GameObject)datas[0];
        maxTime = Convert.ToInt32(datas[1]);

        ActualizePlant(objectToPlant);

        currentTime = timeSkip;
        plantFill.fillAmount = currentTime / maxTime;
    }
}