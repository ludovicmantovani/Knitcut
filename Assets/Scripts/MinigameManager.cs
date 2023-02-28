using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MinigameManager : MonoBehaviour
{
    [SerializeField] private static List<object> dataToKeep;
    [SerializeField] private static bool startOK;

    public static bool StartOK
    {
        get { return startOK; }
        set { startOK = value; }
    }

    private List_Slots listSlots;

    bool dataLoaded;

    public enum MGType
    {
        Cooking,
        Recognition,
        Breeding,
        Capture,
        NULL
    }

    public static MGType mgType;

    private void Start()
    {
        /*mgType = MGType.NULL;

        dataToKeep = new List<object>();

        dataLoaded = false;*/

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if (dataLoaded)
        {
            dataLoaded = false;

            listSlots = FindObjectOfType<List_Slots>();

            CheckItemsToAdd();
        }

        HandleCursor();
    }

    private void HandleCursor()
    {
        if (SceneManager.GetActiveScene().buildIndex == 2 || SceneManager.GetActiveScene().buildIndex == 3)
        {
            playerController player = FindObjectOfType<playerController>();

            if (player.pI.actions["Cursor"].IsPressed())
            {
                Cursor.lockState = CursorLockMode.None;
                player.CameraFC.SetActive(false);
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                player.CameraFC.SetActive(true);
            }
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public static void AddData(List<object> data)
    {
        //dataToKeep = new List<object>();
        dataToKeep = data;
    }

    private void OnLevelWasLoaded(int level)
    {
        if (SceneManager.GetActiveScene().name.Contains("Farm") /*&& mgType != MGType.NULL */&& dataToKeep != null)
            dataLoaded = true;
    }

    private void CheckItemsToAdd()
    {
        /*Debug.Log($"MGTYPE : {mgType}");
        for (int i = 0; i < dataToKeep.Count; i++)
        {
            Debug.Log($"{i}. {dataToKeep[i]}");
        }*/

        if (listSlots == null) return;

        switch (mgType)
        {
            case MGType.Cooking:
                HandleCookingData();
                break;
            case MGType.Recognition:
                HandleRecognitionData();
                break;
            case MGType.Breeding:
                HandleBreedingData();
                break;
            case MGType.Capture:
                HandleCaptureData();
                break;
            case MGType.NULL:
                Debug.Log($"Error, data mgType NULL");
                break;
            default:
                break;
        }

        dataLoaded = false;

        dataToKeep = new List<object>();
    }

    private void HandleCookingData()
    {
        PlayerInventoryUI inventory = FindObjectOfType<PlayerInventoryUI>();

        if (inventory == null) return;

        GameObject itemUI = inventory.CreateItemUI();
        itemUI.GetComponent<DraggableItem>().quantityStacked = 1;

        itemUI.GetComponent<DraggableItem>().Item = (Item)listSlots.stuffs[3 + Convert.ToInt32(dataToKeep[3])];

        itemUI.GetComponent<DraggableItem>().Item.itemName = (string)dataToKeep[0];
        itemUI.GetComponent<DraggableItem>().Item.itemDescription = (string)dataToKeep[1];
        itemUI.GetComponent<DraggableItem>().Item.itemValue = (float)dataToKeep[2];

        /*Debug.Log(itemUI);
        Debug.Log(itemUI.GetComponent<DraggableItem>().Item);
        Debug.Log((Item)listSlots.stuffs[3 + Convert.ToInt32(dataToKeep[3])]);*/
    }

    private void HandleRecognitionData()
    {
        listSlots.UpdateMoney(listSlots.pC.money + Convert.ToInt32(dataToKeep[0]));
    }

    private void HandleBreedingData()
    {
        bool breedingSuccess = Convert.ToBoolean(dataToKeep[0]);

        Debug.Log($"breeding success ? {breedingSuccess}");
    }

    private void HandleCaptureData()
    {

    }
}