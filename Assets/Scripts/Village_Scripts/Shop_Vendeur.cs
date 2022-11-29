using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop_Vendeur : MonoBehaviour
{
    [SerializeField] private GameObject InteractionUI;
    [SerializeField] private GameObject ShopVendeurUI;
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
        if (talking == true)
        {

            SafeTimer = SafeTimer + Time.fixedDeltaTime;
            if (SafeTimer >= 1.5)
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
            ShopVendeurUI.SetActive(true);
            InteractionUI.SetActive(false);
            talking = true;
        }
        if (other.tag == "Player" && Input.GetKey(KeyCode.E) && talking == true && SafeTimer == 1.5)
        {
            ShopVendeurUI.SetActive(false);
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
    public void SellFruit1()
    {
        if (playerInventory.Fruit1 < 1)
        {
            Debug.Log("Not enough Fruit1");
        }
        else
        {
            playerInventory.Fruit1--;
            playerInventory.Money = playerInventory.Money + 2;
        }
    }
    public void SellFruit2()
    {
        if (playerInventory.Fruit2 < 1)
        {
            Debug.Log("Not enough Fruit2");
        }
        else
        {
            playerInventory.Fruit2--;
            playerInventory.Money = playerInventory.Money + 3;
        }
    }
    public void SellFruit3()
    {
        if (playerInventory.Fruit3 < 1)
        {
            Debug.Log("Not enough Fruit3");
        }
        else
        {
            playerInventory.Fruit3--;
            playerInventory.Money = playerInventory.Money + 5;
        }
    }
    public void SellWool1()
    {
        if (playerInventory.Wool1 < 1)
        {
            Debug.Log("Not enough Wool1");
        }
        else
        {
            playerInventory.Wool1--;
            playerInventory.Money = playerInventory.Money + 2;
        }
    }
    public void SellWool2()
    {
        if (playerInventory.Wool2 < 1)
        {
            Debug.Log("Not enough Wool2");
        }
        else
        {
            playerInventory.Wool2--;
            playerInventory.Money = playerInventory.Money + 4;
        }
    }
    public void SellWool3()
    {
        if (playerInventory.Wool3 < 1)
        {
            Debug.Log("Not enough Wool3");
        }
        else
        {
            playerInventory.Wool3--;
            playerInventory.Money = playerInventory.Money + 7;
        }
    }
}
