using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

using NaughtyAttributes;

using NobunAtelier;
using NobunAtelier.Gameplay;

public class PlantStateMachine : TaskStateMachine
{
    [SerializeField]
    private PoolManager poolManager;
    [SerializeField]
    private InteractableObjectBehaviour interactable;

    private ObjectDefinition currentSeed;

    [SerializeField]
    private float dropRadius = 2.0f;

    [Header("Plant Events")]
    [SerializeField]
    private UnityEvent OnBoosting;

    protected override void Start()
    {
        base.Start();
        interactable.OnInteractEvent.AddListener(Interact);
    }

    public override void ResetProgress()
    {
        base.ResetProgress();
        currentSeed = null;
    }

    public override void SetState(GameStateDefinition newState)
    {
        base.SetState(newState);

        if (IsInProgress)
        {
            interactable.Condition = ImprovesGrowth;
        }
        else if (HasNotStarted)
        {
            interactable.Condition = IsPlanteable;
        }
    }

    private void Interact(ObjectDefinition obj)
    {
        if (IsInProgress)
        {
            AddProgress(obj.growthBoost);
            OnBoosting?.Invoke();
        }
        else
        {
            currentSeed = obj;
            MaxValue = obj.plantingTime;
            Begin();
        }
    }

    private bool IsPlanteable(ObjectDefinition obj)
    {
        return obj != null && obj.plantingTime > 0.0f;
    }

    private bool ImprovesGrowth(ObjectDefinition obj)
    {
        return obj.growthBoost > 0.0f;
    }

    [Button]
    public void DropSeeds()
    {
        if (!IsDone)
            return;
        
        if (poolManager == null || currentSeed == null || currentSeed.poolObject == null)
            return;

        poolManager.SpawnObject(currentSeed.poolObject, transform.position, dropRadius, 2);
        ResetProgress();
        SetState(GetInitialStateDefinition());
    }
}