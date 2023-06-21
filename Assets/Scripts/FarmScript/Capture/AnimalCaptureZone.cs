using UnityEngine;

public class AnimalCaptureZone : MonoBehaviour
{
    [SerializeField] private bool nearDetection;
    [SerializeField] private bool behindDetection;
    
    private void OnTriggerEnter(Collider other)
    {
        HandlePlayerDetection(other, true);
    }

    private void OnTriggerExit(Collider other)
    {
        HandlePlayerDetection(other, false);
    }

    private void HandlePlayerDetection(Collider other, bool state)
    {
        if (other.CompareTag("Player"))
        {
            AnimalAI animalAI = GetComponentInParent<AnimalAI>();
            
            if (nearDetection) animalAI.PlayerIsNear = state;
            
            if (behindDetection) animalAI.PlayerIsBehind = state;
        }
    }
}