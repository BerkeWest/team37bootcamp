using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [SerializeField] private PlayerInput m_playerInput;

    private InputAction m_moveAction;
    private InputAction m_jumpAction;
    private InputAction m_dashAction;

    public static InputManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        m_playerInput = GetComponent<PlayerInput>();

        m_moveAction = m_playerInput.actions["Move"];
        m_jumpAction = m_playerInput.actions["Jump"];
        m_dashAction = m_playerInput.actions["Dash"];
    }

    public Vector2 GetMoveInput() { return m_moveAction.ReadValue<Vector2>(); }

    public bool GetJumpInput() { return m_jumpAction.triggered; }

    public bool GetDashInput() { return m_dashAction.triggered; }

}
