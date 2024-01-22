using NobunAtelier;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RigidbodyConstrainerModule : CharacterVelocityModuleBase
{
    private Rigidbody m_rigidbody;
    private bool m_isFalling = false;

    private void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody>();
        EnableConstraint();
    }

    private void EnableConstraint()
    {
        m_isFalling = false;
        m_rigidbody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
    }

    private void DisableConstraint()
    {
        m_isFalling = true;
        m_rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
    }

    public override void StateUpdate(bool grounded)
    {
        base.StateUpdate(grounded);

        // if Falling and just landed
        if (m_isFalling && grounded)
        {
            EnableConstraint();
        }
        else if (m_isFalling == false)
        {
            DisableConstraint();
        }
    }

    public override Vector3 VelocityUpdate(Vector3 currentVel, float deltaTime)
    {
        return currentVel;
    }
}