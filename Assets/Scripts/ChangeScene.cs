using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;
using System;
using System.Collections.Generic;

public class ChangeScene : MonoBehaviour
{
    [SerializeField] private string sceneToLoad = "";
    [SerializeField] private GameObject interactionPanel;
    [SerializeField] private bool isMinigame;

    private PlayerInput playerInput;
    private PlayerController playerController;
    private List_Slots LS;

    private bool canChangeScene = false;
    private bool showInstruction = false;

    public bool ShowInstruction
    {
        get { return showInstruction; }
        set { showInstruction = value; }
    }

    void Awake()
    {
        playerInput = FindObjectOfType<PlayerInput>();
        playerController = FindObjectOfType<PlayerController>();
        LS = FindObjectOfType<List_Slots>();

        interactionPanel.SetActive(false);
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

            string instruction = "Utiliser " + playerInput.InteractionAction.GetBindingDisplayString();

            if (sceneToLoad.Contains("Village"))
            {
                instruction += " pour aller au village marchand";
            }

            if (sceneToLoad.Contains("Farm"))
            {
                instruction += " pour aller à la ferme";
            }

            if (isMinigame)
            {
                if (sceneToLoad.Contains("Cooking"))
                {
                    instruction += " pour accéder à la cuisine";

                    MinigameManager.RecipesPossessed = playerController.PlayerRecipesInventory.GetRecipes();
                }
                else if (sceneToLoad.Contains("Recognition"))
                {
                    instruction += " pour utiliser l'atelier de couture";
                }
                else if (sceneToLoad.Contains("Flower"))
                {
                    GetAnimalType();

                    instruction += " pour reproduire les animaux";
                }
            }

            interactionPanel.GetComponentInChildren<Text>().text = instruction;
        }
    }

    private void GetAnimalType()
    {
        string animalTypeInName = transform.parent.name.Substring(11);

        List<AnimalType> animalTypeList = Enum.GetValues(typeof(AnimalType)).Cast<AnimalType>().ToList();

        for (int i = 0; i < animalTypeList.Count; i++)
        {
            if (animalTypeList[i].ToString() == animalTypeInName)
            {
                AnimalType animalType = animalTypeList[i];

                MinigameManager.AnimalTypeToKeep = animalType;
            }
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