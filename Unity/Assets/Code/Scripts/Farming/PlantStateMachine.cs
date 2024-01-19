using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

using NaughtyAttributes;

using NobunAtelier;
using NobunAtelier.Gameplay;

public class PlantStateMachine : TaskStateMachine
{
    [SerializeField]
    private FarmingObject farmingObj;
    [SerializeField]
    private InteractableObjectBehaviour interactable;
    [SerializeField]
    private SocketStorageBehaviour storage;

    [SerializeField]
    private ObjectDefinition currentSeed;

    [Header("Plant Events")]
    [SerializeField]
    private UnityEvent OnBoosting;

    protected override void Start()
    {
        base.Start();
        interactable.Condition = ImprovesGrowth;
    }

    public override void ResetProgress()
    {
        base.ResetProgress();
        currentSeed = null;
    }

    public void Interact(ObjectDefinition obj)
    {
        if (IsInProgress)
        {
            AddProgress(obj.growthBoost);
            OnBoosting?.Invoke();
        }
    }

    private bool ImprovesGrowth(ObjectDefinition obj)
    {
        return obj.growthBoost > 0.0f;
    }

    public void AddSeedsToStorage()
    {
        if (!IsDone)
            return;

        IReadOnlyList<Transform> sockets = storage.Sockets;
        if (sockets.Count != 2)
            return;

        Transform socketR = sockets[0];
        Transform socketL = sockets[1];
        TransportableObjectBehaviour seedR = farmingObj.SpawnSeed(currentSeed, socketR.position);
        if (seedR)
        {
            seedR.Pick();
            storage.ItemTryAdd(seedR);
        }

        TransportableObjectBehaviour seedL = farmingObj.SpawnSeed(currentSeed, socketL.position);
        if (seedL)
        {
            seedL.Pick();
            storage.ItemTryAdd(seedL);
        }
    }

    [Button]
    public void DropSeeds()
    {
        if (!IsDone)
            return;
        
        farmingObj.SpawnSeed(currentSeed, transform.position);
        farmingObj.SpawnSeed(currentSeed, transform.position);
        SetState(GetInitialStateDefinition());
    }
}