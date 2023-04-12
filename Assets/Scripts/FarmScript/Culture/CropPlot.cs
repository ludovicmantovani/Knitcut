using UnityEngine;
using UnityEngine.InputSystem;
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

        SeedGrowth seed = seedSource.GetComponent<SeedGrowth>();

        // Heal Seed
        if (seed.GetProductStates == SeedGrowth.ProductState.Sick)
            cultureManager.InteractionUI.GetComponentInChildren<Text>().text = $"Utiliser {cultureManager.PlayerInput.HealAction.GetBindingDisplayString()} pour soigner la plante";

        if (seed.GetProductStates == SeedGrowth.ProductState.Sick && cultureManager.PlayerInput.HealAction.triggered) ResumePlantGrowth(seed);

        // Hydrate Seed
        if (seed.GetProductStates == SeedGrowth.ProductState.Dehydrated)
            cultureManager.InteractionUI.GetComponentInChildren<Text>().text = $"Utiliser {cultureManager.PlayerInput.HydrateAction.GetBindingDisplayString()} pour hydrater la plante";

        if (seed.GetProductStates == SeedGrowth.ProductState.Dehydrated && cultureManager.PlayerInput.HydrateAction.triggered) ResumePlantGrowth(seed);
    }

    private void ResumePlantGrowth(SeedGrowth seed)
    {
        cultureManager.InteractionUI.GetComponentInChildren<Text>().text = $"Une graine est actuellement en production sur cette parcelle";

        seed.GetProductStates = SeedGrowth.ProductState.InGrowth;

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