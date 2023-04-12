using UnityEngine;

public class KeepItem : MonoBehaviour
{
    [SerializeField] private Item item;

    public Item Item
    {
        get { return item; }
        set { item = value; }
    }
}