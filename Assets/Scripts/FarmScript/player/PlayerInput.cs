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

    private InputAction deleteSavesAction;

    #region Getters

    public PlayerControls Controls
    {
        get { return controls; }
    }

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

    public InputAction DeleteSavesAction
    {
        get { return deleteSavesAction; }
    }

    #endregion

    private void Awake()
    {
        controls = new PlayerControls();

        moveAction = controls.FindAction("Move");

        quickSaveAction = controls.FindAction("QuickSave");
        quickLoadAction = controls.FindAction("QuickLoad");

        interactionAction = controls.FindAction("Interaction");
        healAction = controls.FindAction("Heal");
        hydrateAction = controls.FindAction("Hydrate");

        inventoryAction = controls.FindAction("Inventory");
        recipesInventoryAction = controls.FindAction("RecipesInventory");

        deleteSavesAction = controls.FindAction("DeleteSaves");

        LoadInputs(); 
    }

    private void OnEnable()
    {
        controls.Player.Enable();
        controls.Admin.Enable();
    }

    private void OnDisable()
    {
        controls.Player.Disable();
        controls.Admin.Disable();
    }

    public InputAction FindAction(string actionName)
    {
        return controls.FindAction(actionName);
    }

    private void LoadInputs()
    {
        KeyBinding_Data data = (KeyBinding_Data)SaveSystem.Load(SaveSystem.SaveType.Save_UIMenu);

        if (data == null || data.rebinds == null || data.rebinds == string.Empty) return;

        controls.LoadBindingOverridesFromJson(data.rebinds);
    }
}   
