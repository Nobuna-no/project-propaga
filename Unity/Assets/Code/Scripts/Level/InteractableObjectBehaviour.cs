using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

using NaughtyAttributes;

public class InteractableObjectBehaviour : MonoBehaviour
{
    [SerializeField, Tooltip("If an item is required and none is provided, it will call OnInteractRejectedEvent")]
    private bool requireItem;
    [SerializeField, ShowIf("requireItem")]
    private List<ObjectDefinition> acceptedItems;
    [SerializeField, HideIf("requireItem")]
    private List<ObjectDefinition> rejectedItems;
    [Header("Events")]
    public Predicate<ObjectDefinition> Condition;
    public UnityEvent<ObjectDefinition> OnInteractEvent;

    // Returns whether the object is interested in the item the character holds
    public virtual bool CheckInput(ObjectDefinition item)
    {
        if (item == null)
            return !requireItem;
        
        List<ObjectDefinition> itemsToCheck = requireItem ? acceptedItems : rejectedItems;
        bool itemInList = itemsToCheck.Contains(item);
        bool itemOk = requireItem == itemInList;

        if (Condition != null)
            itemOk &= Condition.Invoke(item);
        
        return itemOk;
    }

    // Item has been consumed, or there is no item, either way react to user action now
    public virtual void Use(ObjectDefinition item = null)
    {
        OnInteractEvent?.Invoke(item);
    }
}