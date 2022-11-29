using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop_Fruit : MonoBehaviour
{
    [SerializeField] private GameObject InteractionUI;
    [SerializeField] private GameObject ShopFruitUI;
    float SafeTimer = 0;
    public bool talking = false;
    public PlayerInventory playerInventory;

    // Start is called before the first frame update
    void Awake()
    {
        playerInventory = FindObjectOfType<PlayerInventory>();
    }

    // Update is called once per frame
    void Update()
    {
        TimerVerification();
    }
    void TimerVerification()
    {
        if(talking == true)
        {

            SafeTimer = SafeTimer + Time.fixedDeltaTime;
            if(SafeTimer >= 1.5)
            {
                SafeTimer = 1.5f;
            }
        }
        if (talking == false)
        {

            SafeTimer = SafeTimer - Time.fixedDeltaTime;
            if (SafeTimer <= 0)
            {
                SafeTimer = 0;
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            InteractionUI.SetActive(true);
        }

    }
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player" && Input.GetKey(KeyCode.E) && talking == false && SafeTimer == 0)
        {
            ShopFruitUI.SetActive(true);
            InteractionUI.SetActive(false);
            talking = true;
        }
        if(other.tag == "Player" && Input.GetKey(KeyCode.E) && talking == true && SafeTimer == 1.5)
        {
            ShopFruitUI.SetActive(false);
            InteractionUI.SetActive(true);
            talking = false;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            InteractionUI.SetActive(false);
            talking = false;
        }
    }
    public void BuyFruit1()
    {
        if(playerInventory.Money < 2)
        {
            Debug.Log("Not enough money");
        }
        else
        {
            playerInventory.Graine1++;
            playerInventory.Money = playerInventory.Money - 2;
        }
    }
    public void BuyFruit2()
    {
        if (playerInventory.Money < 3)
        {
            Debug.Log("Not enough money");
        }
        else
        {
            playerInventory.Graine2++;
            playerInventory.Money = playerInventory.Money - 3;
        }
    }
    public void BuyFruit3()
    {
        if (playerInventory.Money < 5)
        {
            Debug.Log("Not enough money");
        }
        else
        {
            playerInventory.Graine3++;
            playerInventory.Money = playerInventory.Money - 5;
        }
    }
}
