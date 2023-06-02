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
    private ListSlots listSlots;

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
        listSlots = FindObjectOfType<ListSlots>();

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

            listSlots.SaveData();

            if (sceneToLoad.Contains("Cooking")) CheckPlayerObjects();

            MinigameManager.SwitchScene(sceneToLoad);
        }
    }

    private void CheckPlayerObjects()
    {
        MinigameManager.RecipesPossessed = playerController.PlayerRecipesInventory.GetRecipes();

        MinigameManager.ClearPlayerItems(false, true);

        for (int i = 0; i < listSlots.PlayerSlots.Length; i++)
        {
            if (listSlots.ItemsInSlots[i] != -1)
            {
                Item item = listSlots.PlayerSlots[i].GetComponentInChildren<DraggableItem>().Item;
                
                MinigameManager.AddPlayerItem(item, listSlots.QuantityStackedPlayerInventory[i]);
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
                    if (playerController.PlayerInventory.InventoryIsFull())
                    {
                        canChangeScene = false;
                        instruction = "L'inventaire est plein";
                    }
                    else if (playerController.PlayerRecipesInventory.GetRecipes().Count == 0)
                    {
                        canChangeScene = false;
                        instruction = "Vous ne possédez aucune recette";
                    }
                    else
                    {
                        instruction += " pour accéder à la cuisine";
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
                        instruction = "Il faut au minimum 2 animaux dans l'enclos";
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
                            instruction = "L'enclos associé n'est pas assez grand";
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