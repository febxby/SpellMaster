//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.6.3
//     from Assets/InputSystem/GameControls.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @GameControls: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @GameControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""GameControls"",
    ""maps"": [
        {
            ""name"": ""Play"",
            ""id"": ""8336ee99-6b44-4359-bcbb-2c3ac219ce54"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""d13b5043-f331-4808-b50b-e99932f84921"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""LeftButton"",
                    ""type"": ""Button"",
                    ""id"": ""82f39329-313c-4f22-b865-a76e66f47faa"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""OpenEditor"",
                    ""type"": ""Button"",
                    ""id"": ""ff21544e-204a-4c3e-ba8e-6777ba160b56"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Num1"",
                    ""type"": ""Button"",
                    ""id"": ""dbecc8a2-2724-4ad4-b6e0-a064dc4f99f2"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Num2"",
                    ""type"": ""Button"",
                    ""id"": ""39293758-5a75-4526-8227-28ae37851bce"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Num3"",
                    ""type"": ""Button"",
                    ""id"": ""ace8e02f-58e0-4d4d-b1a5-5f6fffacbbbe"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Num4"",
                    ""type"": ""Button"",
                    ""id"": ""7ce05eae-0200-4a18-bdc0-de28f0955f8e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Fly"",
                    ""type"": ""Button"",
                    ""id"": ""ce2f7d55-9bf5-495e-9a0a-e5c9b378326b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""PickUp"",
                    ""type"": ""Button"",
                    ""id"": ""3ba8b239-6af7-4daf-b401-cf6bb8c469be"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""e6597cf3-a26b-4e78-ab9e-b4f78e9e1219"",
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
                    ""id"": ""45628d39-0c51-433e-8268-bdf7aee8dc78"",
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
                    ""id"": ""c2cae601-2095-4a38-9cbe-e3ee0d65954d"",
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
                    ""id"": ""6564b4a6-0633-477b-91e7-58ba9167f578"",
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
                    ""id"": ""34ae6e05-09ee-404f-bfbd-69842393ee7f"",
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
                    ""id"": ""b9745067-251f-4ff1-b03b-8e805d045992"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LeftButton"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2b6d4c3b-4260-47ee-9a40-ffa9f6500eee"",
                    ""path"": ""<Keyboard>/b"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""OpenEditor"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2c6854cc-f5ed-4edc-bbfd-a667a20aab5a"",
                    ""path"": ""<Keyboard>/1"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Num1"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8839b3a8-b653-43df-bde9-62d1468911b8"",
                    ""path"": ""<Keyboard>/2"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Num2"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""403ad17a-5a61-4903-aa1c-3938d667b2e0"",
                    ""path"": ""<Keyboard>/3"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Num3"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1d158f7f-a2e2-433b-8a3b-2eff003b5b70"",
                    ""path"": ""<Keyboard>/4"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Num4"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f7028854-f9a3-4759-9af5-4373e276ce01"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Fly"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""cc796ba9-fe4a-4852-8062-6e7cca21e67a"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PickUp"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Editor"",
            ""id"": ""2ad1299b-08e6-4195-8c9b-dd2581a3ef54"",
            ""actions"": [
                {
                    ""name"": ""LeftButton"",
                    ""type"": ""Button"",
                    ""id"": ""ac88053a-dee0-45c2-a3cd-deb6ce864715"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""CloseEditor"",
                    ""type"": ""Button"",
                    ""id"": ""efb6fb5c-ea08-4540-8e65-d9aad9f59adc"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""5d5f317e-b4e5-4685-bd74-becc7675da77"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LeftButton"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f857bdcd-a9f6-4d5c-9be6-495017595b78"",
                    ""path"": ""<Keyboard>/b"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CloseEditor"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9f3efdc2-2b8e-4875-9f39-231f08ee8591"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CloseEditor"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Play
        m_Play = asset.FindActionMap("Play", throwIfNotFound: true);
        m_Play_Move = m_Play.FindAction("Move", throwIfNotFound: true);
        m_Play_LeftButton = m_Play.FindAction("LeftButton", throwIfNotFound: true);
        m_Play_OpenEditor = m_Play.FindAction("OpenEditor", throwIfNotFound: true);
        m_Play_Num1 = m_Play.FindAction("Num1", throwIfNotFound: true);
        m_Play_Num2 = m_Play.FindAction("Num2", throwIfNotFound: true);
        m_Play_Num3 = m_Play.FindAction("Num3", throwIfNotFound: true);
        m_Play_Num4 = m_Play.FindAction("Num4", throwIfNotFound: true);
        m_Play_Fly = m_Play.FindAction("Fly", throwIfNotFound: true);
        m_Play_PickUp = m_Play.FindAction("PickUp", throwIfNotFound: true);
        // Editor
        m_Editor = asset.FindActionMap("Editor", throwIfNotFound: true);
        m_Editor_LeftButton = m_Editor.FindAction("LeftButton", throwIfNotFound: true);
        m_Editor_CloseEditor = m_Editor.FindAction("CloseEditor", throwIfNotFound: true);
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

    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }

    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // Play
    private readonly InputActionMap m_Play;
    private List<IPlayActions> m_PlayActionsCallbackInterfaces = new List<IPlayActions>();
    private readonly InputAction m_Play_Move;
    private readonly InputAction m_Play_LeftButton;
    private readonly InputAction m_Play_OpenEditor;
    private readonly InputAction m_Play_Num1;
    private readonly InputAction m_Play_Num2;
    private readonly InputAction m_Play_Num3;
    private readonly InputAction m_Play_Num4;
    private readonly InputAction m_Play_Fly;
    private readonly InputAction m_Play_PickUp;
    public struct PlayActions
    {
        private @GameControls m_Wrapper;
        public PlayActions(@GameControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_Play_Move;
        public InputAction @LeftButton => m_Wrapper.m_Play_LeftButton;
        public InputAction @OpenEditor => m_Wrapper.m_Play_OpenEditor;
        public InputAction @Num1 => m_Wrapper.m_Play_Num1;
        public InputAction @Num2 => m_Wrapper.m_Play_Num2;
        public InputAction @Num3 => m_Wrapper.m_Play_Num3;
        public InputAction @Num4 => m_Wrapper.m_Play_Num4;
        public InputAction @Fly => m_Wrapper.m_Play_Fly;
        public InputAction @PickUp => m_Wrapper.m_Play_PickUp;
        public InputActionMap Get() { return m_Wrapper.m_Play; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayActions set) { return set.Get(); }
        public void AddCallbacks(IPlayActions instance)
        {
            if (instance == null || m_Wrapper.m_PlayActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_PlayActionsCallbackInterfaces.Add(instance);
            @Move.started += instance.OnMove;
            @Move.performed += instance.OnMove;
            @Move.canceled += instance.OnMove;
            @LeftButton.started += instance.OnLeftButton;
            @LeftButton.performed += instance.OnLeftButton;
            @LeftButton.canceled += instance.OnLeftButton;
            @OpenEditor.started += instance.OnOpenEditor;
            @OpenEditor.performed += instance.OnOpenEditor;
            @OpenEditor.canceled += instance.OnOpenEditor;
            @Num1.started += instance.OnNum1;
            @Num1.performed += instance.OnNum1;
            @Num1.canceled += instance.OnNum1;
            @Num2.started += instance.OnNum2;
            @Num2.performed += instance.OnNum2;
            @Num2.canceled += instance.OnNum2;
            @Num3.started += instance.OnNum3;
            @Num3.performed += instance.OnNum3;
            @Num3.canceled += instance.OnNum3;
            @Num4.started += instance.OnNum4;
            @Num4.performed += instance.OnNum4;
            @Num4.canceled += instance.OnNum4;
            @Fly.started += instance.OnFly;
            @Fly.performed += instance.OnFly;
            @Fly.canceled += instance.OnFly;
            @PickUp.started += instance.OnPickUp;
            @PickUp.performed += instance.OnPickUp;
            @PickUp.canceled += instance.OnPickUp;
        }

        private void UnregisterCallbacks(IPlayActions instance)
        {
            @Move.started -= instance.OnMove;
            @Move.performed -= instance.OnMove;
            @Move.canceled -= instance.OnMove;
            @LeftButton.started -= instance.OnLeftButton;
            @LeftButton.performed -= instance.OnLeftButton;
            @LeftButton.canceled -= instance.OnLeftButton;
            @OpenEditor.started -= instance.OnOpenEditor;
            @OpenEditor.performed -= instance.OnOpenEditor;
            @OpenEditor.canceled -= instance.OnOpenEditor;
            @Num1.started -= instance.OnNum1;
            @Num1.performed -= instance.OnNum1;
            @Num1.canceled -= instance.OnNum1;
            @Num2.started -= instance.OnNum2;
            @Num2.performed -= instance.OnNum2;
            @Num2.canceled -= instance.OnNum2;
            @Num3.started -= instance.OnNum3;
            @Num3.performed -= instance.OnNum3;
            @Num3.canceled -= instance.OnNum3;
            @Num4.started -= instance.OnNum4;
            @Num4.performed -= instance.OnNum4;
            @Num4.canceled -= instance.OnNum4;
            @Fly.started -= instance.OnFly;
            @Fly.performed -= instance.OnFly;
            @Fly.canceled -= instance.OnFly;
            @PickUp.started -= instance.OnPickUp;
            @PickUp.performed -= instance.OnPickUp;
            @PickUp.canceled -= instance.OnPickUp;
        }

        public void RemoveCallbacks(IPlayActions instance)
        {
            if (m_Wrapper.m_PlayActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IPlayActions instance)
        {
            foreach (var item in m_Wrapper.m_PlayActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_PlayActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public PlayActions @Play => new PlayActions(this);

    // Editor
    private readonly InputActionMap m_Editor;
    private List<IEditorActions> m_EditorActionsCallbackInterfaces = new List<IEditorActions>();
    private readonly InputAction m_Editor_LeftButton;
    private readonly InputAction m_Editor_CloseEditor;
    public struct EditorActions
    {
        private @GameControls m_Wrapper;
        public EditorActions(@GameControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @LeftButton => m_Wrapper.m_Editor_LeftButton;
        public InputAction @CloseEditor => m_Wrapper.m_Editor_CloseEditor;
        public InputActionMap Get() { return m_Wrapper.m_Editor; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(EditorActions set) { return set.Get(); }
        public void AddCallbacks(IEditorActions instance)
        {
            if (instance == null || m_Wrapper.m_EditorActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_EditorActionsCallbackInterfaces.Add(instance);
            @LeftButton.started += instance.OnLeftButton;
            @LeftButton.performed += instance.OnLeftButton;
            @LeftButton.canceled += instance.OnLeftButton;
            @CloseEditor.started += instance.OnCloseEditor;
            @CloseEditor.performed += instance.OnCloseEditor;
            @CloseEditor.canceled += instance.OnCloseEditor;
        }

        private void UnregisterCallbacks(IEditorActions instance)
        {
            @LeftButton.started -= instance.OnLeftButton;
            @LeftButton.performed -= instance.OnLeftButton;
            @LeftButton.canceled -= instance.OnLeftButton;
            @CloseEditor.started -= instance.OnCloseEditor;
            @CloseEditor.performed -= instance.OnCloseEditor;
            @CloseEditor.canceled -= instance.OnCloseEditor;
        }

        public void RemoveCallbacks(IEditorActions instance)
        {
            if (m_Wrapper.m_EditorActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IEditorActions instance)
        {
            foreach (var item in m_Wrapper.m_EditorActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_EditorActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public EditorActions @Editor => new EditorActions(this);
    public interface IPlayActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnLeftButton(InputAction.CallbackContext context);
        void OnOpenEditor(InputAction.CallbackContext context);
        void OnNum1(InputAction.CallbackContext context);
        void OnNum2(InputAction.CallbackContext context);
        void OnNum3(InputAction.CallbackContext context);
        void OnNum4(InputAction.CallbackContext context);
        void OnFly(InputAction.CallbackContext context);
        void OnPickUp(InputAction.CallbackContext context);
    }
    public interface IEditorActions
    {
        void OnLeftButton(InputAction.CallbackContext context);
        void OnCloseEditor(InputAction.CallbackContext context);
    }
}
