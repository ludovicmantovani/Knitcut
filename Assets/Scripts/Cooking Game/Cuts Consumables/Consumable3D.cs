using UnityEngine;

public class Consumable3D : MonoBehaviour
{
    [SerializeField] private GameObject consumableSlicedPrefab;
    [SerializeField] private float propulsionForce = 15f;
    [SerializeField] private float slicedForce = 10f;

    Rigidbody2D _rigidbody;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _rigidbody.AddForce(transform.up * propulsionForce, ForceMode2D.Impulse);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Blade"))
        {
            SliceConsumable(collision.transform.position);
        }
    }

    private void SliceConsumable(Vector3 bladePosition)
    {
        Vector3 direction = (bladePosition - transform.position).normalized;

        Quaternion rotation = Quaternion.LookRotation(direction);

        GameObject slicedConsumable = Instantiate(consumableSlicedPrefab, transform.position, rotation);

        for (int i = 0; i < slicedConsumable.transform.childCount; i++)
        {
            GameObject slicedPart = slicedConsumable.transform.GetChild(i).gameObject;

            slicedPart.GetComponent<Rigidbody>().AddForce(direction * slicedForce, ForceMode.Impulse);
        }

        FindObjectOfType<Cooking>().AddSlicedConsumablesToCount();

        Destroy(slicedConsumable, 3f);
        Destroy(gameObject);
    }
}