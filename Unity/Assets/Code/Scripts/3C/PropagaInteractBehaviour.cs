using System.Collections.Generic;

using UnityEngine;

using NaughtyAttributes;

using NobunAtelier.Gameplay;

public class PropagaInteractBehaviour : TriggerBehaviour, BehaviourWithPriority
{
    [SerializeField] private SocketStorageBehaviour m_storageComponent;
    public List<InteractableObjectBehaviour> InteractableObjects => m_baseInteractableObjects;

    public int Priority => 0;

    private List<InteractableObjectBehaviour> m_baseInteractableObjects = new List<InteractableObjectBehaviour>();

    public event System.Action OnInteractableObjectAdded;

    public event System.Action OnInteractableObjectRemoved;

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

        if (m_storageComponent.ItemTryPeekFirst(out var obj))
        {
            ObjectDefinition itemDef = (obj as PropagaTransportableObject)?.ItemDefinition;
            if (interactableObj.CheckDeposit(itemDef))
            {
                m_storageComponent.ItemTryConsume(out var objRef);
                interactableObj.Use(obj);
                obj.IsActive = false; // Remove the object
            }
            else
            {
                interactableObj.Use();
            }
        } 
        else
        {
            interactableObj.Use();
        }
    }

    protected override void OnTriggerEnter(Collider other)
    {
        InteractableObjectBehaviour gatherable = other.GetComponent<InteractableObjectBehaviour>();
        if (gatherable != null && !m_baseInteractableObjects.Contains(gatherable))
        {
            m_baseInteractableObjects.Add(gatherable);
            OnInteractableObjectAdded?.Invoke();
        }
    }

    protected override void OnTriggerExit(Collider other)
    {
        InteractableObjectBehaviour gatherable = other.GetComponent<InteractableObjectBehaviour>();
        if (gatherable != null && m_baseInteractableObjects.Contains(gatherable))
        {
            m_baseInteractableObjects.Remove(gatherable);
            OnInteractableObjectRemoved?.Invoke();
        }
    }

    public bool CanBeExecuted()
    {
        return enabled && m_baseInteractableObjects.Count > 0;
    }

    public void Execute()
    {
        TryInteract();
    }
}