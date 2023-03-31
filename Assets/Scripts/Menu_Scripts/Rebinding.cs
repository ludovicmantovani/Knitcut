using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Rebinding : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private int totalInputs = 7;
    [SerializeField] private List<InputAction> defaultInputs;

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

        SaveDefaultControls();
    }

    private void SaveDefaultControls()
    {
        defaultInputs = new List<InputAction>(totalInputs);

        defaultInputs.Add(playerInput.MoveAction);
        defaultInputs.Add(playerInput.QuickSaveAction);
        defaultInputs.Add(playerInput.QuickLoadAction);
        defaultInputs.Add(playerInput.InteractionAction);
        defaultInputs.Add(playerInput.HealAction);
        defaultInputs.Add(playerInput.HydrateAction);
        defaultInputs.Add(playerInput.InventoryAction);
    }

    public void Save()
    {
        rebinds = playerInput.Controls.SaveBindingOverridesAsJson();

        if (rebinds == string.Empty) return;

        Debug.Log($"Saving... : {rebinds}");

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
}