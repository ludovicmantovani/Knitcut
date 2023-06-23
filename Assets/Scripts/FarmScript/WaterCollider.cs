using UnityEngine;

public class WaterCollider : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Teleport Player
            other.GetComponent<PlayerController>().TeleportPlayer();
        }
    }
}