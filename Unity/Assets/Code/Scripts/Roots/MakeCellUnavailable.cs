using UnityEngine;

public class MakeCellUnavailable : MonoBehaviour
{
    void Start()
    {
        TerrainGrid grid = TerrainGrid.Instance;
        Vector2Int cellCoord = grid.GetGridCoordinates(transform.position);
        ref TerrainGrid.Cell cell = ref grid[cellCoord];
        cell.state = TerrainGrid.CellState.Unavailable;
        grid.UpdateAdjacentCells(cellCoord);
    }
}
