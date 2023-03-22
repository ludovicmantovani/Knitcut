using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class Shop_Fruit : MonoBehaviour
{
    public PlayerInput pI;
    [SerializeField] private GameObject InteractionUI;
    [SerializeField] private GameObject ShopFruitUI;
    float SafeTimer = 0;
    public bool talking = false;
    public PlayerInventory playerInventory;
    public PlayerController PC;

    // Start is called before the first frame update
    void Start()
    {
        pI = FindObjectOfType<PlayerInput>();
        playerInventory = FindObjectOfType<PlayerInventory>();
        PC = FindObjectOfType<PlayerController>();
        InteractionUI.GetComponentInChildren<TMP_Text>().text = "Use " + pI.InteractionAction.GetBindingDisplayString() + " to trade";
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
        if (other.tag == "Player" && pI.InteractionAction.triggered && talking == false && SafeTimer == 0)
        {
            ShopFruitUI.SetActive(true);
            InteractionUI.SetActive(false);
            talking = true;
            PC.TalkingShop = true;
        }
        if(other.tag == "Player" && pI.InteractionAction.triggered && talking == true && SafeTimer == 1.5)
        {
            ShopFruitUI.SetActive(false);
            InteractionUI.SetActive(true);
            talking = false;
            PC.TalkingShop = false;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            InteractionUI.SetActive(false);
            talking = false;
            PC.TalkingShop = false;
        }
    }
    public void BuyFruit1()
    {
        if(PC.Money < 2)
        {
            Debug.Log("Not enough money");
        }
        else
        {
            //playerInventory.Graine1++;
            PC.Money = PC.Money - 2;
        }
    }
    public void BuyFruit2()
    {
        if (PC.Money < 3)
        {
            Debug.Log("Not enough money");
        }
        else
        {
            //playerInventory.Graine2++;
            PC.Money = PC.Money - 3;
        }
    }
    public void BuyFruit3()
    {
        if (PC.Money < 5)
        {
            Debug.Log("Not enough money");
        }
        else
        {
            //playerInventory.Graine3++;
            PC.Money = PC.Money - 5;
        }
    }
}
