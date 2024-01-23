using NobunAtelier.Gameplay;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PropagaTransportableCharacter : PropagaTransportableObject
{
    [Header("PROPAGA Transportable Character")]
    [SerializeField]
    private HealthBehaviour m_healthBehaviour;
    [SerializeField] private Transform m_playerPlantTransform;
    [SerializeField] private Rigidbody m_characterRigidbody;
    [SerializeField] private float m_plantingLerpDurationInSeconds = 1f;
    [SerializeField] private UnityEvent OnCharacterPlanted;

    private float m_currentTime = 0;

    public override bool Pick()
    {
        if (!IsPickable)
        {
            return false;
        }

        EnablePhysics(false);

        // Prevent dead body from struggling
        if (!m_healthBehaviour.IsDead)
        {
            OnPickedEvent?.Invoke();
        }
        return true;
    }

    public void PlantCharacter(Vector3 position)
    {
        Debug.Assert(m_characterRigidbody, this);
        m_currentTime = 0;

        gameObject.SetActive(true);
        IsPickable = false;
        StartCoroutine(Planting_Coroutine(position));
    }

    private IEnumerator Planting_Coroutine(Vector3 destination)
    {
        Vector3 origin = m_characterRigidbody.position;
        while (m_currentTime < 1)
        {
            yield return null;
            m_characterRigidbody.position = Vector3.Slerp(origin, destination, m_currentTime);
            m_currentTime += Time.deltaTime / m_plantingLerpDurationInSeconds;
        }

        m_playerPlantTransform.position = m_characterRigidbody.position;
        OnCharacterPlanted?.Invoke();
    }
}