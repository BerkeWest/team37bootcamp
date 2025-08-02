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
    private InputAction m_attackAction;
    private InputAction m_inventoryAction;

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
        m_attackAction = m_playerInput.actions["Attack"];
        m_inventoryAction = m_playerInput.actions["OpenInventory"];
    }

    #region Public Methods
    public Vector2 GetMoveInput() { return m_moveAction.ReadValue<Vector2>(); }

    public bool GetJumpInput() { return m_jumpAction.triggered; }

    public bool GetDashInput() { return m_dashAction.triggered; }

    public bool GetAttackInput() { return m_attackAction.triggered; }
    public bool GetInventoryInput() { return m_inventoryAction.triggered; }
    #endregion
}
