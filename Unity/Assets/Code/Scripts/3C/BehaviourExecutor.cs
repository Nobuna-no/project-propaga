using System.Collections.Generic;

using UnityEngine;

using NaughtyAttributes;

using NobunAtelier;
using NobunAtelier.Gameplay;

public interface BehaviourWithPriority
{
    public int Priority { get; }

    public bool CanBeExecuted();

    public void Execute();
}

public class BehaviourExecutor : MonoBehaviour
{
    [SerializeField]
    private List<BehaviourWithPriority> m_behaviours = new List<BehaviourWithPriority>();

    protected void Awake()
    {
        CaptureBehaviours();
    }

    public void Execute()
    {
        BehaviourWithPriority[] bestBehaviours = GetBestBehaviours();
        foreach (var behaviour in bestBehaviours)
        {
            behaviour.Execute();
        }
    }

    private static int Compare(TriggerBehaviour x, TriggerBehaviour y)
    {
        int priorityX = x is BehaviourWithPriority xB ? xB.Priority : int.MaxValue;
        int priorityY = y is BehaviourWithPriority yB ? yB.Priority : int.MaxValue;
        return priorityX.CompareTo(priorityY);
    }

    private static int Compare(BehaviourWithPriority x, BehaviourWithPriority y)
    {
        return x.Priority.CompareTo(y.Priority);
    }

    private BehaviourWithPriority[] GetBestBehaviours()
    {
        if (m_behaviours == null || m_behaviours.Count == 0)
        {
            return null;
        }

        int bestPriority = -1;
        List<BehaviourWithPriority> bestBehaviours = new List<BehaviourWithPriority>();
        // Sort ascending order (lower first).
        m_behaviours.Sort(Compare);

        for (int i = 0, c = m_behaviours.Count; i < c; i++)
        {
            BehaviourWithPriority behaviour = m_behaviours[i]; 
            if (behaviour == null || !behaviour.CanBeExecuted())
            {
                continue;
            }

            if (bestBehaviours.Count > 0 && behaviour.Priority > bestPriority)
            {
                break;
            }
            
            bestBehaviours.Add(behaviour);
            bestPriority = behaviour.Priority;
        }

        return bestBehaviours.ToArray();
    }

    private void OnValidate()
    {
        CaptureBehaviours();
    }

    [Button("Refresh modules")]
    private void CaptureBehaviours()
    {
        m_behaviours.Clear();
        m_behaviours.AddRange(GetComponents<BehaviourWithPriority>());
    }
}