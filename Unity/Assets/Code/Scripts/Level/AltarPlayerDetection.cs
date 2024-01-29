using NobunAtelier;
using NobunAtelier.Gameplay;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AltarPlayerDetection : TriggerBehaviour
{
    [SerializeField] private Sprite m_defaultAltarTorch;
    [SerializeField] private SpriteRenderer[] m_altarTorches;
    [SerializeField] private UnityEvent OnAltarNotReady;
    [SerializeField] private UnityEvent OnAltarReady;
    [SerializeField] private InteractableObjectBehaviour m_altarInteraction;

    private PropagaGameModeManager m_modeManager;
    private PlayerManager m_playerManager;
    private List<Character> m_targetList = new List<Character>();

    private void Start()
    {
        m_modeManager = PropagaGameModeManager.Instance as PropagaGameModeManager;
        m_playerManager = PlayerManager.Instance;

        Debug.Assert(m_modeManager, this);
        Debug.Assert(m_playerManager, this);
        Debug.Assert(m_altarTorches.Length > 0, this);

        m_playerManager.OnParticipantJoinedEvent.AddListener(RefreshAltarTorch);
        m_playerManager.OnParticipantLeftEvent.AddListener(RefreshAltarTorch);

        m_altarInteraction.IsInteractable = false;
        RefreshAltarTorch();
    }

    private void OnDestroy()
    {
        m_playerManager.OnParticipantJoinedEvent.RemoveListener(RefreshAltarTorch);
        m_playerManager.OnParticipantLeftEvent.RemoveListener(RefreshAltarTorch);
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
        for (int i = m_targetList.Count - 1; i >= 0; i--)
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
        // Disable all of them in case a player leaved the session
        foreach (var torch in m_altarTorches)
        {
            torch.enabled = false;
        }

        for (int i = 0, k = m_targetList.Count; i < m_modeManager.Participants.Count; i++, k--)
        {
            m_altarTorches[i].enabled = true;

            if (k > 0)
            {
                var p = m_modeManager.Participants[i] as IPropagaPlayer;
                m_altarTorches[i].sprite = p.DataDefinition.PlayerAltar;
                for (int j = m_altarTorches[i].transform.childCount - 1; j >= 0; --j)
                {
                    // Set active all children
                    m_altarTorches[i].transform.GetChild(j).gameObject.SetActive(true);
                }
            }
            else
            {
                m_altarTorches[i].sprite = m_defaultAltarTorch;
            }

        }

        if (m_targetList.Count > 0 && m_targetList.Count == m_modeManager.Participants.Count)
        {
            m_altarInteraction.IsInteractable = true;
            OnAltarReady?.Invoke();
        }
        else
        {
            OnAltarNotReady?.Invoke();
        }
    }
}