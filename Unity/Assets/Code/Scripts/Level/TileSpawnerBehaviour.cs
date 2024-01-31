using NaughtyAttributes;
using NobunAtelier;
using NobunAtelier.Gameplay;
using UnityEngine;
using System.Collections;

public class TileSpawnerBehaviour : MonoBehaviour
{
    private TriggerBehaviour m_triggerBehaviour;

    public TileDefinition TileDefinition
    {
        get; set;
    }

    protected void Awake()
    {
        if (TileDefinition == null || !TileDefinition.SpawnBasedOnProximity)
        {
            return;
        }

        gameObject.layer = TileDefinition.TriggerLayer;
        gameObject.isStatic = true;
        m_triggerBehaviour = GetComponent<TriggerBehaviour>();
        Debug.Assert(m_triggerBehaviour != null, this);
        m_triggerBehaviour.OnTriggerEnterEvent += OnTriggerEnterEvent;
    }

    protected void Start()
    {
        if (TileDefinition.SpawnBasedOnProximity)
        {
            return;
        }

        PoolManager.Instance.SpawnObject(TileDefinition, transform.position);

#if UNITY_EDITOR
        DestroyImmediate(gameObject);
#else
        Destroy(gameObject);
#endif
    }

    private void OnTriggerEnterEvent()
    {
        if (!TileDefinition.SpawnBasedOnProximity)
        {
            return;
        }

        m_triggerBehaviour.OnTriggerEnterEvent -= OnTriggerEnterEvent;
        PoolManager.Instance.SpawnObject(TileDefinition, transform.position);

#if UNITY_EDITOR
        DestroyImmediate(gameObject);
#else
        Destroy(gameObject);
#endif
    }
}