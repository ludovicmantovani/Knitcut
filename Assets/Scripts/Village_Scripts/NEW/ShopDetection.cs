using UnityEngine;

public class ShopDetection : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        HandleShopUse(other, true);
    }

    private void OnTriggerExit(Collider other)
    {
        HandleShopUse(other, false);
    }

    private void HandleShopUse(Collider other, bool shopUse)
    {
        if (other.tag == "Player")
        {
            GetComponentInParent<ShopsManager>().CanUseShop = shopUse;
            GetComponentInParent<ShopsManager>().InteractionUI.SetActive(shopUse);
        }
    }
}