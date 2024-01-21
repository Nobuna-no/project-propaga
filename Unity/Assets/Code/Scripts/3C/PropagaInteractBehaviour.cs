using NaughtyAttributes;
using NobunAtelier.Gameplay;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static BehaviourWithPriority;

public class PropagaInteractBehaviour : TriggerBehaviour, BehaviourWithPriority
{
    [SerializeField] private SocketStorageBehaviour m_storageComponent;
    [SerializeField] private InteractableCallback[] m_consumptionCallbacks;

    public List<InteractableObjectBehaviour> InteractableObjects => m_baseInteractableObjects;

    public int Priority => 0;

    public event OnBehaviorFeedbackAvailableDelegate OnDisplayAvailable;

    private List<InteractableObjectBehaviour> m_baseInteractableObjects = new List<InteractableObjectBehaviour>();

    public event System.Action<InteractableObjectBehaviour> OnInteractableObjectAdded;

    public event System.Action<InteractableObjectBehaviour> OnInteractableObjectRemoved;

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

        if (!m_baseInteractableObjects.Find(x => x.GetComponent<IPropagaSpriteProvider>() == m_lastFeedbackTarget))
        {
            // Seems previous target is no longer available...
            m_spriteFeedback.enabled = false;
            m_lastFeedbackTarget = null;

            if (CanBeExecuted())
            {
                OnInteractableObjectAdded(m_baseInteractableObjects[m_baseInteractableObjects.Count - 1]);
            }
            return;
        }

        m_feedbackEnabled = true;
        m_spriteFeedback.enabled = true;
        m_spriteFeedback.sprite = m_lastFeedbackTarget.spriteRenderer.sprite;
        m_spriteFeedback.transform.localScale = m_lastFeedbackTarget.spriteRenderer.transform.lossyScale;
        m_spriteFeedback.transform.localScale *= m_outlineSize;
    }

    [Button]
    public void TryInteract()
    {
        if (!enabled && m_storageComponent == null)
        {
            return;
        }

        if (m_baseInteractableObjects.Count == 0)
        {
            return;
        }

        var interactableObj = m_baseInteractableObjects[m_baseInteractableObjects.Count - 1];
        if (!interactableObj)
        {
            return;
        }

        if (CanInteractWith(interactableObj, out var obj))
        {
            m_storageComponent.ItemTryConsume(out var objRef);

            interactableObj.Use(obj?.ItemDefinition);
            obj.IsActive = false; // Remove the object

            // Raise any target interaction callback.
            for (int i = 0, c = m_consumptionCallbacks.Length; i < c; i++)
            {
                m_consumptionCallbacks[i].CheckAndRaiseCallbackForTarget(interactableObj);
            }
        }
    }

    public bool CanBeExecuted()
    {
        if (!enabled && m_storageComponent == null)
        {
            return false;
        }

        if (m_baseInteractableObjects.Count == 0)
        {
            return false;
        }

        var interactableObj = m_baseInteractableObjects[m_baseInteractableObjects.Count - 1];
        if (!interactableObj)
        {
            return false;
        }

        return CanInteractWith(interactableObj, out var obj);
    }

    public void Execute()
    {
        TryInteract();
    }

    private bool CanInteractWith(InteractableObjectBehaviour interactableObj, out PropagaTransportableObject obj)
    {
        bool interactAccepted;
        if (m_storageComponent.ItemTryPeekFirst(out var transportObj))
        {
            obj = transportObj as PropagaTransportableObject;
            interactAccepted = interactableObj.CheckInput(obj.ItemDefinition);
        }
        else
        {
            obj = null;
            interactAccepted = interactableObj.CheckInput(null);
        }

        return interactAccepted;
    }

    protected override void OnTriggerEnter(Collider other)
    {
        InteractableObjectBehaviour gatherable = other.GetComponent<InteractableObjectBehaviour>();
        if (gatherable != null && !m_baseInteractableObjects.Contains(gatherable))
        {
            m_baseInteractableObjects.Add(gatherable);
            OnInteractableObjectAdded?.Invoke(gatherable);
        }
    }

    protected override void OnTriggerExit(Collider other)
    {
        InteractableObjectBehaviour gatherable = other.GetComponent<InteractableObjectBehaviour>();
        if (gatherable != null && m_baseInteractableObjects.Contains(gatherable))
        {
            m_baseInteractableObjects.Remove(gatherable);
            OnInteractableObjectRemoved?.Invoke(gatherable);
        }
    }

    private void Start()
    {
        OnInteractableObjectAdded += OnInteractableObjectAddedEvent;
        OnInteractableObjectRemoved += OnInteractableObjectRemovedEvent; ;
    }

    private void OnInteractableObjectAddedEvent(InteractableObjectBehaviour obj)
    {
        m_lastFeedbackTarget = obj.GetComponent<IPropagaSpriteProvider>();
        // Send callback and wait for authorization to display through StartBehaviourFeedback.
        OnDisplayAvailable.Invoke();
    }

    private void OnInteractableObjectRemovedEvent(InteractableObjectBehaviour obj)
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
        // m_lastFeedbackTarget.SetActiveInteractionFeedback(false);
        m_lastFeedbackTarget = null;

        if (CanBeExecuted())
        {
            OnInteractableObjectAddedEvent(m_baseInteractableObjects[m_baseInteractableObjects.Count - 1]);
        }
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

    [Serializable]
    private class InteractableCallback
    {
        [SerializeField]
        private InteractableDefinition[] m_targets;

        public UnityEvent<InteractableObjectBehaviour> OnInteraction;

        public void CheckAndRaiseCallbackForTarget(InteractableObjectBehaviour obj)
        {
            if (obj == null)
            {
                return;
            }

            for (int i = 0, c = m_targets.Length; i < c; ++i)
            {
                if (obj.Definition == m_targets[i])
                {
                    OnInteraction?.Invoke(obj);
                    return;
                }
            }
        }
    }
}