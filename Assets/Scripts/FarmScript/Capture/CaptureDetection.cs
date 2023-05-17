using UnityEngine;

public class CaptureDetection : MonoBehaviour
{
    [SerializeField] private bool zoneDetection;
    [SerializeField] private bool animalDetection;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && zoneDetection)
            GetComponentInParent<CaptureManager>().ZoneDetected = true;
            //GetComponentInParent<CaptureManager>().CanPlaceFruit = true;

        if (other.CompareTag("Animal") && animalDetection)
            GetComponentInParent<CaptureManager>().AnimalDetected = true;
            //GetComponentInParent<CaptureManager>().CanTryToCapture = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && zoneDetection)
            GetComponentInParent<CaptureManager>().ZoneDetected = false;
            //GetComponentInParent<CaptureManager>().CanPlaceFruit = false;

        if (other.CompareTag("Animal") && animalDetection)
            GetComponentInParent<CaptureManager>().AnimalDetected = false;
            //GetComponentInParent<CaptureManager>().CanTryToCapture = false;
    }
}