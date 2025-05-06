using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    private PlayerInputActions playerInputActions;
    public static GameInput Instance { get; private set; }

    public event EventHandler OnPlayerAttack;

    private void Awake()
    {
        Instance = this;

        playerInputActions = new PlayerInputActions();
        playerInputActions.Enable();

        playerInputActions.Combat.Attack.started += Attack_startedPlayer;
    }

    private void Attack_startedPlayer(InputAction.CallbackContext obj)
    {
        OnPlayerAttack?.Invoke(this, EventArgs.Empty);
    }

    public Vector2 GetMovementVector() => playerInputActions.Player.Move.ReadValue<Vector2>();

    public Vector3 GetMousePosition() => Mouse.current.position.ReadValue();
}
