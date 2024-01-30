using UnityEngine;

using NaughtyAttributes;

using NobunAtelier;

public class TerrainTileDefinition : DataDefinition
{
    public PoolObjectDefinition prefab;
    public bool useRange = false;
    [HideIf("useRange"), Range(0f, 1f)]
    public float chance = 1.0f;
    [ShowIf("useRange"), Min(0)]
    public int minCount = 0;
    [ShowIf("useRange")]
    public int maxCount = -1;
}