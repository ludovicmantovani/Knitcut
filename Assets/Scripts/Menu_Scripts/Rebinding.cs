using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Rebinding : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private int totalInputs = 7;

    private string rebinds;

    UI_Menu menu;
    PlayerInput playerInput;

    InputActionRebindingExtensions.RebindingOperation rebindingOperation;

    public string Rebinds
    {
        get { return rebinds; }
        set { rebinds = value; }
    }

    public PlayerInput PlayerInput
    {
        get { return playerInput; }
        set { playerInput = value; }
    }

    void Start()
    {
        menu = GetComponent<UI_Menu>();
        playerInput = GetComponent<PlayerInput>();
    }

    public void Save()
    {
        rebinds = playerInput.Controls.SaveBindingOverridesAsJson();

        SaveSystem.Save(SaveSystem.SaveType.Save_UIMenu, menu);
    }

    public void Load()
    {
        KeyBinding_Data data = (KeyBinding_Data)SaveSystem.Load(SaveSystem.SaveType.Save_UIMenu, menu);

        if (data == null) return;

        rebinds = data.rebinds;

        playerInput.Controls.LoadBindingOverridesFromJson(rebinds);
    }

    public void StartRebinding()
    {
        menu.WaitingBinding.SetActive(true);

        GameObject triggerButtonObject = EventSystem.current.currentSelectedGameObject;

        KeyBindingRefs keyBindingRefs = triggerButtonObject.GetComponentInParent<KeyBindingRefs>();

        InputAction action = playerInput.FindAction(keyBindingRefs.InputActionName);

        if (action == null) return;

        action.Disable();

        int indexBinding = keyBindingRefs.InputActionBindingIndex;

        if (action == playerInput.MoveAction) indexBinding += 1;

        rebindingOperation = action.PerformInteractiveRebinding(indexBinding)
        .WithControlsExcluding("<Mouse>/leftButton").WithControlsExcluding("<Mouse>/rightButton").WithControlsExcluding("<Keyboard>/escape")
        .WithCancelingThrough("<Mouse>/leftButton").WithCancelingThrough("<Mouse>/rightButton").WithCancelingThrough("<Keyboard>/escape")
        .OnMatchWaitForAnother(0.1f)
        .OnComplete(operation => {
            RebindComplete(keyBindingRefs, action);
            Clean();
        })
        .Start();

        action.Enable();
    }

    public void RebindComplete(KeyBindingRefs keyBindingRefs, InputAction action)
    {
        keyBindingRefs.TriggerRebindButton.GetComponentInChildren<Text>().text = InputControlPath.ToHumanReadableString(
            action.controls[keyBindingRefs.InputActionBindingIndex].displayName,
            InputControlPath.HumanReadableStringOptions.OmitDevice);
    }

    private void Clean()
    {
        rebindingOperation.Dispose();

        menu.WaitingBinding.SetActive(false);
    }

    public void ResetBinding()
    {
        GameObject triggerButtonObject = EventSystem.current.currentSelectedGameObject;

        KeyBindingRefs keyBindingRefs = triggerButtonObject.GetComponentInParent<KeyBindingRefs>();

        InputAction action = playerInput.FindAction(keyBindingRefs.InputActionName);

        ResetSingleBinding(keyBindingRefs, action);
    }

    public void ResetSingleBinding(KeyBindingRefs keyBindingRefs, InputAction action)
    {
        if (keyBindingRefs == null) return;
        if (action == null) return;

        action.Disable();

        int indexBinding = keyBindingRefs.InputActionBindingIndex;

        if (action == playerInput.MoveAction) indexBinding += 1;

        action.RemoveBindingOverride(indexBinding);

        RebindComplete(keyBindingRefs, action);

        action.Enable();
    }
}