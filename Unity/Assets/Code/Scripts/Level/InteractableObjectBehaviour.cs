using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

using NaughtyAttributes;

using NobunAtelier;
using NobunAtelier.Gameplay;

public class InteractableObjectBehaviour : MonoBehaviour
{
    [SerializeField, Tooltip("If an item is required and none is provided, it will call OnInteractRejectedEvent")]
    private bool requireItem;
    [SerializeField, ShowIf("requireItem")]
    private List<ObjectDefinition> acceptedItems;
    [SerializeField, HideIf("requireItem")]
    private List<ObjectDefinition> rejectedItems;
    [Header("Events")]
    public UnityEvent<TransportableObjectBehaviour> OnInteractEvent;
    public UnityEvent OnInteractRejectedEvent;

    // Returns whether the object is interested in the item the character holds
    public virtual bool CheckDeposit(ObjectDefinition item)
    {
        List<ObjectDefinition> itemsToCheck = requireItem ? acceptedItems : rejectedItems;
        bool itemInList = itemsToCheck.Contains(item);
        return requireItem == itemInList;
    }

    // Item has been consumed, or there is no item, either way react to user action now
    public virtual void Use(TransportableObjectBehaviour item = null)
    {
        if (requireItem && item == null)
        {
            OnInteractRejectedEvent.Invoke();
            return;
        }

        OnInteractEvent?.Invoke(item);
    }
}