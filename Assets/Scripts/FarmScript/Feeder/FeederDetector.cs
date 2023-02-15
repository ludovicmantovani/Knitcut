using UnityEngine;

public class FeederDetector : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            GetComponentInParent<Feeder>().CanUseFeeder = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            GetComponentInParent<Feeder>().CanUseFeeder = false;
    }
}