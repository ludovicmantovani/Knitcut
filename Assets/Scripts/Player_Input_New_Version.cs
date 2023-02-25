// GENERATED AUTOMATICALLY FROM 'Assets/Scripts/Player_Input_New_Version.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @Player_Input_New_Version : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @Player_Input_New_Version()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""Player_Input_New_Version"",
    ""maps"": [
        {
            ""name"": ""Actions"",
            ""id"": ""29ca77e7-5a8f-486d-af97-037da8a13e34"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""7cecbf22-58a6-49ee-947a-800f55e9b56c"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""CameraLook"",
                    ""type"": ""PassThrough"",
                    ""id"": ""a6670b77-6096-44ac-8ce9-b05c2319fcb9"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""QuickSave"",
                    ""type"": ""Button"",
                    ""id"": ""95dcea50-8535-4c31-b156-fd5c855b899e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""QuickLoad"",
                    ""type"": ""Button"",
                    ""id"": ""39ad0cac-e21c-4190-86ca-5fa1e9b97ed7"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Intercation_Environnements"",
                    ""type"": ""Button"",
                    ""id"": ""9d6bc202-ba0e-407e-8035-a86912b6cffc"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Heal_plant"",
                    ""type"": ""Button"",
                    ""id"": ""3630e42c-d137-46eb-b4e2-c43d7277b72b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Hydrate_plant"",
                    ""type"": ""Button"",
                    ""id"": ""05251346-08ee-4480-8604-8e2e317cd163"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Inventory"",
                    ""type"": ""Button"",
                    ""id"": ""4d4a2371-3862-4c72-ac46-6b268d9e2350"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""WASD"",
                    ""id"": ""c3af400c-39a6-40ba-a038-a402df32830b"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""3d6cd6bf-c3f8-433e-a019-91a2811dfcd0"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""ec662bbb-81d0-4528-8368-e945836f6f85"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""957de0af-0b08-4915-bef5-47bf80c276b5"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""f05d6845-1fb4-412a-8e89-57d729a5259c"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""3e5ff323-7fda-4004-acfb-b6b5be2959ef"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CameraLook"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6bd04020-4575-4317-99ac-042d48d8f1fd"",
                    ""path"": ""<Keyboard>/f5"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""QuickSave"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b142bebe-4113-4d00-a0d9-7a8419886e3c"",
                    ""path"": ""<Keyboard>/f9"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""QuickLoad"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""197452b3-2f9c-4717-8dd8-c36dd8263e9f"",
                    ""path"": ""<Keyboard>/f"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Intercation_Environnements"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""854cfd2d-3c52-436b-b492-2841a9ebd248"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Heal_plant"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1613ca06-324f-41ae-b965-c161703386a3"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Hydrate_plant"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""97228064-c35b-45e2-978d-eedf91c5e02e"",
                    ""path"": ""<Keyboard>/i"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Inventory"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Manual_Save"",
            ""id"": ""cd13c41e-e194-4af8-8acd-20ddb92c8191"",
            ""actions"": [
                {
                    ""name"": ""save_Inventory"",
                    ""type"": ""Button"",
                    ""id"": ""16f9757d-2620-492d-9815-83cafac40075"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""aa0a2b81-1882-4ca6-82c1-7e925d733e37"",
                    ""path"": ""<Keyboard>/m"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""save_Inventory"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Actions
        m_Actions = asset.FindActionMap("Actions", throwIfNotFound: true);
        m_Actions_Move = m_Actions.FindAction("Move", throwIfNotFound: true);
        m_Actions_CameraLook = m_Actions.FindAction("CameraLook", throwIfNotFound: true);
        m_Actions_QuickSave = m_Actions.FindAction("QuickSave", throwIfNotFound: true);
        m_Actions_QuickLoad = m_Actions.FindAction("QuickLoad", throwIfNotFound: true);
        m_Actions_Intercation_Environnements = m_Actions.FindAction("Intercation_Environnements", throwIfNotFound: true);
        m_Actions_Heal_plant = m_Actions.FindAction("Heal_plant", throwIfNotFound: true);
        m_Actions_Hydrate_plant = m_Actions.FindAction("Hydrate_plant", throwIfNotFound: true);
        m_Actions_Inventory = m_Actions.FindAction("Inventory", throwIfNotFound: true);
        // Manual_Save
        m_Manual_Save = asset.FindActionMap("Manual_Save", throwIfNotFound: true);
        m_Manual_Save_save_Inventory = m_Manual_Save.FindAction("save_Inventory", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // Actions
    private readonly InputActionMap m_Actions;
    private IActionsActions m_ActionsActionsCallbackInterface;
    private readonly InputAction m_Actions_Move;
    private readonly InputAction m_Actions_CameraLook;
    private readonly InputAction m_Actions_QuickSave;
    private readonly InputAction m_Actions_QuickLoad;
    private readonly InputAction m_Actions_Intercation_Environnements;
    private readonly InputAction m_Actions_Heal_plant;
    private readonly InputAction m_Actions_Hydrate_plant;
    private readonly InputAction m_Actions_Inventory;
    public struct ActionsActions
    {
        private @Player_Input_New_Version m_Wrapper;
        public ActionsActions(@Player_Input_New_Version wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_Actions_Move;
        public InputAction @CameraLook => m_Wrapper.m_Actions_CameraLook;
        public InputAction @QuickSave => m_Wrapper.m_Actions_QuickSave;
        public InputAction @QuickLoad => m_Wrapper.m_Actions_QuickLoad;
        public InputAction @Intercation_Environnements => m_Wrapper.m_Actions_Intercation_Environnements;
        public InputAction @Heal_plant => m_Wrapper.m_Actions_Heal_plant;
        public InputAction @Hydrate_plant => m_Wrapper.m_Actions_Hydrate_plant;
        public InputAction @Inventory => m_Wrapper.m_Actions_Inventory;
        public InputActionMap Get() { return m_Wrapper.m_Actions; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(ActionsActions set) { return set.Get(); }
        public void SetCallbacks(IActionsActions instance)
        {
            if (m_Wrapper.m_ActionsActionsCallbackInterface != null)
            {
                @Move.started -= m_Wrapper.m_ActionsActionsCallbackInterface.OnMove;
                @Move.performed -= m_Wrapper.m_ActionsActionsCallbackInterface.OnMove;
                @Move.canceled -= m_Wrapper.m_ActionsActionsCallbackInterface.OnMove;
                @CameraLook.started -= m_Wrapper.m_ActionsActionsCallbackInterface.OnCameraLook;
                @CameraLook.performed -= m_Wrapper.m_ActionsActionsCallbackInterface.OnCameraLook;
                @CameraLook.canceled -= m_Wrapper.m_ActionsActionsCallbackInterface.OnCameraLook;
                @QuickSave.started -= m_Wrapper.m_ActionsActionsCallbackInterface.OnQuickSave;
                @QuickSave.performed -= m_Wrapper.m_ActionsActionsCallbackInterface.OnQuickSave;
                @QuickSave.canceled -= m_Wrapper.m_ActionsActionsCallbackInterface.OnQuickSave;
                @QuickLoad.started -= m_Wrapper.m_ActionsActionsCallbackInterface.OnQuickLoad;
                @QuickLoad.performed -= m_Wrapper.m_ActionsActionsCallbackInterface.OnQuickLoad;
                @QuickLoad.canceled -= m_Wrapper.m_ActionsActionsCallbackInterface.OnQuickLoad;
                @Intercation_Environnements.started -= m_Wrapper.m_ActionsActionsCallbackInterface.OnIntercation_Environnements;
                @Intercation_Environnements.performed -= m_Wrapper.m_ActionsActionsCallbackInterface.OnIntercation_Environnements;
                @Intercation_Environnements.canceled -= m_Wrapper.m_ActionsActionsCallbackInterface.OnIntercation_Environnements;
                @Heal_plant.started -= m_Wrapper.m_ActionsActionsCallbackInterface.OnHeal_plant;
                @Heal_plant.performed -= m_Wrapper.m_ActionsActionsCallbackInterface.OnHeal_plant;
                @Heal_plant.canceled -= m_Wrapper.m_ActionsActionsCallbackInterface.OnHeal_plant;
                @Hydrate_plant.started -= m_Wrapper.m_ActionsActionsCallbackInterface.OnHydrate_plant;
                @Hydrate_plant.performed -= m_Wrapper.m_ActionsActionsCallbackInterface.OnHydrate_plant;
                @Hydrate_plant.canceled -= m_Wrapper.m_ActionsActionsCallbackInterface.OnHydrate_plant;
                @Inventory.started -= m_Wrapper.m_ActionsActionsCallbackInterface.OnInventory;
                @Inventory.performed -= m_Wrapper.m_ActionsActionsCallbackInterface.OnInventory;
                @Inventory.canceled -= m_Wrapper.m_ActionsActionsCallbackInterface.OnInventory;
            }
            m_Wrapper.m_ActionsActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
                @CameraLook.started += instance.OnCameraLook;
                @CameraLook.performed += instance.OnCameraLook;
                @CameraLook.canceled += instance.OnCameraLook;
                @QuickSave.started += instance.OnQuickSave;
                @QuickSave.performed += instance.OnQuickSave;
                @QuickSave.canceled += instance.OnQuickSave;
                @QuickLoad.started += instance.OnQuickLoad;
                @QuickLoad.performed += instance.OnQuickLoad;
                @QuickLoad.canceled += instance.OnQuickLoad;
                @Intercation_Environnements.started += instance.OnIntercation_Environnements;
                @Intercation_Environnements.performed += instance.OnIntercation_Environnements;
                @Intercation_Environnements.canceled += instance.OnIntercation_Environnements;
                @Heal_plant.started += instance.OnHeal_plant;
                @Heal_plant.performed += instance.OnHeal_plant;
                @Heal_plant.canceled += instance.OnHeal_plant;
                @Hydrate_plant.started += instance.OnHydrate_plant;
                @Hydrate_plant.performed += instance.OnHydrate_plant;
                @Hydrate_plant.canceled += instance.OnHydrate_plant;
                @Inventory.started += instance.OnInventory;
                @Inventory.performed += instance.OnInventory;
                @Inventory.canceled += instance.OnInventory;
            }
        }
    }
    public ActionsActions @Actions => new ActionsActions(this);

    // Manual_Save
    private readonly InputActionMap m_Manual_Save;
    private IManual_SaveActions m_Manual_SaveActionsCallbackInterface;
    private readonly InputAction m_Manual_Save_save_Inventory;
    public struct Manual_SaveActions
    {
        private @Player_Input_New_Version m_Wrapper;
        public Manual_SaveActions(@Player_Input_New_Version wrapper) { m_Wrapper = wrapper; }
        public InputAction @save_Inventory => m_Wrapper.m_Manual_Save_save_Inventory;
        public InputActionMap Get() { return m_Wrapper.m_Manual_Save; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(Manual_SaveActions set) { return set.Get(); }
        public void SetCallbacks(IManual_SaveActions instance)
        {
            if (m_Wrapper.m_Manual_SaveActionsCallbackInterface != null)
            {
                @save_Inventory.started -= m_Wrapper.m_Manual_SaveActionsCallbackInterface.OnSave_Inventory;
                @save_Inventory.performed -= m_Wrapper.m_Manual_SaveActionsCallbackInterface.OnSave_Inventory;
                @save_Inventory.canceled -= m_Wrapper.m_Manual_SaveActionsCallbackInterface.OnSave_Inventory;
            }
            m_Wrapper.m_Manual_SaveActionsCallbackInterface = instance;
            if (instance != null)
            {
                @save_Inventory.started += instance.OnSave_Inventory;
                @save_Inventory.performed += instance.OnSave_Inventory;
                @save_Inventory.canceled += instance.OnSave_Inventory;
            }
        }
    }
    public Manual_SaveActions @Manual_Save => new Manual_SaveActions(this);
    public interface IActionsActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnCameraLook(InputAction.CallbackContext context);
        void OnQuickSave(InputAction.CallbackContext context);
        void OnQuickLoad(InputAction.CallbackContext context);
        void OnIntercation_Environnements(InputAction.CallbackContext context);
        void OnHeal_plant(InputAction.CallbackContext context);
        void OnHydrate_plant(InputAction.CallbackContext context);
        void OnInventory(InputAction.CallbackContext context);
    }
    public interface IManual_SaveActions
    {
        void OnSave_Inventory(InputAction.CallbackContext context);
    }
}
