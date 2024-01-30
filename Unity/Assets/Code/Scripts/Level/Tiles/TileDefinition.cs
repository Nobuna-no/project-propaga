using UnityEngine;
using NobunAtelier;
using NaughtyAttributes;

public class TileDefinition : PoolObjectDefinition
{
    [SerializeField] private bool m_proximitySpawning = false;
    [SerializeField, Layer, ShowIf("m_proximitySpawning")]
    private int m_spawnTriggerLayer;

    public bool SpawnBasedOnProximity => m_proximitySpawning;
    public int TriggerLayer => m_spawnTriggerLayer;
}