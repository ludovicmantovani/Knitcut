using UnityEngine;

public class CultureArea : MonoBehaviour
{
    [SerializeField] private CultureManager cultureManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && cultureManager != null)
            cultureManager.InCultureArea = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && cultureManager != null)
            cultureManager.InCultureArea = false;
    }
}