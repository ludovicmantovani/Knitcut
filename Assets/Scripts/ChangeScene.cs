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
    private AnimalPenManager animalPenManager;
    private List_Slots LS;

    private bool canChangeScene = false;
    private bool showInstruction = false;

    #region Getters / Setters

    public GameObject InteractionPanel
    {
        get { return interactionPanel; }
        set { interactionPanel = value; }
    }

    public bool CanChangeScene
    {
        get { return canChangeScene; }
        set { canChangeScene = value; }
    }

    public bool ShowInstruction
    {
        get { return showInstruction; }
        set { showInstruction = value; }
    }

    #endregion

    void Awake()
    {
        playerInput = FindObjectOfType<PlayerInput>();
        playerController = FindObjectOfType<PlayerController>();
        animalPenManager = FindObjectOfType<AnimalPenManager>();
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

            if (SceneManager.GetActiveScene().name.Contains("Village"))
            {
                playerController.InFarm = false;
            }

            playerController.SavePlayerPositionInScene();

            LS.SaveData();

            CheckPlayerInventory();

            MinigameManager.SwitchScene(sceneToLoad);
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
                instruction += " pour aller � la ferme";
            }

            if (isMinigame)
            {
                if (sceneToLoad.Contains("Cooking"))
                {
                    if (playerController.PlayerInventory.InventoryIsFull())
                    {
                        canChangeScene = false;
                        instruction = "L'inventaire est plein";
                    }
                    else if (playerController.PlayerRecipesInventory.GetRecipes().Count == 0)
                    {
                        canChangeScene = false;
                        instruction = "Vous ne poss�dez aucune recette";
                    }
                    else
                    {
                        instruction += " pour acc�der � la cuisine";

                        MinigameManager.RecipesPossessed = playerController.PlayerRecipesInventory.GetRecipes();
                    }
                }
                else if (sceneToLoad.Contains("Recognition"))
                {
                    instruction += " pour utiliser l'atelier de couture";
                }
                else if (sceneToLoad.Contains("Flower"))
                {
                    if (GetAnimalsAdultsCount() < 2)
                    {
                        canChangeScene = false;
                        instruction = "Il n'y a aucun animal dans cet enclos";
                    }
                    else
                    {
                        GetAnimalType();

                        if (animalPenManager.CheckAnimalPenRestrictions(MinigameManager.AnimalTypeToKeep, true))
                        {
                            instruction += " pour reproduire les animaux";
                        }
                        else
                        {
                            canChangeScene = false;
                            instruction = "L'enclos associ� n'est pas assez grand";
                        }
                    }
                }
            }

            interactionPanel.GetComponentInChildren<Text>().text = instruction;
        }
    }

    private int GetAnimalsAdultsCount()
    {
        int count = 0;

        Transform currentAnimalPen = transform.parent.parent;

        for (int i = 0; i < currentAnimalPen.childCount; i++)
        {
            if (currentAnimalPen.GetChild(i).CompareTag("Animal"))
            {
                AnimalStates animalStates = currentAnimalPen.GetChild(i).GetComponent<AnimalStates>();

                if (!animalStates.IsChild) count++;
            }
        }

        return count;
    }

    private void GetAnimalType()
    {
        Transform currentAnimalPen = transform.parent.parent;

        string animalTypeInName = currentAnimalPen.name.Substring(11);

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
}