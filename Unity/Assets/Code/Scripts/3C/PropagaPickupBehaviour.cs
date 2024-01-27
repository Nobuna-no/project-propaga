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
            m_lastFeedbackTarget = null;
        }

        UdpateFeedbackActivation();
    }

    private void UdpateFeedbackActivation()
    {
        m_feedbackEnabled = m_lastFeedbackTarget != null && m_lastFeedbackTarget.spriteRenderer != null;
        m_spriteFeedback.enabled = m_feedbackEnabled;
        if (m_spriteFeedback.enabled)
        {
            m_spriteFeedback.sprite = m_lastFeedbackTarget.spriteRenderer.sprite;
            m_spriteFeedback.transform.localScale = m_lastFeedbackTarget.spriteRenderer.transform.lossyScale;
            m_spriteFeedback.transform.localScale *= m_outlineSize;
        }
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

        var feedbackTarget = obj.GetComponent<IPropagaSpriteProvider>();
        if (feedbackTarget != m_lastFeedbackTarget)
        {
            // Just another object that was not the first target that is leaving the array.
            return;
        }

        m_lastFeedbackTarget = null;
        UdpateFeedbackActivation();
    }

    public void Update()
    {
        UpdateFeedbackObject();
    }

    public void UpdateFeedbackObject()
    {
        if (GatherableObjects.Count == 0)
            return;
        
        var pickUpObj = GatherableObjects[GatherableObjects.Count - 1];
        bool shouldBeTarget = false;
        var spriteProvider = pickUpObj.GetComponent<IPropagaSpriteProvider>();
        if (CanPickup)
        {
            shouldBeTarget = true;
        }
        
        bool isTarget = spriteProvider == m_lastFeedbackTarget;
        if (isTarget == shouldBeTarget)
        {
            return;
        }
        
        if (shouldBeTarget)
        {
            m_lastFeedbackTarget = spriteProvider;
            OnDisplayAvailable?.Invoke();
        }
        else
        {
            m_lastFeedbackTarget = null;
            UdpateFeedbackActivation();
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