using NaughtyAttributes;
using NobunAtelier;
using NobunAtelier.Gameplay;
using UnityEngine;

public class TileSpawnerBehaviour : PoolableBehaviour
{
    [SerializeField] private TileDefinition m_tileDefinitionToSpawn;
    private TriggerBehaviour m_triggerBehaviour;

    protected override void OnReset()
    {
        if (!m_tileDefinitionToSpawn.SpawnBasedOnProximity)
        {
            return;
        }

        gameObject.layer = m_tileDefinitionToSpawn.TriggerLayer;
        gameObject.isStatic = true;
        m_triggerBehaviour = GetComponent<TriggerBehaviour>();
        Debug.Assert(m_triggerBehaviour != null, this);
        m_triggerBehaviour.OnTriggerEnterEvent += OnTriggerEnterEvent;
    }

    protected override void OnActivation()
    {
        if (m_tileDefinitionToSpawn.SpawnBasedOnProximity)
        {
            return;
        }

        PoolManager.Instance.SpawnObject(m_tileDefinitionToSpawn, transform.position);

#if UNITY_EDITOR
        DestroyImmediate(gameObject);
#else
        Destroy(gameObject);
#endif
    }

    private void OnTriggerEnterEvent()
    {
        if (!m_tileDefinitionToSpawn.SpawnBasedOnProximity)
        {
            return;
        }

        m_triggerBehaviour.OnTriggerEnterEvent -= OnTriggerEnterEvent;
        PoolManager.Instance.SpawnObject(m_tileDefinitionToSpawn, transform.position);

#if UNITY_EDITOR
        DestroyImmediate(gameObject);
#else
        Destroy(gameObject);
#endif
    }
}