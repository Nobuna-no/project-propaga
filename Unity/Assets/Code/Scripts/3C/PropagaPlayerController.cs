using NobunAtelier;
using NobunAtelier.Gameplay;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System.Runtime.CompilerServices;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerControllerStateMachine))]
public class PropagaPlayerController : NobunAtelier.PlayerController
{
    [Header("Gameplay")]
    [SerializeField]
    private SocketStorageBehaviour m_storageComponent;
    
    [Header("Visuals")]
    [SerializeField]
    private PlayerIdBehaviour m_playerId;

    public PlayerIdBehaviour PlayerId => m_playerId;

    [Header("Input")]
    [SerializeField] private string m_interactActionName;
    [SerializeField] private string m_attackActionName;
    [SerializeField] private string m_dropActionName;


    [Header("State Machine")]
    [SerializeField, Required] private PlayerControllerStateDefinition m_idleDefinition;
    [SerializeField, Required] private PlayerControllerStateDefinition m_strugglingDefinition;
    [SerializeField, Required] private PlayerControllerStateDefinition m_interactDefinition;
    [SerializeField, Required] private PlayerControllerStateDefinition m_dropDefinition;
    [SerializeField, Required] private PlayerControllerStateDefinition m_attackDefinition;
    [SerializeField, Required] private PlayerControllerStateDefinition m_throwDefinition;
    [SerializeField, Required] private PlayerControllerStateDefinition m_rootedDefinition;

    [Header("Debug")]
    [SerializeField, ReadOnly] private bool m_wantToInteract;
    [SerializeField, ReadOnly] private bool m_wantToAttack;
    [SerializeField, ReadOnly] private bool m_wantToDrop;

    private PlayerControllerStateMachine m_stateMachine;
    private InputAction m_interactAction;
    private InputAction m_dropAction;
    private InputAction m_attackAction;

    protected override void Awake()
    {
        base.Awake();

        m_stateMachine = GetComponent<PlayerControllerStateMachine>();
        Debug.Assert(m_stateMachine != null, $"{this.name}: Missing state machine.", this);

        Debug.Assert(m_idleDefinition != null, $"{this.name}: Missing idle state definition.", this);
        Debug.Assert(m_strugglingDefinition != null, $"{this.name}: Missing struggling state definition.", this);
        Debug.Assert(m_interactDefinition != null, $"{this.name}: Missing interact state definition.", this);
        Debug.Assert(m_dropDefinition != null, $"{this.name}: Missing drop state definition.", this);
        Debug.Assert(m_attackDefinition != null, $"{this.name}: Missing attack state definition.", this);
        Debug.Assert(m_throwDefinition != null, $"{this.name}: Missing throw state definition.", this);
    }

    public override void EnableInput()
    {
        base.EnableInput();

        m_interactAction = ActionMap.FindAction(m_interactActionName);
        m_interactAction.performed += OnInteractAction_performed;
        m_dropAction = ActionMap.FindAction(m_dropActionName);
        m_dropAction.performed += OnDropAction_performed;
        m_attackAction = ActionMap.FindAction(m_attackActionName);
        m_attackAction.performed += OnAttackAction_performed;
    }


    public override void DisableInput()
    {
        m_interactAction.performed -= OnInteractAction_performed;
        m_dropAction.performed -= OnDropAction_performed;
        m_attackAction.performed -= OnAttackAction_performed;

        base.DisableInput();
    }

    // Can attack from idle and while being grabbed.
    private void OnAttackAction_performed(InputAction.CallbackContext obj)
    {
        if (m_stateMachine.CurrentStateDefinition != m_idleDefinition
            && m_stateMachine.CurrentStateDefinition != m_strugglingDefinition)
        {
            return;
        }

        m_wantToAttack = obj.ReadValue<float>() != 0;
    }

    // Can drop only when idle.
    private void OnDropAction_performed(InputAction.CallbackContext obj)
    {
        if (m_stateMachine.CurrentStateDefinition != m_idleDefinition)
        {
            return;
        }

        m_wantToDrop = obj.ReadValue<float>() != 0;
    }

    // Can pick only when idle.
    private void OnInteractAction_performed(InputAction.CallbackContext obj)
    {
        if (m_stateMachine.CurrentStateDefinition != m_idleDefinition)
        {
            return;
        }

        m_wantToInteract = obj.ReadValue<float>() != 0;
    }

    protected override void UpdateController(float deltaTime)
    {
        // We prevent any input to go through if we are not idling (even movement)
        if (m_stateMachine.CurrentStateDefinition != m_idleDefinition
            && m_stateMachine.CurrentStateDefinition != m_strugglingDefinition)
        {
            return;
        }

        base.UpdateController(deltaTime);

        if (m_wantToDrop && TryToDrop())
        {
            return;
        }

        if (m_wantToAttack)
        {
            DoAttack();
            return;
        }

        if (m_wantToInteract)
        {
            DoInteract();
            return;
        }
    }

    private void DoInteract()
    {
        m_wantToInteract = false;

        // if (m_storageComponent.HasAvailableSocket)
        {
            // Debug.Log("Do try to interact");
            // m_storageComponent.ItemTryPeekFirst(out TransportableObjectBehaviour item)
            m_stateMachine.SetState(m_interactDefinition);
        }
    }

    private void DoAttack()
    {
        m_wantToAttack = false;

        if (m_storageComponent.HasAvailableItem)
        {
            // Debug.Log("Do throw item");
            m_stateMachine.SetState(m_throwDefinition);
        }
        else
        {
            // Debug.Log("Do attack");
            // TryDropIngredient(item);
            m_stateMachine.SetState(m_attackDefinition);
        }
    }

    private bool TryToDrop()
    {
        m_wantToDrop = false;

        if (m_storageComponent.HasAvailableItem)
        {
            // Debug.Log("Do drop item");
            m_stateMachine.SetState(m_dropDefinition);
            return true;
        }

        return false;
    }

    [Button(enabledMode: EButtonEnableMode.Playmode)]
    private void ForceRootedState()
    {
        m_stateMachine.SetState(m_rootedDefinition);
    }

}
