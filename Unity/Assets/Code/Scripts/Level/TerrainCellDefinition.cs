using UnityEngine;

using NobunAtelier;

public class TerrainCellDefinition : DataDefinition
{
    public GameObject Prefab => m_prefab;

    [SerializeField]
    private GameObject m_prefab;
}