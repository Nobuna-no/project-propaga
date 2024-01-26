using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractionInfo
{
    public ObjectDefinition ObjectDefinition;
    public PropagaTransportableObject Target;
}

public class InteractableObjectBehaviour : MonoBehaviour, IPropagaSpriteProvider
{
    [SerializeField, Required]
    private InteractableDefinition m_definition;

    [SerializeField]
    private bool m_isInteractable = true;

    [SerializeField, Tooltip("If an item is required and none is provided, it will call OnInteractRejectedEvent")]
    private bool requireItem;

    [SerializeField, ShowIf("requireItem")]
    private List<ObjectDefinition> acceptedItems;

    [SerializeField, HideIf("requireItem")]
    private List<ObjectDefinition> rejectedItems;

    [Header("Events")]
    public Predicate<ObjectDefinition> Condition;

    public UnityEvent<ObjectDefinition> OnInteractEvent;
    public UnityEvent<InteractionInfo> OnInteractionEvent;

    public InteractableDefinition Definition => m_definition;



    [Header("Feedback")]

    [SerializeField] private SpriteRenderer m_visual;
    public SpriteRenderer spriteRenderer => m_visual;
    public bool RequireItem => requireItem;

    public bool IsInteractable
    {
        get => m_isInteractable;
        set => m_isInteractable = value;
    }

    // Returns whether the object is interested in the item the character holds
    public virtual bool CheckInput(ObjectDefinition item)
    {
        if (isActiveAndEnabled == false || !m_isInteractable)
        {
            return false;
        }

        if (item == null)
        {
            return !requireItem;
        }

        List<ObjectDefinition> itemsToCheck = requireItem ? acceptedItems : rejectedItems;
        bool itemInList = itemsToCheck.Contains(item);
        bool itemOk = requireItem == itemInList;

        if (Condition != null)
        {
            itemOk &= Condition.Invoke(item);
        }

        return itemOk;
    }

    // Item has been consumed, or there is no item, either way react to user action now
    public virtual void Use(PropagaTransportableObject transportableObject, ObjectDefinition item = null)
    {
        OnInteractEvent?.Invoke(item);
        OnInteractionEvent?.Invoke(new InteractionInfo()
        {
            ObjectDefinition = item,
            Target = transportableObject
        }); ;
    }

    [Button]
    public void Lock()
    {
        m_isInteractable = false;
    }

    [Button]
    public void Release()
    {
        m_isInteractable = true;
    }

    //public void SetActiveInteractionFeedback(bool enable)
    //{
    //    m_interactionFeedback.SetActive(enable);
    //}

    private void Awake()
    {
        if (m_visual == null)
        {
            m_visual = transform.parent.GetComponentInChildren<SpriteRenderer>();
        }

        Debug.Assert(m_visual != null, this);
    }
}