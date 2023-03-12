using UnityEngine;

public class ShopDetection : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
            GetComponentInParent<ShopManager>().CanUseShop = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
            GetComponentInParent<ShopManager>().CanUseShop = false;
    }
}