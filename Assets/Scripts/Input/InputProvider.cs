using Input.Direct;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "Input Provider", menuName = "Game/Input Provider")]
public class InputProvider : ScriptableObject,
    Controls.IGameplayActions,
    Controls.IConsoleActions,
    Controls.IMenuActions
{
    public event UnityAction<Vector2> Move = delegate {  };
    public event UnityAction<Vector2> MoveMouse = delegate {  };
    public event UnityAction StartSprint = delegate {  };
    public event UnityAction CancelSprint = delegate {  };
    public event UnityAction StartInteract = delegate {  };
    public event UnityAction StopInteract = delegate {  };
    public event UnityAction StartSecondaryInteract = delegate {  };
    public event UnityAction StopSecondaryInteract = delegate {  };
    public event UnityAction StartDrop = delegate {  };
    public event UnityAction EndDrop = delegate {  };
    public event UnityAction StartToss = delegate {  };
    public event UnityAction EndToss = delegate {  };
    public event UnityAction StartRotate = delegate {  };
    public event UnityAction StopRotate = delegate {  };
    public event UnityAction Jump = delegate {  };
    public event UnityAction StartSlow = delegate {  };
    public event UnityAction StopSlow = delegate {  };
    public event UnityAction<float> Scroll = delegate {  };
    public event UnityAction ConsoleOpen = delegate {  };
    public event UnityAction ConsoleSubmit = delegate {  };
    public event UnityAction Exit = delegate {  };
    public event UnityAction<Vector2> ChangeCursorPosition = delegate {  };
    public event UnityAction OpenLaptop = delegate {  };
    
    Controls controls;

    void OnEnable()
    {
        if (controls != null) return;

        controls = new Controls();
        controls.Gameplay.SetCallbacks(this);
        controls.Console.SetCallbacks(this);
        controls.Menu.SetCallbacks(this);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Move.Invoke(context.ReadValue<Vector2>());
    }

    public void OnMoveMouse(InputAction.CallbackContext context)
    {
        MoveMouse.Invoke(context.ReadValue<Vector2>());
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Performed:
                StartSprint.Invoke();
                break;
            case InputActionPhase.Canceled:
                CancelSprint.Invoke();
                break;
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Performed:
                StartInteract.Invoke();
                break;
            case InputActionPhase.Canceled:
                StopInteract.Invoke();
                break;
        }
    }

    public void OnSecondaryInteract(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Performed:
                StartSecondaryInteract.Invoke();
                break;
            case InputActionPhase.Canceled:
                StopSecondaryInteract.Invoke();
                break;
        }
    }

    public void OnDrop(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Performed:
                StartDrop.Invoke();
                break;
            case InputActionPhase.Canceled:
                EndDrop.Invoke();
                break;
        }
    }

    public void OnToss(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Performed:
                StartToss.Invoke();
                break;
            case InputActionPhase.Canceled:
                EndToss.Invoke();
                break;
        }
    }

    public void OnRotate(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Performed:
                StartRotate.Invoke();
                break;
            case InputActionPhase.Canceled:
                StopRotate.Invoke();
                break;
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        Jump.Invoke();
    }

    public void OnSlow(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Performed:
                StartSlow.Invoke();
                break;
            case InputActionPhase.Canceled:
                StopSlow.Invoke();
                break;
        }
    }

    public void OnScroll(InputAction.CallbackContext context)
    {
        Scroll.Invoke(context.ReadValue<float>());
    }

    public void OnOpen(InputAction.CallbackContext context)
    {
        ConsoleOpen.Invoke();
    }

    public void OnSubmit(InputAction.CallbackContext context)
    {
        ConsoleSubmit.Invoke();
    }

    public void OnExit(InputAction.CallbackContext context)
    {
        Exit.Invoke();
    }

    public void OnCursorPosition(InputAction.CallbackContext context)
    {
        ChangeCursorPosition.Invoke(context.ReadValue<Vector2>());
    }

    public void OnOpenLaptop(InputAction.CallbackContext context)
    {
        OpenLaptop.Invoke();
    }
}
