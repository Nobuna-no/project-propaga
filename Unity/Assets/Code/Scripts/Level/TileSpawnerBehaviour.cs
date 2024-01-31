using NobunAtelier;
using NobunAtelier.Gameplay;
using System.Collections;
using UnityEngine;

public class TileSpawnerBehaviour : MonoBehaviour
{
    private TriggerBehaviour m_triggerBehaviour;

    public TileDefinition TileDefinition { get; set; }

    private IEnumerator Start()
    {
        yield return null;

        Debug.Assert(TileDefinition != null, this);

        if (!TileDefinition.SpawnBasedOnProximity)
        {
            PoolManager.Instance.SpawnObject(TileDefinition, transform.position);
            Destroy(gameObject);
        }
        else
        {
            gameObject.layer = TileDefinition.TriggerLayer;
            gameObject.isStatic = true;
            m_triggerBehaviour = GetComponent<TriggerBehaviour>();
            Debug.Assert(m_triggerBehaviour != null, this);
            m_triggerBehaviour.OnTriggerEnterEvent += OnTriggerEnterEvent;
        }
    }

    private void OnTriggerEnterEvent()
    {
        if (!TileDefinition.SpawnBasedOnProximity)
        {
            return;
        }

        m_triggerBehaviour.OnTriggerEnterEvent -= OnTriggerEnterEvent;
        StartCoroutine(SpawnAndDestroy_Coroutine());
    }

    private IEnumerator SpawnAndDestroy_Coroutine()
    {
        yield return new WaitForFixedUpdate();

        PoolManager.Instance.SpawnObject(TileDefinition, transform.position);
        Destroy(gameObject);
    }
}