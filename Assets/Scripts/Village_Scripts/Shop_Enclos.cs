using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Shop_Enclos : MonoBehaviour
{
    [SerializeField] private GameObject InteractionUI;
    [SerializeField] private GameObject ShopEnclosUI;
    float SafeTimer = 0;
    public bool talking = false;
    public PlayerInventory playerInventory;
    public playerController PC;
    public int levelEnclo1 = 0;
    public int levelEnclo2 = 0;
    public int levelEnclo3 = 0;
    public TMP_Text prix1;
    public TMP_Text prix2;
    public TMP_Text prix3;


    // Start is called before the first frame update
    void Awake()
    {
        playerInventory = FindObjectOfType<PlayerInventory>();
        PC = FindObjectOfType<playerController>();
    }

    // Update is called once per frame
    void Update()
    {
        TimerVerification();
        ActualisationPrix();
        if (Input.GetKeyDown(KeyCode.F5))
        {
            SaveEncloslevel();
        }
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
            ShopEnclosUI.SetActive(true);
            InteractionUI.SetActive(false);
            talking = true;
            PC.TalkingShop = true;
        }
        if (other.tag == "Player" && Input.GetKey(KeyCode.E) && talking == true && SafeTimer == 1.5)
        {
            ShopEnclosUI.SetActive(false);
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
    public void BuyEnclos1()
    {
        
        if (playerInventory.Money < 10 && levelEnclo1 == 0)
        {
            Debug.Log("Not enough money");
        }
        if (playerInventory.Money < 20 && levelEnclo1 == 1)
        {
            Debug.Log("Not enough money");
        }
        if (playerInventory.Money < 50 && levelEnclo1 == 2)
        {
            Debug.Log("Not enough money");
        }
        if (playerInventory.Money >= 10 && levelEnclo1 == 0)
        {
            levelEnclo1 = 1;
            playerInventory.Money = playerInventory.Money - 10;
            return;
        }
        if (playerInventory.Money >= 20 && levelEnclo1 == 1)
        {
            levelEnclo1 = 2;
            playerInventory.Money = playerInventory.Money - 20;
            return;
        }
        if (playerInventory.Money >= 50 && levelEnclo1 == 2)
        {
            levelEnclo1 = 3;
            playerInventory.Money = playerInventory.Money - 50;
            return;
        }
        if(levelEnclo1 == 3)
        {
            Debug.Log("Max level Reach");
        }
    }
    public void BuyEnclos2()
    {
        if (playerInventory.Money < 25 && levelEnclo2 == 0)
        {
            Debug.Log("Not enough money");
        }
        if (playerInventory.Money < 40 && levelEnclo2 == 1)
        {
            Debug.Log("Not enough money");
        }
        if (playerInventory.Money < 75 && levelEnclo2 == 2)
        {
            Debug.Log("Not enough money");
        }
        if (playerInventory.Money >= 25 && levelEnclo2 == 0)
        {
            levelEnclo2 = 1;
            playerInventory.Money = playerInventory.Money - 25;
            return;
        }
        if (playerInventory.Money >= 40 && levelEnclo2 == 1)
        {
            levelEnclo2 = 2;
            playerInventory.Money = playerInventory.Money - 40;
            return;
        }
        if (playerInventory.Money >= 75 && levelEnclo2 == 2)
        {
            levelEnclo2 = 3;
            playerInventory.Money = playerInventory.Money - 75;
            return;
        }
        if (levelEnclo2 == 3)
        {
            Debug.Log("Max level Reach");
        }

    }
    public void BuyEnclos3()
    {
        if (playerInventory.Money < 40 && levelEnclo3 == 0)
        {
            Debug.Log("Not enough money");
        }
        if (playerInventory.Money < 70 && levelEnclo3 == 1)
        {
            Debug.Log("Not enough money");
        }
        if (playerInventory.Money < 90 && levelEnclo3 == 2)
        {
            Debug.Log("Not enough money");
        }
        if (playerInventory.Money >= 40 && levelEnclo3 == 0)
        {
            levelEnclo3 = 1;
            playerInventory.Money = playerInventory.Money - 40;
            return;
        }
        if (playerInventory.Money >= 70 && levelEnclo3 == 1)
        {
            levelEnclo3 = 2;
            playerInventory.Money = playerInventory.Money - 70;
            return;
        }
        if (playerInventory.Money >= 90 && levelEnclo3 == 2)
        {
            levelEnclo3 = 3;
            playerInventory.Money = playerInventory.Money - 90;
            return;
        }
        if (levelEnclo3 == 3)
        {
            Debug.Log("Max level Reach");
        }
    }
    void ActualisationPrix()
    {
        if(levelEnclo1 == 0)
        {
            prix1.text = "10";
        }
        if (levelEnclo1 == 1)
        {
            prix1.text = "20";
        }
        if (levelEnclo1 == 2)
        {
            prix1.text = "50";
        }
        if (levelEnclo2 == 0)
        {
            prix2.text = "25";
        }
        if (levelEnclo2 == 1)
        {
            prix2.text = "40";
        }
        if (levelEnclo2 == 2)
        {
            prix2.text = "75";
        }
        if (levelEnclo3 == 0)
        {
            prix3.text = "40";
        }
        if (levelEnclo3 == 1)
        {
            prix3.text = "70";
        }
        if (levelEnclo3 == 2)
        {
            prix3.text = "90";
        }
    }

    public void SaveEncloslevel()
    {
        
        SaveSystem.SaveEnclosLevel(this);
        

    }
    
}
