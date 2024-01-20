using NaughtyAttributes;
using NobunAtelier;
using UnityEngine;
using UnityEngine.Splines;

public class OmmrootNode : MonoBehaviour
{
    [SerializeField] private PoolObjectDefinition m_objectToSpawn;
    [SerializeField] private InteractableObjectBehaviour m_interactableObject;
    [SerializeField] private GameObject m_visual;
    [SerializeField] private SplineInstantiate m_topSplineInstantiate;
    [SerializeField] private SplineInstantiate m_rightSplineInstantiate;
    [SerializeField] private SplineInstantiate m_bottomSplineInstantiate;
    [SerializeField] private SplineInstantiate m_leftSplineInstantiate;

    private void Awake()
    {
        Debug.Assert(m_topSplineInstantiate != null);
        Debug.Assert(m_rightSplineInstantiate != null);
        Debug.Assert(m_bottomSplineInstantiate != null);
        Debug.Assert(m_leftSplineInstantiate != null);

        // Reset state of the object as prefab spawning sometime behave weirdly...
        m_visual.SetActive(true);
        m_interactableObject.Release();
    }

    private void Start()
    {
        Vector2Int gridCoord = TerrainGrid.Instance.GetGridCoordinates(transform.position);
        TerrainGrid.Instance[gridCoord].state = TerrainCellState.Occupied;

        m_topSplineInstantiate.Clear();
        m_rightSplineInstantiate.Clear();
        m_bottomSplineInstantiate.Clear();
        m_leftSplineInstantiate.Clear();
    }

    public bool IsTopRootAccessible()
    {
        Vector2Int gridCoord = TerrainGrid.Instance.GetGridCoordinates(transform.position);
        TerrainCellDefinition cell = default;
        if (TerrainGrid.Instance.TryGetTopCell(gridCoord, ref cell))
        {
            return cell.state == TerrainCellState.Available;
        }
        return false;
    }

    public bool IsBottomRootAccessible()
    {
        Vector2Int gridCoord = TerrainGrid.Instance.GetGridCoordinates(transform.position);
        TerrainCellDefinition cell = default;
        if (TerrainGrid.Instance.TryGetBottomCell(gridCoord, ref cell))
        {
            return cell.state == TerrainCellState.Available;
        }
        return false;
    }

    public bool IsLeftRootAccessible()
    {
        Vector2Int gridCoord = TerrainGrid.Instance.GetGridCoordinates(transform.position);
        TerrainCellDefinition cell = default;
        if (TerrainGrid.Instance.TryGetLeftCell(gridCoord, ref cell))
        {
            return cell.state == TerrainCellState.Available;
        }
        return false;
    }

    public bool IsRightRootAccessible()
    {
        Vector2Int gridCoord = TerrainGrid.Instance.GetGridCoordinates(transform.position);
        TerrainCellDefinition cell = default;
        if (TerrainGrid.Instance.TryGetRightCell(gridCoord, ref cell))
        {
            return cell.state == TerrainCellState.Available;
        }
        return false;
    }

    [Button]
    public void GrowTopRoot()
    {
        m_topSplineInstantiate.gameObject.SetActive(true);
        m_topSplineInstantiate.Randomize();

        Vector2Int gridCoord = TerrainGrid.Instance.GetGridCoordinates(transform.position);
        gridCoord.y += 1;

        TerrainGrid.Instance[gridCoord].state = TerrainCellState.Occupied;
        TerrainGrid.Instance.UpdateAdjacentCells(gridCoord);
        SpawnNewNode(TerrainGrid.Instance.GetCellPosition(gridCoord));
        UpdateInteractableUsability();
    }

    [Button]
    public void GrowBottomRoot()
    {
        m_bottomSplineInstantiate.gameObject.SetActive(true);
        m_bottomSplineInstantiate.Randomize();

        Vector2Int gridCoord = TerrainGrid.Instance.GetGridCoordinates(transform.position);
        gridCoord.y -= 1;
        TerrainGrid.Instance[gridCoord].state = TerrainCellState.Occupied;
        TerrainGrid.Instance.UpdateAdjacentCells(gridCoord);
        SpawnNewNode(TerrainGrid.Instance.GetCellPosition(gridCoord));
        UpdateInteractableUsability();
    }

    [Button]
    public void GrowLeftRoot()
    {
        m_leftSplineInstantiate.gameObject.SetActive(true);
        m_leftSplineInstantiate.Randomize();

        Vector2Int gridCoord = TerrainGrid.Instance.GetGridCoordinates(transform.position);
        gridCoord.x -= 1;
        TerrainGrid.Instance[gridCoord].state = TerrainCellState.Occupied;
        TerrainGrid.Instance.UpdateAdjacentCells(gridCoord);
        SpawnNewNode(TerrainGrid.Instance.GetCellPosition(gridCoord));
        UpdateInteractableUsability();
    }

    [Button]
    public void GrowRightRoot()
    {
        m_rightSplineInstantiate.gameObject.SetActive(true);
        m_rightSplineInstantiate.Randomize();

        Vector2Int gridCoord = TerrainGrid.Instance.GetGridCoordinates(transform.position);
        gridCoord.x += 1;
        TerrainGrid.Instance[gridCoord].state = TerrainCellState.Occupied;
        TerrainGrid.Instance.UpdateAdjacentCells(gridCoord);
        SpawnNewNode(TerrainGrid.Instance.GetCellPosition(gridCoord));
        UpdateInteractableUsability();
    }

    private void SpawnNewNode(Vector3 position)
    {
        OmmrootNodePool.Instance.SpawnObject(m_objectToSpawn, position);
    }

    private void UpdateInteractableUsability()
    {
        if (!IsTopRootAccessible() && !IsBottomRootAccessible() &&
            !IsLeftRootAccessible() && !IsRightRootAccessible())
        {
            m_interactableObject.enabled = false;
            m_visual.SetActive(false);
        }
    }
}