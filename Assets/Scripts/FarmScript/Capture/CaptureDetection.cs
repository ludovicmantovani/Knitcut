using UnityEngine;
using UnityEngine.Serialization;

public class CaptureDetection : MonoBehaviour
{
    [SerializeField] private bool zoneDetection;
    [SerializeField] private bool animalDetection;
    [SerializeField] private bool playerHiddenDetection;

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
            GetComponentInParent<CaptureV2>().ZoneDetected = state;

        if (other.CompareTag("Animal") && animalDetection)
            GetComponentInParent<CaptureV2>().AnimalDetected = state;

        if (other.CompareTag("Player") && playerHiddenDetection)
            GetComponentInParent<CaptureV2>().PlayerIsHidden = state;
    }
}