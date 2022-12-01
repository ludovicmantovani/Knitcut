using UnityEngine;

public class Consumable3D : MonoBehaviour
{
    public GameObject consumableSlicedPrefab;
    public float startForce = 15f;

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

        FindObjectOfType<Cooking>().AddSlicedConsumablesToCount();

        Destroy(slicedConsumable, 3f);
        Destroy(gameObject);
    }
}