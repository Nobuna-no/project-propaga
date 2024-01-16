using NaughtyAttributes;
using UnityEngine;

public class PlayerControllerState_AnimatorHelper : PlayerControllerState
{
    [Header("Animation")]
    [SerializeField]
    private string m_animParamName;

    private Animator m_animator;

    public override void Enter()
    {
        base.Enter();

        if (m_animator == null)
        {
            var controller = ParentStateMachine.GetComponent<NobunAtelier.CharacterControllerBase>();
            Debug.Assert(controller, $"No controller attached to the parent state machine '{ParentStateMachine.name}'...");

            m_animator = controller.ControlledCharacter.Animator;
            Debug.Assert(m_animator, $"No animator mounted on the character movement '{controller.ControlledCharacter.name}'!");
        }

        m_animator.SetTrigger(m_animParamName);
    }
}