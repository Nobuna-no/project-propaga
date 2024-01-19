using UnityEngine;
using UnityEngine.Events;

using NobunAtelier;

public class TaskStateMachine : StateMachineComponent<GameStateDefinition, GameStateCollection>
{
    public enum Mode
    {
        TimeBased, // Progress fills as time passes
        ActionBased // Progress has to be triggered by player action
    }

    [Header("Task-related")]
    [SerializeField]
    private GameStateDefinition inProgressState;
    [SerializeField]
    private GameStateDefinition doneState;

    [SerializeField]
    private Mode mode = Mode.TimeBased;

    [SerializeField]
    private float maxValue = 100.0f;

    [NaughtyAttributes.ShowIf("mode", Mode.TimeBased)]
    [SerializeField, Tooltip("In progress per second")]
    private float initialSpeed = 1.0f;

    private float currentValue = 0.0f;
    private float currentSpeed = 1.0f;

    [SerializeField]
    private UnityEvent<float> OnProgressChanged;

    protected float CurrentValue
    {
        get => currentValue;
        set
        {
            currentValue = value;
            OnProgressChanged?.Invoke(currentValue);
        }
    }

    public float MaxValue
    {
        get => maxValue;
        set => maxValue = value;
    }

    // As a value between [0, 1]
    public float Progress
    {
        get => Mathf.Clamp01(currentValue / maxValue);
    }

    public float Speed
    {
        get => currentSpeed;
        set => currentSpeed = value;
    }

    public float InitialSpeed => initialSpeed;

    public bool IsInProgress => CurrentStateDefinition == inProgressState;
    public bool IsDone => CurrentStateDefinition == doneState;
    public bool HasNotStarted => CurrentStateDefinition == GetInitialStateDefinition();

    private void Update()
    {
        Tick(Time.deltaTime);
    }

    public override void Tick(float deltaTime)
    {
        base.Tick(deltaTime);
        if (mode == Mode.TimeBased && CurrentStateDefinition == inProgressState)
        {
            currentValue += currentSpeed * deltaTime;
            OnProgressChanged?.Invoke(Progress);
            if (currentValue >= maxValue)
                SetState(doneState);
        }
    }

    public virtual void ResetProgress()
    {
        currentValue = 0;
        currentSpeed = initialSpeed;
    }

    public void AddProgress(float amount)
    {
        if (CurrentStateDefinition == inProgressState)
        {
            currentValue += amount;
            OnProgressChanged?.Invoke(Progress);
            if (currentValue >= maxValue)
                SetState(doneState);
        }
    }

    public virtual void Begin()
    {
        SetState(inProgressState);
    }

    public virtual void End()
    {
        SetState(doneState);
    }
}