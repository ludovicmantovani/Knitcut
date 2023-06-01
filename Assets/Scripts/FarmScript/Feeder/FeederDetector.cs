using UnityEngine;

public class FeederDetector : MonoBehaviour
{
    [SerializeField] private GameObject feeder;

    private void OnTriggerEnter(Collider other)
    {
        HandleFeederInteraction(other, true);
    }

    private void OnTriggerExit(Collider other)
    {
        HandleFeederInteraction(other, false);
    }

    private void HandleFeederInteraction(Collider other, bool state)
    {
        if (feeder == null) return;

        if (other.CompareTag("Player"))
        {
            feeder.GetComponent<Feeder>().UpdateInteraction(state);
        }
    }
}