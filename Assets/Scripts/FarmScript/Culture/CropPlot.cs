using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CropPlot : MonoBehaviour
{
    [SerializeField] private Item itemInCropPlot = null;
    [SerializeField] private GameObject product = null;
    [SerializeField] private float timeOfCultivation = 20f;

    private CultureManager cultureManager;
    private bool isCultivating;

    public Item ItemInCropPlot
    {
        get { return itemInCropPlot; }
        set { itemInCropPlot = value; }
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
        if (!isCultivating && itemInCropPlot != null)
        {
            isCultivating = true;

            StartCoroutine(Cultivating());
        }
    }

    private IEnumerator Cultivating()
    {
        Debug.Log($"Cultivating...");

        yield return new WaitForSeconds(timeOfCultivation);

        Debug.Log($"Cultivation finished...");
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