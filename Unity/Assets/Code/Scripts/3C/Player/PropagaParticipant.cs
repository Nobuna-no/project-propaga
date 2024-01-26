
using NobunAtelier;
using UnityEngine;
using UnityEngine.InputSystem;

public class PropagaParticipant : PlayerInputParticipant, IPropagaPlayer
{
    public PropagaPlayerDefinition DataDefinition { get; set; }
}
