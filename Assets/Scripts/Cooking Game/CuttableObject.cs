using UnityEngine;

public class CuttableObject : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Identify if the object is being cut
        Debug.Log(collision.gameObject.tag);
        if (collision.transform.CompareTag("Cut"))
        {
            Destroy(gameObject);
        }
    }
}