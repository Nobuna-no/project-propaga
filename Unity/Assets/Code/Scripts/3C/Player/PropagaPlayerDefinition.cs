using UnityEngine;
using NobunAtelier;

public class PropagaPlayerDefinition : DataDefinition
{
    [SerializeField] private Material m_playerMaterial;
    [SerializeField] private Sprite m_playerAltar;

    public Material PlayerMaterial => m_playerMaterial;
    public Sprite PlayerAltar => m_playerAltar;
}