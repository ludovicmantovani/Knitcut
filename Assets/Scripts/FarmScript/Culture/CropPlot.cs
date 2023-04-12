using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CropPlot : MonoBehaviour
{
    //[SerializeField] private Item itemInCropPlot = null;
    [SerializeField] private GameObject seedSource = null;
    [SerializeField] private GameObject product = null;
    [SerializeField] private bool isCultivating;

    private CultureManager cultureManager;

    /*public Item ItemInCropPlot
    {
        get { return itemInCropPlot; }
        set { itemInCropPlot = value; }
    }*/

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

    private void Start()
    {
        cultureManager = GetComponentInParent<CultureManager>();

        isCultivating = false;
    }

    private void Update()
    {
        if (!isCultivating && seedSource != null/* && itemInCropPlot != null*/)
        {
            isCultivating = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            cultureManager.HandleCropPlot(this, true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            cultureManager.HandleCropPlot(this, false);
    }
}