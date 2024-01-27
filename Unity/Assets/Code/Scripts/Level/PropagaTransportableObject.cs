using NobunAtelier.Gameplay;
using UnityEngine;
using UnityEngine.Events;

public interface IPropagaSpriteProvider
{
    public SpriteRenderer spriteRenderer { get; }
}

public class PropagaTransportableObject : TransportableObjectBehaviour, IPropagaSpriteProvider
{
    [Header("PROPAGA Transportable Object")]
    [SerializeField] private ObjectDefinition itemDefinition;
    [SerializeField, Tooltip("Can the object be consumed by an interactable?")]
    private bool m_canBeConsumed = true;

    [Header("PROPAGA Feedback")]
    [SerializeField] private SpriteRenderer m_visual;

    public ObjectDefinition ItemDefinition => itemDefinition;

    public SpriteRenderer spriteRenderer => m_visual;

    public UnityEvent OnObjectActivation;
    public UnityEvent OnObjectDeactivation;

    public bool CanBeConsumed
    {
        get => m_canBeConsumed;
        set => m_canBeConsumed = value;
    }

    protected override void Awake()
    {
        base.Awake();

        if (m_visual == null)
        {
            m_visual = GetComponentInChildren<SpriteRenderer>();
        }
    }

    protected override void OnActivation()
    {
        base.OnActivation();
        OnObjectActivation?.Invoke();
    }

    protected override void OnDeactivation()
    {
        base.OnDeactivation();
        OnObjectDeactivation?.Invoke();
    }

}