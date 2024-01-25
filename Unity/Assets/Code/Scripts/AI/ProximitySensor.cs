using NobunAtelier;
using NobunAtelier.Gameplay;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Events;

public class ProximitySensor : TriggerBehaviour
{
    [SerializeField] private UnityEvent<Transform> OnNewTargetAcquired;
    [SerializeField] private UnityEvent OnTargetLost;
    [SerializeField] private UnityEvent OnTargetAtRange;
    [SerializeField, Range(0, 10)] private float m_atRangeDistanceUnit = 1.0f;
    [SerializeField] private Character m_currentTarget;
    private List<Character> m_targetList = new List<Character>();
    private HealthBehaviour m_targetHealthBehaviour;

    public void TryRaiseAtRangeEvent()
    {
        if (m_currentTarget == null)
        {
            return;
        }

        Vector2 diff = new Vector2(transform.position.x - m_currentTarget.Position.x, transform.position.z - m_currentTarget.Position.z);

        Debug.Log($"Try raise at range - diff: {diff}");
        if (diff.sqrMagnitude < m_atRangeDistanceUnit * m_atRangeDistanceUnit)
        {
            OnTargetAtRange?.Invoke();
        }
    }

    protected override void OnTriggerEnter(Collider other)
    {
        var target = other.GetComponent<Character>();
        if (target == null || m_targetList.Contains(target)
            || target.TryGetAbilityModule<HealthBehaviour>(out var outModule) && outModule.IsDead)
        {
            return;
        }

        m_targetList.Add(target);
        if (m_targetList.Count == 1)
        {
            AcquiredNewTarget(target);
        }
    }



    protected override void OnTriggerExit(Collider other)
    {
        var target = other.GetComponent<Character>();
        if (target == null || !m_targetList.Contains(target))
        {
            return;
        }

        LooseTarget(target);
    }

    private void AcquiredNewTarget(Character target)
    {
        m_currentTarget = target;
        OnNewTargetAcquired?.Invoke(target.transform);
        if (target.TryGetAbilityModule<HealthBehaviour>(out m_targetHealthBehaviour))
        {
            m_targetHealthBehaviour.OnDeath.AddListener(OnTargetDeath);
        }
    }

    private void OnTargetDeath(HitInfo hit)
    {
        m_targetHealthBehaviour?.OnDeath.RemoveListener(OnTargetDeath);

        if (m_currentTarget == null)
        {
            return;
        }
        LooseTarget(m_currentTarget);
    }

    private void LooseTarget(Character target)
    {
        m_targetList.Remove(target);

        m_currentTarget = null;
        m_targetHealthBehaviour = null;

        if (m_targetList.Count == 0)
        {
            OnTargetLost?.Invoke();
            return;
        }

        AcquiredNewTarget(m_targetList[m_targetList.Count - 1]);
    }
}
