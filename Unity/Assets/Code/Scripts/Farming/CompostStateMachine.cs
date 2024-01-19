
using UnityEngine;

using NobunAtelier;

public class CompostStateMachine : TaskStateMachine
{
    [SerializeField]
    private PoolManager poolManager;
    [SerializeField]
    private InteractableObjectBehaviour interactable;
    [SerializeField]
    private ObjectDefinition result;

    [SerializeField]
    private float dropRadius = 2.0f;

    protected override void Start()
    {
        base.Start();
        interactable.OnInteractEvent.AddListener(Interact);
    }

    public override void ResetProgress()
    {
        // Don't call the base intentionally

        CurrentValue = Mathf.Max(CurrentValue - MaxValue, 0);
        Speed = InitialSpeed;
    }

    private void Interact(ObjectDefinition obj)
    {
        if (!IsInProgress)
        {
            Begin();
        }

        AddProgress(obj.compostValue);
    }

    public void DropCompost()
    {
        if (!IsDone)
            return;
        
        if (poolManager != null && result != null && result.poolObject != null)
        {
            poolManager.SpawnObject(result.poolObject, transform.position, dropRadius, 1);
        }

        if (CurrentValue > 0.0f)
            Begin();
        else
            SetState(GetInitialStateDefinition());
    }
}
