using UnityEngine;

public class CaptureDetection : MonoBehaviour
{
    [SerializeField] private bool zoneDetection;
    [SerializeField] private bool animalDetection;

    private void OnTriggerEnter(Collider other)
    {
        HandleCaptureDetection(other, true);
    }

    private void OnTriggerExit(Collider other)
    {
        HandleCaptureDetection(other, false);
    }

    private void HandleCaptureDetection(Collider other, bool state)
    {
        if (other.CompareTag("Player") && zoneDetection)
            GetComponentInParent<CaptureManager>().ZoneDetected = state;

        if (other.CompareTag("Animal") && animalDetection)
            GetComponentInParent<CaptureManager>().AnimalDetected = state;
    }
}