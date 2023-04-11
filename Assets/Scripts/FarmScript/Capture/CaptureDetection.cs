using UnityEngine;

public class CaptureDetection : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            GetComponentInParent<CaptureManager>().CanPlaceFruit = true;

        if (other.CompareTag("Animal"))
            GetComponentInParent<CaptureManager>().CanTryToCapture = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            GetComponentInParent<CaptureManager>().CanPlaceFruit = false;

        if (other.CompareTag("Animal"))
            GetComponentInParent<CaptureManager>().CanTryToCapture = false;
    }
}