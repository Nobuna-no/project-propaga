using UnityEngine;
using UnityEngine.Events;

using NobunAtelier;
using NobunAtelier.Gameplay;

public class FarmingObject : PoolableBehaviour
{
    private FarmingSpot sourceSpot;

    [SerializeField]
    private UnityEvent m_OnInit;

    [SerializeField]
    private UnityEvent m_OnReset;

    public void Init(FarmingSpot origin)
    {
        sourceSpot = origin;
        m_OnInit?.Invoke();
    }

    protected override void OnCreation()
    {
        m_OnReset?.Invoke();
        sourceSpot = null;
    }

    public void Despawn()
    {
        sourceSpot.Despawn(this);
        sourceSpot = null;
    }

    public TransportableObjectBehaviour SpawnSeed(ObjectDefinition obj, Vector3 position)
    {
        if (sourceSpot?.PoolManager == null || obj?.poolObject == null)
            return null;

        return sourceSpot.PoolManager.SpawnObject(obj.poolObject, position) as TransportableObjectBehaviour;
    }
}