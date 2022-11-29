using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement_Village : MonoBehaviour
{
    [SerializeField] private Rigidbody PlayerRb;
    [SerializeField] private Vector3 Direction;
    [SerializeField] private float PlayerSpeed;
    public Shop_Fruit shop_Fruit;
    public Shop_Vendeur shop_Vendeur;
    // Start is called before the first frame update
    void Start()
    {
        PlayerRb = GetComponent<Rigidbody>();
        shop_Fruit = FindObjectOfType<Shop_Fruit>();
        shop_Vendeur = FindObjectOfType<Shop_Vendeur>();
    }

    // Update is called once per frame
    void Update()
    {
        MovementCheck();
    }
    void MovementCheck()
    {
        if (shop_Fruit.talking == false && shop_Vendeur.talking == false)
        {
            Direction.x = Input.GetAxisRaw("Horizontal");
            PlayerRb.MovePosition(PlayerRb.position + Direction * PlayerSpeed * Time.fixedDeltaTime);
        }
        else return;
        
    }
}
