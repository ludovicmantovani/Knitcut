using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CropPlot : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject seedSource = null;
    [SerializeField] private GameObject product = null;
    [SerializeField] private GameObject surfaceHighlight = null;
    [SerializeField] private bool isCultivating;
    [SerializeField] private bool manageSource;

    private CultureManager cultureManager;

    #region Getters / Setters

    public GameObject SeedSource
    {
        get { return seedSource; }
        set { seedSource = value; }
    }

    public GameObject Product
    {
        get { return product; }
        set { product = value; }
    }

    public bool IsCultivating
    {
        get { return isCultivating; }
        set { isCultivating = value; }
    }

    #endregion

    private void Start()
    {
        cultureManager = GetComponentInParent<CultureManager>();

        isCultivating = false;
        manageSource = false;
    }

    private void Update()
    {
        if (!isCultivating && seedSource != null)
        {
            isCultivating = true;
        }

        if (manageSource)
        {
            ManageSource();
        }
    }

    private void ManageSource()
    {
        if (seedSource == null) return;

        PlantGrowth plant = seedSource.GetComponent<PlantGrowth>();

        if (plant.GetProductState == PlantGrowth.ProductState.Sick && cultureManager.PlayerInput.HealAction.triggered) ResumePlantGrowth(plant);

        if (plant.GetProductState == PlantGrowth.ProductState.Dehydrated && cultureManager.PlayerInput.HydrateAction.triggered) ResumePlantGrowth(plant);
    }

    private void ResumePlantGrowth(PlantGrowth plant)
    {
        cultureManager.InteractionUI.GetComponentInChildren<TMP_Text>().text = $"Une graine est actuellement en production sur cette parcelle";

        plant.GetProductState = PlantGrowth.ProductState.InGrowth;

        manageSource = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            surfaceHighlight.SetActive(true);
            cultureManager.HandleCropPlot(this, true);

            if (seedSource != null) manageSource = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            surfaceHighlight.SetActive(false);
            cultureManager.HandleCropPlot(this, false);

            manageSource = false;
        }
    }
}