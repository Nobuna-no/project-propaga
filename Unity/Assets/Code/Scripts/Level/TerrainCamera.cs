using UnityEngine;
using UnityEngine.UI;

public class TerrainCamera : MonoBehaviour
{
    public enum EditMode
    {
        ToggleAvailability,
        SetZone1,
        SetZone2,
        SetZone3,
        Count
    }

    private Camera cam;
    [SerializeField]
    private GameObject otherMode;
    [SerializeField]
    private TerrainGrid terrainGrid;
    [SerializeField]
    private Text text;
    private EditMode currentMode;
    const string instruction = " (Scroll to change mode)";
    
    void Start()
    {
        cam = GetComponent<Camera>();
        if (text != null)
            text.text = currentMode.ToString() + instruction;
    }

    void OnEnable()
    {
        otherMode.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                Vector3 hitPos = hit.point;
                TerrainCellDefinition cell = terrainGrid[hitPos];
                DoAction(cell, terrainGrid.GetGridCoordinates(hitPos));
            }
        }

        if (Input.mouseScrollDelta.y != 0.0f)
        {
            int modeCount = (int)EditMode.Count;
            int sign = (int)Mathf.Sign(Input.mouseScrollDelta.y);
            currentMode = (EditMode)((int)(currentMode + sign + modeCount) % modeCount);
            if (text != null)
                text.text = currentMode.ToString() + instruction;
        }
    } 

    private void DoAction(TerrainCellDefinition cell, Vector2Int gridPos)
    {
        switch(currentMode)
        {
            case EditMode.ToggleAvailability:
                ToggleAvailability(cell, gridPos);
                break;
            case EditMode.SetZone1:
                SetZone(cell, 1);
                break;
            case EditMode.SetZone2:
                SetZone(cell, 2);
                break;
            case EditMode.SetZone3:
                SetZone(cell, 3);
                break;
        }
    }

    private void ToggleAvailability(TerrainCellDefinition cell, Vector2Int gridPos)
    {
        bool available = cell.state == TerrainCellState.Available;
        cell.state = available ? TerrainCellState.Unavailable : TerrainCellState.Available;
        terrainGrid.UpdateAdjacentCells(gridPos);
    }

    private void SetZone(TerrainCellDefinition cell, int zoneId)
    {
        cell.zoneId = zoneId;
    }
}
