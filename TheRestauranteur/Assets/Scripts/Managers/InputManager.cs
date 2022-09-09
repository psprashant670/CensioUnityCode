using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;
    public static Action<InputAction.CallbackContext> MouseClick, MouseDrag, Enter;

    public PlayerInput playerInput;

    private void Awake() 
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    public void OnTouch(InputAction.CallbackContext value)
    {
        if (MouseClick != null)
        {
            MouseClick(value);
        }
    }

    public void OnPos(InputAction.CallbackContext value)
    {
        if (MouseDrag != null)
        {
            MouseDrag(value);
        }
    }

    public void OnEnter(InputAction.CallbackContext value)
    {
        if (Enter != null)
        {
            Enter(value);
        }
    }

}
