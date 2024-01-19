using UnityEngine;

using NobunAtelier;

public class ObjectDefinition : DataDefinition
{
    public PoolObjectDefinition poolObject;
    public PoolObjectDefinition plantedObject;
    [Tooltip("How much does the object add to plant growth if the object is added to plants, in seconds.")]
    public float growthBoost = 0.0f;
    [Tooltip("How much does it add to the progress if composted, in progress.")]
    public float compostValue = 0.0f;
}