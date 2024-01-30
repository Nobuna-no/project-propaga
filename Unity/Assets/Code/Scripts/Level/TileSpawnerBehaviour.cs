using NaughtyAttributes;
using NobunAtelier;
using NobunAtelier.Gameplay;
using UnityEngine;

public class TileSpawnerBehaviour : PoolableBehaviour
{
    [SerializeField] private PoolObjectDefinition m_poolableToSpawn;
    public PoolObjectDefinition PoolableToSpawn => m_poolableToSpawn;

    [SerializeField] private bool m_proximitySpawning = false;

    [SerializeField, Layer, ShowIf("m_spawnOnProximity")]
    private int m_spawnTriggerLayer;

    private TriggerBehaviour m_triggerBehaviour;

    protected override void OnReset()
    {
        if (m_proximitySpawning)
        {
            gameObject.layer = m_spawnTriggerLayer;
            gameObject.isStatic = true;
            m_triggerBehaviour = GetComponent<TriggerBehaviour>();
            m_triggerBehaviour.OnTriggerEnterEvent += OnTriggerEnterEvent;
        }
    }

    protected override void OnActivation()
    {
        if (m_proximitySpawning)
        {
            return;
        }

        PoolManager.Instance.SpawnObject(m_poolableToSpawn, transform.position);

#if UNITY_EDITOR
        DestroyImmediate(gameObject);
#else
        Destroy(gameObject);
#endif
    }

    private void OnTriggerEnterEvent()
    {
        if (!m_proximitySpawning)
        {
            return;
        }

        m_triggerBehaviour.OnTriggerEnterEvent -= OnTriggerEnterEvent;
        PoolManager.Instance.SpawnObject(PoolableToSpawn, transform.position);

#if UNITY_EDITOR
        DestroyImmediate(gameObject);
#else
        Destroy(gameObject);
#endif
    }
}