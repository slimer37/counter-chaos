// GENERATED AUTOMATICALLY FROM 'Assets/Settings/Controls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace Core
{
    public class @Controls : IInputActionCollection, IDisposable
    {
        public InputActionAsset asset { get; }
        public @Controls()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""Controls"",
    ""maps"": [
        {
            ""name"": ""Gameplay"",
            ""id"": ""c91b91cc-8bef-4123-952c-cbee26013e92"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""5b169ca4-1a72-432d-9866-5217aba38ae5"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Move Mouse"",
                    ""type"": ""Value"",
                    ""id"": ""e857c98d-cf8b-4f26-958e-db972948ce5d"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Sprint"",
                    ""type"": ""Value"",
                    ""id"": ""5b48072b-7981-4586-a5ba-4fb427e13e88"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Interact"",
                    ""type"": ""Value"",
                    ""id"": ""b1cc4888-a331-4bfd-ba99-60f092c5936e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Secondary Interact"",
                    ""type"": ""Value"",
                    ""id"": ""21172373-e067-4232-8987-06a08f9e0b98"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Drop"",
                    ""type"": ""Value"",
                    ""id"": ""4b4e7243-6056-4bff-8a30-ec185ba26502"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Toss"",
                    ""type"": ""Value"",
                    ""id"": ""c16c4857-2936-4b3d-8003-a95b9618a054"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Rotate"",
                    ""type"": ""Value"",
                    ""id"": ""0fe83fe5-c0fb-49ba-9860-6776c16f135b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""WASD"",
                    ""id"": ""6fae5528-8ac8-4724-850e-b0b3580ff83c"",
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
                    ""id"": ""2b033417-d30f-4a71-9334-5afa5d377754"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard + Mouse"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""7424582c-a765-49b3-ba46-9c0e0829f219"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard + Mouse"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""7d384b97-77fc-4fd8-a110-52a8f1c3a3a2"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard + Mouse"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""ec47a47c-afdc-486b-8261-33b2e4376fba"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard + Mouse"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""b91bc581-72a1-44d4-b6c0-9b8c72b32aca"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard + Mouse"",
                    ""action"": ""Move Mouse"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a96cf30b-17e4-451b-9a98-e2a25442ecc1"",
                    ""path"": ""<Keyboard>/shift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard + Mouse"",
                    ""action"": ""Sprint"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4169c2c2-c74b-44f5-98e1-c08462976cd1"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard + Mouse"",
                    ""action"": ""Interact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""21f76599-e0c7-4bb9-b9c1-b52ce0746fc8"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard + Mouse"",
                    ""action"": ""Drop"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""fb328527-072c-4ebe-9d77-bdd0265807d2"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard + Mouse"",
                    ""action"": ""Toss"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""810094d7-7b87-4c95-952b-8c91f7f15f73"",
                    ""path"": ""<Keyboard>/r"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard + Mouse"",
                    ""action"": ""Rotate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c93a9064-0d70-4a2a-8169-9969ebbb0b7a"",
                    ""path"": ""<Keyboard>/r"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard + Mouse"",
                    ""action"": ""Secondary Interact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Menu"",
            ""id"": ""e3a93637-a32f-408f-839d-fd5fef7ce8e6"",
            ""actions"": [
                {
                    ""name"": ""Exit"",
                    ""type"": ""Button"",
                    ""id"": ""c000e25f-6bec-4f9a-be92-faa82ee8711d"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""230136c3-347d-4efc-95d5-4a7842eb2c25"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Exit"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Keyboard + Mouse"",
            ""bindingGroup"": ""Keyboard + Mouse"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
            // Gameplay
            m_Gameplay = asset.FindActionMap("Gameplay", throwIfNotFound: true);
            m_Gameplay_Move = m_Gameplay.FindAction("Move", throwIfNotFound: true);
            m_Gameplay_MoveMouse = m_Gameplay.FindAction("Move Mouse", throwIfNotFound: true);
            m_Gameplay_Sprint = m_Gameplay.FindAction("Sprint", throwIfNotFound: true);
            m_Gameplay_Interact = m_Gameplay.FindAction("Interact", throwIfNotFound: true);
            m_Gameplay_SecondaryInteract = m_Gameplay.FindAction("Secondary Interact", throwIfNotFound: true);
            m_Gameplay_Drop = m_Gameplay.FindAction("Drop", throwIfNotFound: true);
            m_Gameplay_Toss = m_Gameplay.FindAction("Toss", throwIfNotFound: true);
            m_Gameplay_Rotate = m_Gameplay.FindAction("Rotate", throwIfNotFound: true);
            // Menu
            m_Menu = asset.FindActionMap("Menu", throwIfNotFound: true);
            m_Menu_Exit = m_Menu.FindAction("Exit", throwIfNotFound: true);
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

        // Gameplay
        private readonly InputActionMap m_Gameplay;
        private IGameplayActions m_GameplayActionsCallbackInterface;
        private readonly InputAction m_Gameplay_Move;
        private readonly InputAction m_Gameplay_MoveMouse;
        private readonly InputAction m_Gameplay_Sprint;
        private readonly InputAction m_Gameplay_Interact;
        private readonly InputAction m_Gameplay_SecondaryInteract;
        private readonly InputAction m_Gameplay_Drop;
        private readonly InputAction m_Gameplay_Toss;
        private readonly InputAction m_Gameplay_Rotate;
        public struct GameplayActions
        {
            private @Controls m_Wrapper;
            public GameplayActions(@Controls wrapper) { m_Wrapper = wrapper; }
            public InputAction @Move => m_Wrapper.m_Gameplay_Move;
            public InputAction @MoveMouse => m_Wrapper.m_Gameplay_MoveMouse;
            public InputAction @Sprint => m_Wrapper.m_Gameplay_Sprint;
            public InputAction @Interact => m_Wrapper.m_Gameplay_Interact;
            public InputAction @SecondaryInteract => m_Wrapper.m_Gameplay_SecondaryInteract;
            public InputAction @Drop => m_Wrapper.m_Gameplay_Drop;
            public InputAction @Toss => m_Wrapper.m_Gameplay_Toss;
            public InputAction @Rotate => m_Wrapper.m_Gameplay_Rotate;
            public InputActionMap Get() { return m_Wrapper.m_Gameplay; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(GameplayActions set) { return set.Get(); }
            public void SetCallbacks(IGameplayActions instance)
            {
                if (m_Wrapper.m_GameplayActionsCallbackInterface != null)
                {
                    @Move.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMove;
                    @Move.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMove;
                    @Move.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMove;
                    @MoveMouse.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMoveMouse;
                    @MoveMouse.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMoveMouse;
                    @MoveMouse.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMoveMouse;
                    @Sprint.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnSprint;
                    @Sprint.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnSprint;
                    @Sprint.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnSprint;
                    @Interact.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnInteract;
                    @Interact.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnInteract;
                    @Interact.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnInteract;
                    @SecondaryInteract.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnSecondaryInteract;
                    @SecondaryInteract.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnSecondaryInteract;
                    @SecondaryInteract.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnSecondaryInteract;
                    @Drop.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnDrop;
                    @Drop.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnDrop;
                    @Drop.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnDrop;
                    @Toss.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnToss;
                    @Toss.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnToss;
                    @Toss.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnToss;
                    @Rotate.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnRotate;
                    @Rotate.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnRotate;
                    @Rotate.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnRotate;
                }
                m_Wrapper.m_GameplayActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @Move.started += instance.OnMove;
                    @Move.performed += instance.OnMove;
                    @Move.canceled += instance.OnMove;
                    @MoveMouse.started += instance.OnMoveMouse;
                    @MoveMouse.performed += instance.OnMoveMouse;
                    @MoveMouse.canceled += instance.OnMoveMouse;
                    @Sprint.started += instance.OnSprint;
                    @Sprint.performed += instance.OnSprint;
                    @Sprint.canceled += instance.OnSprint;
                    @Interact.started += instance.OnInteract;
                    @Interact.performed += instance.OnInteract;
                    @Interact.canceled += instance.OnInteract;
                    @SecondaryInteract.started += instance.OnSecondaryInteract;
                    @SecondaryInteract.performed += instance.OnSecondaryInteract;
                    @SecondaryInteract.canceled += instance.OnSecondaryInteract;
                    @Drop.started += instance.OnDrop;
                    @Drop.performed += instance.OnDrop;
                    @Drop.canceled += instance.OnDrop;
                    @Toss.started += instance.OnToss;
                    @Toss.performed += instance.OnToss;
                    @Toss.canceled += instance.OnToss;
                    @Rotate.started += instance.OnRotate;
                    @Rotate.performed += instance.OnRotate;
                    @Rotate.canceled += instance.OnRotate;
                }
            }
        }
        public GameplayActions @Gameplay => new GameplayActions(this);

        // Menu
        private readonly InputActionMap m_Menu;
        private IMenuActions m_MenuActionsCallbackInterface;
        private readonly InputAction m_Menu_Exit;
        public struct MenuActions
        {
            private @Controls m_Wrapper;
            public MenuActions(@Controls wrapper) { m_Wrapper = wrapper; }
            public InputAction @Exit => m_Wrapper.m_Menu_Exit;
            public InputActionMap Get() { return m_Wrapper.m_Menu; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(MenuActions set) { return set.Get(); }
            public void SetCallbacks(IMenuActions instance)
            {
                if (m_Wrapper.m_MenuActionsCallbackInterface != null)
                {
                    @Exit.started -= m_Wrapper.m_MenuActionsCallbackInterface.OnExit;
                    @Exit.performed -= m_Wrapper.m_MenuActionsCallbackInterface.OnExit;
                    @Exit.canceled -= m_Wrapper.m_MenuActionsCallbackInterface.OnExit;
                }
                m_Wrapper.m_MenuActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @Exit.started += instance.OnExit;
                    @Exit.performed += instance.OnExit;
                    @Exit.canceled += instance.OnExit;
                }
            }
        }
        public MenuActions @Menu => new MenuActions(this);
        private int m_KeyboardMouseSchemeIndex = -1;
        public InputControlScheme KeyboardMouseScheme
        {
            get
            {
                if (m_KeyboardMouseSchemeIndex == -1) m_KeyboardMouseSchemeIndex = asset.FindControlSchemeIndex("Keyboard + Mouse");
                return asset.controlSchemes[m_KeyboardMouseSchemeIndex];
            }
        }
        public interface IGameplayActions
        {
            void OnMove(InputAction.CallbackContext context);
            void OnMoveMouse(InputAction.CallbackContext context);
            void OnSprint(InputAction.CallbackContext context);
            void OnInteract(InputAction.CallbackContext context);
            void OnSecondaryInteract(InputAction.CallbackContext context);
            void OnDrop(InputAction.CallbackContext context);
            void OnToss(InputAction.CallbackContext context);
            void OnRotate(InputAction.CallbackContext context);
        }
        public interface IMenuActions
        {
            void OnExit(InputAction.CallbackContext context);
        }
    }
}