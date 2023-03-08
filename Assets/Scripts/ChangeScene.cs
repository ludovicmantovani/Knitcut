using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChangeScene : MonoBehaviour
{
    [SerializeField] private string sceneToLoad = "";
    [SerializeField] private GameObject interactionPanel;
    [SerializeField] private bool isMinigame;

    private PlayerInput pI;
    private Shop_Enclos se;
    private playerController PC;
    private List_Slots LS;

    bool canChangeScene = false;
    bool showInstruction = false;

    void Awake()
    {
        pI = GetComponent<PlayerInput>();
        se = FindObjectOfType<Shop_Enclos>();
        PC = FindObjectOfType<playerController>();
        LS = FindObjectOfType<List_Slots>();
    }

    void Update()
    {
        ShowInteraction();
        HandleChangeScene();        
    }

    private void HandleChangeScene()
    {
        if (canChangeScene && sceneToLoad != "" && pI.actions["Intercation_Environnements"].triggered)
        {
            canChangeScene = false;

            if (SceneManager.GetActiveScene().name.Contains("Farm"))
            {
                FindObjectOfType<playerController>().SavePlayerPos();
            }

            if (SceneManager.GetActiveScene().name.Contains("Village"))
            {
                se.SaveEncloslevel();
                PC.farm = false;
            }

            LS.SaveData();

            CheckPlayerInventory();

            SceneManager.LoadScene(sceneToLoad);
        }
    }

    private void CheckPlayerInventory()
    {
        for (int i = 0; i < LS.PlayerSlots.Length; i++)
        {
            if (LS.ItemsInSlots[i] != -1)
            {
                Item item = LS.PlayerSlots[i].GetComponentInChildren<DraggableItem>().Item;

                if (item.itemType == ItemType.Consumable)
                {
                    MinigameManager.AddPlayerItem(item, LS.QuantityStackedPlayerInventory[i]);
                }
            }
        }
    }

    private void ShowInteraction()
    {
        if (showInstruction && sceneToLoad != "")
        {
            showInstruction = false;

            string instruction = "Use " + pI.actions["Intercation_Environnements"].GetBindingDisplayString();

            if (sceneToLoad.Contains("Village"))
            {
                instruction += " to go to Village";
            }

            if (sceneToLoad.Contains("Farm"))
            {
                instruction += " to return to Farm";
            }

            if (isMinigame)
            {
                if (sceneToLoad.Contains("Cooking"))
                {
                    instruction += " to enter the kitchen";
                }
                else if (sceneToLoad.Contains("Recognition"))
                {
                    instruction += " to use the sewing workshop";
                }
                else if (sceneToLoad.Contains("Flower"))
                {
                    instruction += " to try to breed the animals";
                }
            }

            interactionPanel.GetComponentInChildren<Text>().text = instruction;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) canChangeScene = true;

        interactionPanel.SetActive(true);
        showInstruction = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) canChangeScene = false;

        interactionPanel.SetActive(false);
        showInstruction = false;
    }
}