using NaughtyAttributes;
using NobunAtelier;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;

public class OmmrootNode : PoolableBehaviour
{
    private static Dictionary<Vector2Int, OmmrootNode> s_nodePerCell;

    [Header("Ommroot - Data")]
    [SerializeField] private PoolObjectDefinition m_greenAreaNodeToSpawn;
    [SerializeField] private PoolObjectDefinition m_yellowAreaNodeToSpawn;
    [SerializeField] private PoolObjectDefinition m_redAreaNodeToSpawn;

    [Header("Ommroot - Behaviour")]
    [SerializeField] private GameObject m_visual;
    [SerializeField] private InteractableObjectBehaviour m_interactableObject;
    [SerializeField] private SphereCollider m_safeZoneCollider;
    [SerializeField, MinMaxSlider(0.1f,10f)] private Vector2 m_safeZoneRadiusRange = Vector2.one;
    [SerializeField] private AnimationCurve m_safeZoneExpansionCurve = AnimationCurve.EaseInOut(0,0,1,1);
    [SerializeField, Tooltip("In seconds")]
    private float m_safeZoneExpansionDuration = 1;

    [Header("Ommroot - Splines")]
    [SerializeField] private SplineInstantiate m_topSplineInstantiate;
    [SerializeField] private SplineInstantiate m_rightSplineInstantiate;
    [SerializeField] private SplineInstantiate m_bottomSplineInstantiate;
    [SerializeField] private SplineInstantiate m_leftSplineInstantiate;

    private float m_currentTime = 0;
    private bool m_isGrowing = false;
    private Vector3 m_targetScale;
    private Vector3 m_originScale;

    private int m_nodeConnectionCount = 0;

    [RuntimeInitializeOnLoadMethod()]
    private static void Initialize()
    {
        s_nodePerCell = new Dictionary<Vector2Int, OmmrootNode>();
    }

    public void ManualInitialization()
    {
        OnReset();
        OnActivation();
    }

    protected override void OnReset()
    {
        Debug.Assert(m_topSplineInstantiate != null);
        Debug.Assert(m_rightSplineInstantiate != null);
        Debug.Assert(m_bottomSplineInstantiate != null);
        Debug.Assert(m_leftSplineInstantiate != null);

        m_topSplineInstantiate.Clear();
        m_rightSplineInstantiate.Clear();
        m_bottomSplineInstantiate.Clear();
        m_leftSplineInstantiate.Clear();

        m_topSplineInstantiate.enabled = false;
        m_rightSplineInstantiate.enabled = false;
        m_bottomSplineInstantiate.enabled = false;
        m_leftSplineInstantiate.enabled = false;

        // Reset state of the object as prefab spawning sometime behave weirdly...
        //m_visual.SetActive(true);
        //m_interactableObject.IsInteractable = true;

        //Vector2Int gridCoord = TerrainGrid.Instance.GetGridCoordinates(transform.position);
        //TerrainGrid.Instance[gridCoord].state = TerrainGrid.CellState.Occupied;
        //s_nodePerCell.Add(gridCoord, this);
        //RefreshNodeAndSurroundingNodes(gridCoord);
        //enabled = false;
    }

    protected override void OnActivation()
    {
        m_visual.SetActive(true);
        m_interactableObject.IsInteractable = true;

        Vector2Int gridCoord = TerrainGrid.Instance.GetGridCoordinates(transform.position);
        TerrainGrid.Instance[gridCoord].state = TerrainGrid.CellState.Occupied;
        s_nodePerCell.Add(gridCoord, this);
        RefreshNodeAndSurroundingNodes(gridCoord);
        enabled = false;
    }

    public bool IsTopRootAccessible()
    {
        Vector2Int gridCoord = TerrainGrid.Instance.GetGridCoordinates(transform.position);
        TerrainGrid.Cell cell = default;
        if (TerrainGrid.Instance.TryGetTopCell(gridCoord, ref cell))
        {
            return cell.state == TerrainGrid.CellState.Available;
        }
        return false;
    }

    public bool IsBottomRootAccessible()
    {
        Vector2Int gridCoord = TerrainGrid.Instance.GetGridCoordinates(transform.position);
        TerrainGrid.Cell cell = default;
        if (TerrainGrid.Instance.TryGetBottomCell(gridCoord, ref cell))
        {
            return cell.state == TerrainGrid.CellState.Available;
        }
        return false;
    }

    public bool IsLeftRootAccessible()
    {
        Vector2Int gridCoord = TerrainGrid.Instance.GetGridCoordinates(transform.position);
        TerrainGrid.Cell cell = default;
        if (TerrainGrid.Instance.TryGetLeftCell(gridCoord, ref cell))
        {
            return cell.state == TerrainGrid.CellState.Available;
        }
        return false;
    }

    public bool IsRightRootAccessible()
    {
        Vector2Int gridCoord = TerrainGrid.Instance.GetGridCoordinates(transform.position);
        TerrainGrid.Cell cell = default;
        if (TerrainGrid.Instance.TryGetRightCell(gridCoord, ref cell))
        {
            return cell.state == TerrainGrid.CellState.Available;
        }
        return false;
    }

    [Button]
    public void GrowTopRoot()
    {
        m_topSplineInstantiate.enabled = true;
        m_topSplineInstantiate.Randomize();

        SpawnNewNodeAndUpdateGrid(TerrainGrid.Instance.GetGridCoordinates(transform.position) + Vector2Int.up);
    }

    [Button]
    public void GrowBottomRoot()
    {
        m_bottomSplineInstantiate.enabled = true;
        m_bottomSplineInstantiate.Randomize();

        SpawnNewNodeAndUpdateGrid(TerrainGrid.Instance.GetGridCoordinates(transform.position) + Vector2Int.down);
    }

    [Button]
    public void GrowLeftRoot()
    {
        m_leftSplineInstantiate.enabled = true;
        m_leftSplineInstantiate.Randomize();

        SpawnNewNodeAndUpdateGrid(TerrainGrid.Instance.GetGridCoordinates(transform.position) + Vector2Int.left);
    }

    [Button]
    public void GrowRightRoot()
    {
        m_rightSplineInstantiate.enabled = true;
        m_rightSplineInstantiate.Randomize();

        SpawnNewNodeAndUpdateGrid(TerrainGrid.Instance.GetGridCoordinates(transform.position) + Vector2Int.right);
    }

    private void SpawnNewNodeAndUpdateGrid(Vector2Int gridCoord)
    {
        TerrainGrid.Instance[gridCoord].state = TerrainGrid.CellState.Occupied;
        TerrainGrid.Instance.UpdateAdjacentCells(gridCoord);
        SpawnNewNode(TerrainGrid.Instance.GetCellPosition(gridCoord), TerrainGrid.Instance[gridCoord].zoneId);
    }

    private void SpawnNewNode(Vector3 position, int areaId)
    {
        if (areaId <= 1)
        {
            OmmrootNodePool.Instance.SpawnObject(m_greenAreaNodeToSpawn, position);
        }
        else if (areaId == 2)
        {
            OmmrootNodePool.Instance.SpawnObject(m_yellowAreaNodeToSpawn, position);
        }
        else
        {
            OmmrootNodePool.Instance.SpawnObject(m_redAreaNodeToSpawn, position);
        }
    }

    private void UpdateInteractableUsability()
    {
        m_nodeConnectionCount = 0;
        if (!IsTopRootAccessible() && !IsBottomRootAccessible() &&
            !IsLeftRootAccessible() && !IsRightRootAccessible())
        {
            m_nodeConnectionCount = 4;

            m_interactableObject.enabled = false;
            m_visual.SetActive(false);
            return;
        }

        var gridCoord = TerrainGrid.Instance.GetGridCoordinates(transform.position);
        if (s_nodePerCell.TryGetValue(gridCoord, out var lnode))
        {
            m_nodeConnectionCount++;
        }
        if (s_nodePerCell.TryGetValue(gridCoord + Vector2Int.right, out var rnode) && lnode.m_interactableObject.enabled)
        {
            m_nodeConnectionCount++;
        }
        if (s_nodePerCell.TryGetValue(gridCoord + Vector2Int.up, out var unode) && unode.m_interactableObject.enabled)
        {
            m_nodeConnectionCount++;
        }
        if (s_nodePerCell.TryGetValue(gridCoord + Vector2Int.down, out var dnode) && dnode.m_interactableObject.enabled)
        {
            m_nodeConnectionCount++;
        }

        RefreshNodeSafeZoneRadius();
    }

    // Scale the safe zone based on the amount of connection
    private void RefreshNodeSafeZoneRadius()
    {
        m_isGrowing = true;
        m_originScale = m_safeZoneCollider.transform.localScale;
        m_targetScale = Vector3.one * Mathf.Lerp(m_safeZoneRadiusRange.x, m_safeZoneRadiusRange.y, m_safeZoneExpansionCurve.Evaluate((float)m_nodeConnectionCount / 4f));
        m_currentTime = 0;
        enabled = true;
    }

    private void RefreshNodeAndSurroundingNodes(Vector2Int gridCoord)
    {
        if (s_nodePerCell.TryGetValue(gridCoord + Vector2Int.left, out var lnode) && lnode.m_interactableObject.enabled)
        {
            lnode.UpdateInteractableUsability();
        }
        if (s_nodePerCell.TryGetValue(gridCoord + Vector2Int.right, out var rnode) && rnode.m_interactableObject.enabled)
        {
            rnode.UpdateInteractableUsability();
        }
        if (s_nodePerCell.TryGetValue(gridCoord + Vector2Int.up, out var unode) && unode.m_interactableObject.enabled)
        {
            unode.UpdateInteractableUsability();
        }
        if (s_nodePerCell.TryGetValue(gridCoord + Vector2Int.down, out var dnode) && dnode.m_interactableObject.enabled)
        {
            dnode.UpdateInteractableUsability();
        }

        UpdateInteractableUsability();
    }

    protected override void FixedUpdate()
    {
        if (!m_isGrowing)
        {
            enabled = false;
        }

        m_currentTime += Time.fixedDeltaTime / m_safeZoneExpansionDuration;
        m_safeZoneCollider.transform.localScale = Vector3.Slerp(m_originScale, m_targetScale, m_currentTime);

        if (m_currentTime >= 1)
        {
            m_isGrowing = false;
            enabled = false;
        }
    }
}