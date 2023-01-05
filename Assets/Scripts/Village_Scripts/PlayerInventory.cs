using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerInventory : MonoBehaviour
{
    public int Money = 10;
    public int Fruit1 = 4;
    public int Fruit2 = 2;
    public int Fruit3 = 1;

    public int Graine1 = 0;
    public int Graine2 = 0;
    public int Graine3 = 0;

    public int Wool1 = 3;
    public int Wool2 = 1;
    public int Wool3 = 2;

    public TMP_Text Quantite_Money;
    public TMP_Text Quantite_Fruit1;
    public TMP_Text Quantite_Fruit2;
    public TMP_Text Quantite_Fruit3;
    public TMP_Text Quantite_Wool1;
    public TMP_Text Quantite_Wool2;
    public TMP_Text Quantite_Wool3;
    public TMP_Text Quantite_Seed1;
    public TMP_Text Quantite_Seed2;
    public TMP_Text Quantite_Seed3;

    public bool InventoryOpen = false;
    public bool Open = false;
    public GameObject Inventory;
    public float secur = 0.1f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Open == true)
        {
            secur = secur - Time.deltaTime;
        }
        AffichageInventaire();
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            if (Input.GetKeyDown(KeyCode.F5))
            {
                SavePlayerInv();
            }
            if (Input.GetKeyDown(KeyCode.F9))
            {
                LoadPlayerInv();
            }
        }
        


    }
    void SavePlayerInv()
    {
        SaveSystem.SavePlayerInventory(this);
    }
    void LoadPlayerInv()
    {
        Player_Data data = SaveSystem.LoadPlayerInventory();

        Money = data.Money;
        Fruit1 = data.Fruit1;
        Fruit2 = data.Fruit2;
        Fruit3 = data.Fruit3;
        Wool1 = data.Wool1;
        Wool2 = data.Wool2;
        Wool3 = data.Wool3;
    }
    void AffichageInventaire()
    {
        if (Input.GetKeyDown(KeyCode.I) )
        {
            Open = true;
        }
        if (InventoryOpen == false && secur <= 0)
        {
            Inventory.SetActive(true);
            InventoryOpen = true;
            secur = 0.1f;
            Open = false;
        }
        else if (InventoryOpen == true && secur <= 0)
        {
            Inventory.SetActive(false);
            InventoryOpen = false;
            secur = 0.1f;
            Open = false;
        }

        Quantite_Money.text = Money.ToString();
        Quantite_Fruit1.text = Fruit1.ToString();
        Quantite_Fruit2.text = Fruit2.ToString();
        Quantite_Fruit3.text = Fruit3.ToString();
        Quantite_Wool1.text = Wool1.ToString();
        Quantite_Wool2.text = Wool2.ToString();
        Quantite_Wool3.text = Wool3.ToString();
        Quantite_Seed1.text = Graine1.ToString();
        Quantite_Seed2.text = Graine2.ToString();
        Quantite_Seed3.text = Graine3.ToString();


    }
}
