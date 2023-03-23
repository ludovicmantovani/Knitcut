using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    private PlayerControls controls;

    private InputAction moveAction;

    private InputAction quickSaveAction;
    private InputAction quickLoadAction;

    private InputAction interactionAction;
    private InputAction healAction;
    private InputAction hydrateAction;

    private InputAction inventoryAction;
    private InputAction recipesInventoryAction;

    #region Getters

    public InputAction MoveAction
    {
        get { return moveAction; }
    }

    public InputAction QuickSaveAction
    {
        get { return quickSaveAction; }
    }

    public InputAction QuickLoadAction
    {
        get { return quickLoadAction; }
    }

    public InputAction InteractionAction
    {
        get { return interactionAction; }
    }

    public InputAction HealAction
    {
        get { return healAction; }
    }

    public InputAction HydrateAction
    {
        get { return hydrateAction; }
    }

    public InputAction InventoryAction
    {
        get { return inventoryAction; }
    }

    public InputAction RecipesInventoryAction
    {
        get { return recipesInventoryAction; }
    }

    #endregion

    private void Awake()
    {
        controls = new PlayerControls();

        moveAction = controls.FindAction("Move");

        quickSaveAction = controls.FindAction("QuickSave");
        quickLoadAction = controls.FindAction("QuickLoad");

        interactionAction = controls.FindAction("Intercation");
        healAction = controls.FindAction("Heal");
        hydrateAction = controls.FindAction("Hydrate");

        inventoryAction = controls.FindAction("Inventory");
        recipesInventoryAction = controls.FindAction("RecipesInventory");
    }

    private void OnEnable()
    {
        controls.Player.Enable();
    }

    private void OnDisable()
    {
        controls.Player.Disable();
    }
}   
