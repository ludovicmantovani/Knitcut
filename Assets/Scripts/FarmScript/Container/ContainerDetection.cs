using UnityEngine;

public class ContainerDetection : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            GetComponentInParent<Container>().CanUseContainer = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            GetComponentInParent<Container>().CanUseContainer = false;
    }
}