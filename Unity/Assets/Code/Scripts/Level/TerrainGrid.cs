using NaughtyAttributes;
using System;
using System.IO;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

/// <example>
///Vector2Int gridCoord = terrainGrid.GetGridCoordinates(position);
///ref TerrainGrid.Cell currentCell = ref terrainGrid[gridCoord];
///currentCell.state = TerrainGrid.CellState.Occupied;
///terrainGrid.UpdateAdjacentCells(gridCoord);
///
/// bool isLeftCellAvailable = currentCell.availableAdjacentCells.HasFlag(TerrainGrid.CellConnection.Left);
/// OR
/// TerrainGrid.Cell? leftCell = terrainGrid.GetLeftCell(gridCoord);
/// bool isLeftCellAvailable = leftCell.HasValue ? leftCell.Value.state == TerrainGrid.CellState.Available : false;
/// </example>
public class TerrainGrid : Singleton<TerrainGrid>
{
    [Serializable]
    private struct TerrainData
    {
        public int width;
        public int height;
        public TerrainCellDefinition[] cells;
    }

    [Header("Ommworld")]
    [SerializeField]
    private TextAsset importAsset;
    [SerializeField, Tooltip("In tiles.")]
    private int width = 10;

    [SerializeField, Tooltip("In tiles.")]
    private int height = 10;

    [SerializeField, Tooltip("In Unity unit.")]
    private float tileSize = 1.0f;

    [SerializeField]
    private bool m_drawGizmos = true;

    // The world space origin (0, 0, 0) is the middle of the overall grid
    private float OriginOffsetX => width * 0.5f;

    private float OriginOffsetY => height * 0.5f;
    public float TileSize => tileSize;

    // The terrain is made of cells, squares with points in the middle.
    // (0, 0) is at the bottom left.
    private TerrainCellDefinition[,] cells;

    public TerrainCellDefinition this[int x, int y]
    {
        get => cells[x, y];
        set => cells[x, y] = value;
    }

    public TerrainCellDefinition this[Vector3 position]
    {
        get
        {
            Vector2Int coord = GetGridCoordinates(position);
            return cells[coord.x, coord.y];
        }
    }

    public TerrainCellDefinition this[Vector2Int coord]
    {
        get
        {
            return cells[coord.x, coord.y];
        }
    }

    public int Width => width;
    public int Height => height;

    public bool TryGetTopCell(Vector2Int coord, ref TerrainCellDefinition cell)
    {
        int y = coord.y + 1;
        if (y >= height)
        {
            return false;
        }

        cell = cells[coord.x, y];
        return true;
    }

    public TerrainCellDefinition GetTopCell(Vector2Int coord)
    {
        int y = coord.y + 1;
        if (y >= height)
        {
            return null;
        }

        return cells[coord.x, y];
    }

    public bool TryGetBottomCell(Vector2Int coord, ref TerrainCellDefinition cell)
    {
        int y = coord.y - 1;
        if (y < 0)
        {
            return false;
        }

        cell = cells[coord.x, y];
        return true;
    }

    public TerrainCellDefinition GetBottomCell(Vector2Int coord)
    {
        int y = coord.y - 1;
        if (y < 0)
        {
            return null;
        }

        return cells[coord.x, y];
    }

    public bool TryGetRightCell(Vector2Int coord, ref TerrainCellDefinition cell)
    {
        int x = coord.x + 1;
        if (x >= width)
        {
            return false;
        }

        cell = cells[x, coord.y];
        return true;
    }

    public TerrainCellDefinition GetRightCell(Vector2Int coord)
    {
        int x = coord.x + 1;
        if (x >= width)
        {
            return null;
        }

        return cells[x, coord.y];
    }

    public bool TryGetLeftCell(Vector2Int coord, ref TerrainCellDefinition cell)
    {
        int x = coord.x - 1;
        if (x < 0)
        {
            return false;
        }

        cell = cells[x, coord.y];
        return true;
    }

    public TerrainCellDefinition GetLeftCell(Vector2Int coord)
    {
        int x = coord.x - 1;
        if (x < 0)
        {
            return null;
        }

        return cells[x, coord.y];
    }

    protected override void OnSingletonAwake()
    {
        cells = new TerrainCellDefinition[width, height];
        if (importAsset == null)
            Reset();
        else
            Import();
    }

    [Button]
    private void Reset()
    {
        for (int i = 0; i < width; ++i)
        {
            for (int j = 0; j < height; ++j)
            {
                cells[i, j].state = TerrainCellState.Available;
                TerrainCellConnection connections = TerrainCellConnection.None;
                connections |= i == 0 ? TerrainCellConnection.None : TerrainCellConnection.Left;
                connections |= i == width - 1 ? TerrainCellConnection.None : TerrainCellConnection.Right;
                connections |= j == 0 ? TerrainCellConnection.None : TerrainCellConnection.Bottom;
                connections |= j == height - 1 ? TerrainCellConnection.None : TerrainCellConnection.Top;
                cells[i, j].availableAdjacentCells = connections;
            }
        }
    }

    // Returns the gid coordinates that contains this position (or the closest)
    public Vector2Int GetGridCoordinates(Vector3 pos)
    {
        // Convert position from world space in Unity unit to world space in tiles
        // Then world space to grid space
        float x = (pos.x / tileSize) + OriginOffsetX;
        float y = (pos.z / tileSize) + OriginOffsetY;

        return new Vector2Int
        {
            x = Mathf.FloorToInt(Mathf.Clamp(x, 0, width)),
            y = Mathf.FloorToInt(Mathf.Clamp(y, 0, height))
        };
    }

    // Returns the position in the middle of the cell
    public Vector3 GetCellPosition(Vector2Int coord)
    {
        Vector3 position = new Vector3
        {
            x = (coord.x - OriginOffsetX + 0.5f) * tileSize,
            z = (coord.y - OriginOffsetY + 0.5f) * tileSize
        };

        return position;
    }

    // Given the coord, updates all the adjacent cells (but not the center) to availability of the center
    public void UpdateAdjacentCells(Vector2Int coord)
    {
        TerrainCellDefinition currentCell = cells[coord.x, coord.y];
        bool isAvailable = currentCell.state == TerrainCellState.Available;

        if (coord.x > 0)
            SetConnection(coord.x - 1, coord.y, TerrainCellConnection.Right, isAvailable);

        if (coord.y > 0)
            SetConnection(coord.x, coord.y - 1, TerrainCellConnection.Top, isAvailable);

        if (coord.x < width - 1)
            SetConnection(coord.x + 1, coord.y, TerrainCellConnection.Left, isAvailable);

        if (coord.y < height - 1)
            SetConnection(coord.x, coord.y + 1, TerrainCellConnection.Bottom, isAvailable);
    }

    public void SetConnection(int coordX, int coordY, TerrainCellConnection connect, bool enabled)
    {
        if (enabled)
            cells[coordX, coordY].availableAdjacentCells |= connect;
        else
            cells[coordX, coordY].availableAdjacentCells &= ~connect;
    }

    private void OnDrawGizmos()
    {
        if (!m_drawGizmos)
        {
            return;
        }

        for (int i = 0; i < width; ++i)
        {
            for (int j = 0; j < height; ++j)
            {
                Color cellColor;
                switch (cells == null ? 0 : cells[i, j].zoneId)
                {
                    case 1: cellColor = Color.green; break;
                    case 2: cellColor = Color.yellow; break;
                    case 3: cellColor = Color.red; break;
                    default: cellColor = Color.white; break;
                }
                cellColor.a = 0.1f;
                Gizmos.color = cellColor;
                Vector3 position = GetCellPosition(new Vector2Int(i, j));
                Gizmos.DrawCube(position, new Vector3(tileSize, 2.0f, tileSize));
                cellColor.a = 1.0f;

                switch (cells == null ? TerrainCellState.Available : cells[i, j].state)
                {
                    case TerrainCellState.Occupied: cellColor = Color.red; break;
                    case TerrainCellState.Unavailable: cellColor = Color.white; break;
                    default: cellColor = Color.green; break;
                }
                Gizmos.color = cellColor;
                Gizmos.DrawSphere(position, tileSize * 0.025f);

                float halfSize = tileSize * 0.5f;
                Gizmos.color = cells == null || !cells[i, j].availableAdjacentCells.HasFlag(TerrainCellConnection.Left) ? Color.red : Color.black;
                Gizmos.DrawLine(position, position + Vector3.left * halfSize);
                Gizmos.color = cells == null || !cells[i, j].availableAdjacentCells.HasFlag(TerrainCellConnection.Right) ? Color.red : Color.black;
                Gizmos.DrawLine(position, position + Vector3.right * halfSize);
                Gizmos.color = cells == null || !cells[i, j].availableAdjacentCells.HasFlag(TerrainCellConnection.Bottom) ? Color.red : Color.black;
                Gizmos.DrawLine(position, position + Vector3.back * halfSize);
                Gizmos.color = cells == null || !cells[i, j].availableAdjacentCells.HasFlag(TerrainCellConnection.Top) ? Color.red : Color.black;
                Gizmos.DrawLine(position, position + Vector3.forward * halfSize);
            }
        }
    }

    private void Import()
    {
        if (importAsset == null)
            return;

        string importJson = importAsset.text;
        TerrainData importData = JsonUtility.FromJson<TerrainData>(importJson);
        if (width != importData.width || height != importData.height)
        {
            Debug.Log("Incompatible size in import terrain data.");
        }
        
        for (int i = 0; i < width; ++i)
        {
            for (int j = 0; j < height; ++j)
            {
                cells[i, j] = importData.cells[i * height + j];
            }
        }
    }

#if UNITY_EDITOR
    [Button]
    private void Export()
    {
        string path = EditorUtility.SaveFilePanel("Save terrain preset", "", "TerrainPreset.json", "json").NullIfEmpty();
        if (path == null)
            return;

        Debug.Log($"Saving to {path}");
        TerrainData exportData = new TerrainData
        {
            width = width,
            height = height,
            cells = new TerrainCellDefinition[width * height],
        };

        for (int i = 0; i < width; ++i)
        {
            for (int j = 0; j < height; ++j)
            {
                exportData.cells[i * height + j] = cells[i, j];
            }
        }

        string exportJson = JsonUtility.ToJson(exportData);
        File.WriteAllText(path, exportJson);
    }
#endif
}