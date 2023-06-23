using UnityEngine;

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
            CaptureManager.instance.ZoneDetected = state;

        if (animalDetection && other.CompareTag("Animal"))
        {
            AnimalAI animal = CaptureManager.instance.WildAnimalAttracted;
            
            if (animal == null) return;
            
            if (other.gameObject == animal.gameObject)
                CaptureManager.instance.AnimalDetected = state;
        }

        if (other.CompareTag("Player") && playerHiddenDetection)
            CaptureManager.instance.PlayerIsHidden = state;
    }
}