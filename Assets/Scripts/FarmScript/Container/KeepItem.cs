using Gameplay.UI.Quests;
using UnityEngine;

public class KeepItem : MonoBehaviour
{
    [SerializeField] private Item item;
    [SerializeField] private bool canPickUp;

    [Header("Quest")]
    [SerializeField] private string questCompletionRecupLaineAmelioration = "RecupLaine";

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
                QuestManager qm = QuestManager.Instance;
                if (qm && item.itemName == "Wool")
                {
                    qm.CompleteObjective(questCompletionRecupLaineAmelioration);
                }
                Destroy(gameObject);
            }
        }
    }
}