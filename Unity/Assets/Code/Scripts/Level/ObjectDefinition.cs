using UnityEngine;

using NobunAtelier;

public class ObjectDefinition : DataDefinition
{
    public PoolObjectDefinition poolObject;
    [Tooltip("How much time does the object take if planted, in seconds.")]
    public float plantingTime = 0.0f;
    [Tooltip("How much does the object add to plant growth if the object is added to plants, in seconds.")]
    public float growthBoost = 0.0f;
    [Tooltip("How much does it add to the progress if composted, in fraction of progress."), Range(0.0f, 1.0f)]
    public float compostValue = 0.0f;
}