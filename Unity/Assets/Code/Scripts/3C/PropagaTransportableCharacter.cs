using NobunAtelier.Gameplay;
using UnityEngine;

public class PropagaTransportableCharacter : PropagaTransportableObject
{
    [Header("PROPAGA Transportable Character")]
    [SerializeField]
    private HealthBehaviour m_healthBehaviour;

    // Prevent dead body from struggling
    public override bool Pick()
    {
        if (!IsPickable)
        {
            return false;
        }

        EnablePhysics(false);
        if (!m_healthBehaviour.IsDead)
        {
            OnPickedEvent?.Invoke();
        }
        return true;
    }
}