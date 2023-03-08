using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MinigameManager : MonoBehaviour
{
    private static List<object> dataToKeep;
    private static bool startOK = false;
    private static string sceneToLoad = "FarmScene";
    private static playerController playerController;
    private static List<GameObject> openInventories = new List<GameObject>();

    public class PlayerItem
    {
        public Item item;
        public int quantity;
    }

    private static List<PlayerItem> playerItems = new List<PlayerItem>();

    public static List<object> DataToKeep
    {
        get { return dataToKeep; }
    }

    public static bool StartOK
    {
        get { return startOK; }
        set { startOK = value; }
    }

    public static List<GameObject> OpenInventories
    {
        get { return openInventories; }
        set { openInventories = value; }
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

    private void Update()
    {
        HandlePanels();

        if (playerController == null && FindObjectOfType<playerController>())
            playerController = FindObjectOfType<playerController>();

        if (dataLoaded)
        {
            dataLoaded = false;

            listSlots = FindObjectOfType<List_Slots>();

            if (listSlots == null) return;

            CheckItemsToAdd();
        }
    }

    private void OnLevelWasLoaded()
    {
        if (SceneManager.GetActiveScene().name.Contains("Farm") && dataToKeep != null) dataLoaded = true;

        if (SceneManager.GetActiveScene().name.Contains("Cooking")) HandleItemsInInventory();
    }

    private void HandleItemsInInventory()
    {
        for (int i = 0; i < playerItems.Count; i++)
        {
            Debug.Log($"{i}. {playerItems[i].item} x{playerItems[i].quantity}");
        }
    }

    public static void AddPlayerItem(Item item, int quantity)
    {
        PlayerItem playerItem = new PlayerItem();
        playerItem.item = item;
        playerItem.quantity = quantity;

        playerItems.Add(playerItem);
    }

    #region Open Inventories

    public static void AddOpenInventory(GameObject inventory)
    {
        if (!openInventories.Contains(inventory))
            openInventories.Add(inventory);
    }

    public static void RemoveOpenInventory(GameObject inventory)
    {
        if (openInventories.Contains(inventory))
            openInventories.Remove(inventory);
    }

    private void HandlePanels()
    {
        if (playerController == null) return;

        if (openInventories.Count > 0)
        {
            playerController.CameraFC.SetActive(false);
            playerController.CanMove = false;
        }
        else
        {
            playerController.CameraFC.SetActive(true);
            playerController.CanMove = true;
        }
    }

#endregion

    #region Handle Mini Games Datas

    public static void FinalizeMG(MGType _mgType, params object[] data)
    {
        mgType = _mgType;

        dataToKeep = new List<object>();

        for (int i = 0; i < data.Length; i++)
        {
            dataToKeep.Add(data[i]);
        }
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

    #endregion

    public static void SwitchScene()
    {
        Debug.Log($"Switch scene '{SceneManager.GetActiveScene().name}' to scene '{sceneToLoad}'");
        SceneManager.LoadScene(sceneToLoad);
    }
}