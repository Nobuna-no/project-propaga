using NobunAtelier;
using NobunAtelier.Gameplay;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AltarPlayerDetection : TriggerBehaviour
{
    [SerializeField]
    private SpriteRenderer[] m_altarTorches;
    [SerializeField] private UnityEvent OnAltarReady;

    private PropagaGameModeManager m_modeManager;
    private List<Character> m_targetList = new List<Character>();

    private void Start()
    {
        m_modeManager = PropagaGameModeManager.Instance as PropagaGameModeManager;
        Debug.Assert(m_modeManager, this);
        Debug.Assert(m_altarTorches.Length > 0, this);

        foreach (var torch in m_altarTorches)
        {
            torch.enabled = false;
        }

        RefreshAltarTorch();
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
        if (target.TryGetAbilityModule<HealthBehaviour>(out var hp))
        {
            hp.OnDeath.AddListener(OnTargetDeath);
        }

        RefreshAltarTorch();
    }

    protected override void OnTriggerExit(Collider other)
    {
        var target = other.GetComponent<Character>();
        if (target == null || !m_targetList.Contains(target))
        {
            return;
        }

        m_targetList.Remove(target);
        if (target.TryGetAbilityModule<HealthBehaviour>(out var hp))
        {
            hp.OnDeath.RemoveListener(OnTargetDeath);
        }

        RefreshAltarTorch();
    }

    private void OnTargetDeath(HitInfo hit)
    {
        for (int i = m_targetList.Count; i >= 0; i--)
        {
            Character t = m_targetList[i];
            if (t.TryGetAbilityModule<HealthBehaviour>(out var hp) && hp.IsDead)
            {
                hp.OnDeath.RemoveListener(OnTargetDeath);
                m_targetList.RemoveAt(i);
                break;
            }
        }

        RefreshAltarTorch();
    }

    private void RefreshAltarTorch()
    {
        foreach (var torch in m_altarTorches)
        {
            torch.enabled = false;
        }

        for (int i = 0; i < m_targetList.Count; i++)
        {
            var p = m_modeManager.Participants[i] as IPropagaPlayer;
            m_altarTorches[i].enabled = true;
            // m_altarTorches[i].sprite = propaga.DataDefinition.PlayerAltar;
        }

        if (m_modeManager.Participants == null)
        {
            return;
        }

        if (m_targetList.Count == m_modeManager.Participants.Count)
        {
            OnAltarReady?.Invoke();
        }
    }
}