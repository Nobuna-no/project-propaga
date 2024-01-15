using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGrid : MonoBehaviour
{
    [System.Flags]
    public enum CellConnection
    {
        None = 0,
        Top = 1 << 0,
        Bottom = 1 << 1,
        Right = 1 << 2,
        Left = 1 << 3,
        All = Top | Bottom | Right | Left,
    }

    public enum CellState
    {
        Available, // Free to be occupied
        Occupied,
        Unavailable, // Cannot be occupied
    }

    public struct Cell
    {
        public CellState state;
        public GameObject occupyingObject;
        public CellConnection availableConnections; 
    }

    [SerializeField, Tooltip("In tiles.")]
    private int width = 10;
    [SerializeField, Tooltip("In tiles.")]
    private int height = 10;
    [SerializeField, Tooltip("In Unity unit.")]
    private float tileSize = 1.0f;

    // The world space origin (0, 0, 0) is the middle of the overall grid
    private float OriginOffsetX => width * 0.5f;
    private float OriginOffsetY => height * 0.5f;
    
    // The terrain is made of cells, squares with points in the middle.
    // (0, 0) is at the bottom left.
    Cell[,] cells;

    private void Awake()
    {
        cells = new Cell[width,height];
        for (int i = 0 ; i < width ; ++i)
        {
            for (int j = 0 ; j < height ; ++j)
            {
                cells[i, j].state = CellState.Available;
                CellConnection connections = CellConnection.None;
                connections |= i == 0 ? CellConnection.None : CellConnection.Left;
                connections |= i == width - 1 ? CellConnection.None : CellConnection.Right;
                connections |= j == 0 ? CellConnection.None : CellConnection.Bottom;
                connections |= j == height - 1 ? CellConnection.None : CellConnection.Top;
                cells[i, j].availableConnections = connections;
            }
        }
    }

    public Cell GetCell(Vector3 pos)
    {
        // Convert position from world space in Unity unit to world space in tiles
        // Then world space to grid space
        float x = (pos.x / tileSize) + OriginOffsetX;
        float y = (pos.z / tileSize) + OriginOffsetY;

        int coordX = Mathf.FloorToInt(Mathf.Clamp(x, 0, width)); 
        int coordY = Mathf.FloorToInt(Mathf.Clamp(y, 0, height));

        return cells[coordX, coordY];
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

    private void OnDrawGizmosSelected()
    {
        for (int i = 0 ; i < width ; ++i)
        {
            for (int j = 0 ; j < height ; ++j)
            {
                Vector3 position = GetCellPosition(new Vector2Int(i, j));
                Gizmos.DrawWireCube(position, new Vector3(tileSize, 1.0f, tileSize));
            }
        }
    }
}
