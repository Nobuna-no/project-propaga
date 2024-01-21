using NobunAtelier.Gameplay;
using UnityEngine;

public interface IPropagaSpriteProvider
{
    // public void SetActiveInteractionFeedback(bool enable);
    public SpriteRenderer spriteRenderer { get; }
}

public class PropagaTransportableObject : TransportableObjectBehaviour, IPropagaSpriteProvider
{
    [SerializeField]
    private ObjectDefinition itemDefinition;

    [Header("Feedback")]
    [SerializeField] private SpriteRenderer m_visual;

    public ObjectDefinition ItemDefinition => itemDefinition;

    public SpriteRenderer spriteRenderer => m_visual;
    //public void SetActiveInteractionFeedback(bool enable)
    //{
    //    m_visual.SetActive(enable);
    //}

    protected override void Awake()
    {
        base.Awake();

        if (m_visual == null)
        {
            m_visual = GetComponentInChildren<SpriteRenderer>();
        }

        Debug.Assert(m_visual != null, this);
    }
}