using UnityEngine;

using NobunAtelier;
using NobunAtelier.Gameplay;

public class PropagaTransportableObject : TransportableObjectBehaviour
{
    [SerializeField]
    private ObjectDefinition itemDefinition;

    public ObjectDefinition ItemDefinition => itemDefinition;
}