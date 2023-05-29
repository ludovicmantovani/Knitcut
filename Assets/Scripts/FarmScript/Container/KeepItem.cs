using UnityEngine;

public class KeepItem : MonoBehaviour
{
    [SerializeField] private Item item;
    [SerializeField] private Plant plant;
    [SerializeField] private bool canPickUp;

    public Item Item
    {
        get { return item; }
        set { item = value; }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && canPickUp)
        {
            PlayerController player = FindObjectOfType<PlayerController>();

            if (!player.PlayerInventory.InventoryIsFull())
            {
                player.PlayerInventory.AddItemToInventory(item);

                Destroy(gameObject);
            }
        }
    }
}