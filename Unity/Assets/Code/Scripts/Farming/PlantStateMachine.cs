using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

using NaughtyAttributes;

using NobunAtelier;
using NobunAtelier.Gameplay;
using UnityEngine.UIElements;

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

        if (interactable)
        {
            interactable.Condition = ImprovesGrowth;
        }
    }

    public override void ResetProgress()
    {
        base.ResetProgress();
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

    public void AttackObjectToSocket(PropagaTransportableObject obj)
    {
        storage.ItemTryAdd(obj);
    }

    public void DropObjectFromSocket()
    {
        storage.ThrowFirstItem();
    }

    //public void AddSeedsToStorage()
    //{
    //    if (!IsDone)
    //        return;

    //    IReadOnlyList<Transform> sockets = storage.Sockets;
    //    if (sockets.Count != 2)
    //        return;

    //    Transform socketR = sockets[0];
    //    Transform socketL = sockets[1];
    //    TransportableObjectBehaviour seedR = farmingObj.SpawnSeed(currentSeed, socketR.position);
    //    if (seedR)
    //    {
    //        seedR.Pick();
    //        storage.ItemTryAdd(seedR);
    //    }

    //    TransportableObjectBehaviour seedL = farmingObj.SpawnSeed(currentSeed, socketL.position);
    //    if (seedL)
    //    {
    //        seedL.Pick();
    //        storage.ItemTryAdd(seedL);
    //    }
    //}

    //[Button]
    //public void DropSeeds()
    //{
    //    if (!IsDone)
    //        return;

    //    farmingObj.SpawnSeed(currentSeed, transform.position);
    //    farmingObj.SpawnSeed(currentSeed, transform.position);
    //    SetState(GetInitialStateDefinition());
    //}
    bool m_dropRight = false;
    [Button]
    public void DropOneSeed()
    {
        // farmingObj.SpawnSeed(currentSeed, storage.Sockets[0].position);
        Transform socket = storage.Sockets[m_dropRight ? 0 : 1];
        TransportableObjectBehaviour seedR = farmingObj.SpawnSeed(currentSeed, socket.position);
        if (seedR)
        {
            seedR.Pick();
            storage.ItemTryAdd(seedR);
        }

        storage.FirstItemDrop();
        //AddSeedsToStorage();
        //TransportableObjectBehaviour seedR = farmingObj.SpawnSeed(currentSeed, storage.Sockets[0].position);
        //if (seedR)
        //{
        //    seedR.Pick();
        //    storage.ItemTryAdd(seedR);
        //}
    }

    public void PopulateSocketsWithSeeds()
    {
        if (!IsDone)
            return;

        int randomCount = Random.Range(1, storage.Sockets.Count + 1);

        for (int i = 0; i < randomCount; i++)
        {
            Transform s = storage.Sockets[i];
            TransportableObjectBehaviour seed = PoolManager.Instance.SpawnObject(currentSeed.poolObject, s.position) as TransportableObjectBehaviour;
            if (seed)
            {
                seed.Pick();
                storage.ItemTryAdd(seed);
            }
        }
    }
}