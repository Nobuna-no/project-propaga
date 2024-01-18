using NobunAtelier.Gameplay;

public class PropagaPickupBehaviour : PickupBehaviour, BehaviourWithPriority
{
    public int Priority => 1;

    public bool CanBeExecuted()
    {
        // Should check if gatherable object is pickable
        return enabled && GatherableObjects.Count > 0;
    }

    public void Execute()
    {
        TryGather();
    }
}