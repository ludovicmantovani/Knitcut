using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MinigameManager : MonoBehaviour
{
    private static List<object> dataToKeep;
    private static bool startOK = false;
    private static string sceneToLoad = "FarmScene";

    public static List<object> DataToKeep
    {
        get { return dataToKeep; }
    }

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
        Capture
    }

    private static MGType mgType;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if (dataLoaded)
        {
            dataLoaded = false;

            listSlots = FindObjectOfType<List_Slots>();

            if (listSlots == null) return;

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

    public static void FinalizeMG(MGType _mgType, params object[] data)
    {
        mgType = _mgType;

        dataToKeep = new List<object>();

        for (int i = 0; i < data.Length; i++)
        {
            dataToKeep.Add(data[i]);
        }
    }

    private void OnLevelWasLoaded()
    {
        if (SceneManager.GetActiveScene().name.Contains("Farm") && dataToKeep != null)
            dataLoaded = true;
    }

    private void CheckItemsToAdd()
    {
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
            default:
                Debug.Log($"Error, data mgType");
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

    public static void SwitchScene()
    {
        Debug.Log($"Switch to scene '{sceneToLoad}'");
        SceneManager.LoadScene(sceneToLoad);
    }
}