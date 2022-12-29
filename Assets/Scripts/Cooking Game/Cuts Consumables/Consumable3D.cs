using UnityEngine;

public class Consumable3D : MonoBehaviour
{
    public GameObject consumableSlicedPrefab;
    public float startForce = 15f;
    public float slicedForce = 10f;

    Rigidbody2D _rigidbody;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _rigidbody.AddForce(transform.up * startForce, ForceMode2D.Impulse);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Blade"))
        {
            SliceConsumable(collision.transform.position);
        }
    }

    private void SliceConsumable(Vector3 consumable)
    {
        Vector3 direction = (consumable - transform.position).normalized;

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