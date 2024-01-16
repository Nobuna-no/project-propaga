using NaughtyAttributes;
using NobunAtelier.Gameplay;
using UnityEngine;

public class PlayerControllerState_DeadOrAlive : PlayerControllerState
{
    [Header("States Outcome")]
    [SerializeField]
    private PlayerControllerStateDefinition m_nextStateIfAlive;
    [SerializeField]
    private PlayerControllerStateDefinition m_nextStateIfDead;

    private HealthBehaviour m_healthBehaviour;

    public override void Enter()
    {
        if (m_healthBehaviour == null)
        {
            var controller = ParentStateMachine.GetComponent<NobunAtelier.CharacterControllerBase>();
            Debug.Assert(controller, $"No controller attached to the parent state machine '{ParentStateMachine.name}'...");

            var character = controller.ControlledCharacter;
            character.TryGetAbilityModule(out m_healthBehaviour);
            Debug.Assert(m_healthBehaviour, $"No m_healthBehaviour attached to controlled character '{character.name}'...");
        }

        SetNextState(m_healthBehaviour.IsDead ? m_nextStateIfDead : m_nextStateIfAlive);

        base.Enter();
    }
}