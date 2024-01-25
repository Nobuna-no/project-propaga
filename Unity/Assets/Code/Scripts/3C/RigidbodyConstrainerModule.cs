using NobunAtelier;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class RigidbodyConstrainerModule : CharacterVelocityModuleBase
{
    [SerializeField] private UnityEvent m_OnGroundedAndConstraint;
    [SerializeField] private UnityEvent m_OnFallingAndUnconstrained;
    private Rigidbody m_rigidbody;
    private bool m_isFalling = false;


    private void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody>();
    }

    private void EnableConstraint()
    {
        m_isFalling = false;
        m_rigidbody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
        m_OnGroundedAndConstraint?.Invoke();
    }

    private void DisableConstraint()
    {
        m_isFalling = true;
        m_rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        m_OnFallingAndUnconstrained?.Invoke();
    }

    public override void StateUpdate(bool grounded)
    {
        base.StateUpdate(grounded);

        // if Falling and just landed
        if (m_isFalling && grounded)
        {
            EnableConstraint();
        }
        else if (!grounded && !m_isFalling)
        {
            DisableConstraint();
        }
    }

    public override Vector3 VelocityUpdate(Vector3 currentVel, float deltaTime)
    {
        return currentVel;
    }
}