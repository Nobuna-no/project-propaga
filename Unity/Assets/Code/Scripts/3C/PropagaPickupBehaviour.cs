using NobunAtelier.Gameplay;
using System;
using Unity.VisualScripting;
using UnityEngine;
using static BehaviourWithPriority;

public class PropagaPickupBehaviour : PickupBehaviour, BehaviourWithPriority
{
    public int Priority => 1;
    public event OnBehaviorFeedbackAvailableDelegate OnDisplayAvailable;

    private bool m_feedbackEnabled = false;

    private IPropagaSpriteProvider m_lastFeedbackTarget;
    [SerializeField] private SpriteRenderer m_spriteFeedback;
    [SerializeField] private float m_outlineSize = 1.1f;

    public void StartBehaviourFeedback()
    {
        if (m_lastFeedbackTarget == null)
        {
            return;
        }

        if (!GatherableObjects.Find(x => x.GetComponent<IPropagaSpriteProvider>() == m_lastFeedbackTarget))
        {
            // Seems previous target is no longer available...
            m_spriteFeedback.enabled = false;
            m_lastFeedbackTarget = null;

            if (CanPickup && GatherableObjects.Count > 0)
            {
                OnGatherableObjectAddedEvent(GatherableObjects[GatherableObjects.Count - 1]);
            }
            return;
        }

        m_feedbackEnabled = true;
        m_spriteFeedback.enabled = true;
        m_spriteFeedback.sprite = m_lastFeedbackTarget.spriteRenderer.sprite;
        m_spriteFeedback.transform.localScale = m_lastFeedbackTarget.spriteRenderer.transform.lossyScale;
        m_spriteFeedback.transform.localScale *= m_outlineSize;
    }

    protected override void Awake()
    {
        base.Awake();
        OnGatherableObjectAdded += OnGatherableObjectAddedEvent;
        OnGatherableObjectRemoved += OnGatherableObjectRemovedEvent; ;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    private void OnGatherableObjectAddedEvent(TransportableObjectBehaviour obj)
    {
        m_lastFeedbackTarget = obj.GetComponent<IPropagaSpriteProvider>();
        // Send callback and wait for authorization to display through StartBehaviourFeedback.
        OnDisplayAvailable.Invoke();
    }

    private void OnGatherableObjectRemovedEvent(TransportableObjectBehaviour obj)
    {
        if (!m_feedbackEnabled)
        {
            return;
        }

        if (m_lastFeedbackTarget == null)
        {
            m_feedbackEnabled = false;
            return;
        }

        var feedbackTarget = obj.GetComponent<IPropagaSpriteProvider>();
        if (feedbackTarget != m_lastFeedbackTarget)
        {
            // Just another object that was not the first target that is leaving the array.
            return;
        }

        m_feedbackEnabled = false;
        m_spriteFeedback.enabled = false;
        m_lastFeedbackTarget = null;

        if (CanPickup && GatherableObjects.Count > 0)
        {
            OnGatherableObjectAddedEvent(GatherableObjects[GatherableObjects.Count - 1]);
        }
    }


    public bool CanBeExecuted()
    {
        // Should check if gatherable object is pickable
        return CanPickup;
    }

    public void Execute()
    {
        TryGather();
    }

    private void LateUpdate()
    {
        if (!m_feedbackEnabled)
        {
            return;
        }

        if (m_spriteFeedback != null)
        {
            m_spriteFeedback.transform.position = m_lastFeedbackTarget.spriteRenderer.transform.position;
        }
    }
}