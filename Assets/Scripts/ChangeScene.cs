using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChangeScene : MonoBehaviour
{
    [SerializeField] private string sceneToLoad = "";
    [SerializeField] private GameObject interactionPanel;
    [SerializeField] private bool isMinigame;

    private PlayerInput playerInput;
    private Shop_Enclos shopEnclos;
    private PlayerController playerController;
    private List_Slots LS;

    bool canChangeScene = false;
    bool showInstruction = false;

    void Awake()
    {
        playerInput = FindObjectOfType<PlayerInput>();
        shopEnclos = FindObjectOfType<Shop_Enclos>();
        playerController = FindObjectOfType<PlayerController>();
        LS = FindObjectOfType<List_Slots>();
    }

    void Update()
    {
        ShowInteraction();
        HandleChangeScene();        
    }

    private void HandleChangeScene()
    {
        if (canChangeScene && sceneToLoad != "" && playerInput.InteractionAction.triggered)
        {
            canChangeScene = false;

            playerController.SavePlayerPositionInScene();

            if (SceneManager.GetActiveScene().name.Contains("Village"))
            {
                shopEnclos.SaveEncloslevel();
                playerController.InFarm = false;
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
                
                MinigameManager.AddPlayerItem(item, LS.QuantityStackedPlayerInventory[i]);
            }
        }
    }

    private void ShowInteraction()
    {
        if (showInstruction && sceneToLoad != "")
        {
            showInstruction = false;

            string instruction = "Use " + playerInput.InteractionAction.GetBindingDisplayString();

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