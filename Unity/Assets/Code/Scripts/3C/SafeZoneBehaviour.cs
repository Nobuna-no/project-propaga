using NaughtyAttributes;
using NobunAtelier.Gameplay;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SafeZoneBehaviour : TriggerBehaviour
{
    [SerializeField, Required]
    private HealthBehaviour m_healthBehaviour;

    [SerializeField, Required, Tooltip("Damage to apply when in  danger area")]
    private HitDefinition m_dangerZoneHitDefinition;

    [SerializeField, Tooltip("Delay before applying damage when in danger area")]
    private float m_damageDelayInSeconds = 1f;

    [SerializeField, Layer] private int m_safeAreaLayerMask;

    private List<Collider> m_safeAreaList = new List<Collider>();

    public UnityEvent OnDangerZoneEntered;
    public UnityEvent OnDangerZoneExited;

    [SerializeField, ReadOnly] private bool m_isInDangerZone = false;
    [SerializeField, ReadOnly] private float m_currentTimer = 0;
    [SerializeField, ReadOnly] private int m_safeAreaCount = 0;

    private void Start()
    {
        gameObject.layer = m_safeAreaLayerMask;
        m_healthBehaviour.OnDeath.AddListener(OnDeath);
        m_healthBehaviour.OnResurrection.AddListener(OnResurrection);
    }

    private void OnDeath(HitInfo hit)
    {
        ExitDangerZone();
    }

    private void OnResurrection()
    {
        // There is an issue in case the player is resurrected while collision are disabled. But this should not happen. right...
        // When the entity is grabbed back to safe zone, it's collision are disabled...
        if (m_safeAreaList.Count == 0)
        {
            EnterDangerZone();
        }
    }

    protected override void OnTriggerEnter(Collider other)
    {
        // No. Other player are not safe zone :D
        if (other.GetComponent<SafeZoneBehaviour>() != null)
        {
            return;
        }

        m_safeAreaList.Add(other);
        m_safeAreaCount = m_safeAreaList.Count;
        if (m_isInDangerZone)
        {
            ExitDangerZone();
        }
    }

    protected override void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<SafeZoneBehaviour>() != null)
        {
            return;
        }

        m_safeAreaList.Remove(other);
        m_safeAreaCount = m_safeAreaList.Count;

        if (m_healthBehaviour.IsDead)
        {
            return;
        }

        if (m_safeAreaList.Count == 0)
        {
            EnterDangerZone();
        }
    }

    private void EnterDangerZone()
    {
        OnDangerZoneEntered?.Invoke();
        m_isInDangerZone = true;
        m_currentTimer = m_damageDelayInSeconds;

        StartCoroutine(DangerZone_Coroutine());
    }

    private void ExitDangerZone()
    {
        OnDangerZoneExited?.Invoke();
        m_isInDangerZone = false;
    }

    private IEnumerator DangerZone_Coroutine()
    {
        while (m_isInDangerZone)
        {
            if (m_currentTimer <= 0)
            {
                m_currentTimer = m_damageDelayInSeconds;
                m_healthBehaviour.ApplyDamage(m_dangerZoneHitDefinition, transform.position, this.gameObject);
            }

            do
            {
                yield return new WaitForFixedUpdate();
            }
            while (enabled == false); // If the component is disabled while we were in danger zone, temporize.

            m_currentTimer -= Time.fixedDeltaTime;
        }
        m_currentTimer = 0;
    }
}