using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class TerrainCamera : MonoBehaviour
{
    private Camera cam;
    [SerializeField]
    private GameObject otherMode;
    [SerializeField]
    private TerrainGrid terrainGrid;
    [SerializeField]
    private Text text;
    [SerializeField]
    private TerrainCellCollection terrainCells;
    private int currentMode;
    private string[] baseModes = new string[4]
    {
        "Toggle Availability",
        "Set Zone 1",
        "Set Zone 2",
        "Set Zone 3"
    };
    private int modeCount;
    const string instruction = " (Scroll to change mode)";

    void Start()
    {
        int definitionCount = terrainCells ? terrainCells.GetData().Count : 0;
        modeCount = baseModes.Length + definitionCount;

        cam = GetComponent<Camera>();
        if (text != null)
            text.text = GetModeText(currentMode) + instruction;
    }

    void OnEnable()
    {
        otherMode.SetActive(false);
    }

    private string GetModeText(int i)
    {
        if (i < baseModes.Length)
            return baseModes[i];

        i -= baseModes.Length;
        IReadOnlyList<TerrainCellDefinition> definitions = terrainCells.GetData();
        if (i >= definitions.Count)
            return "";

        return "Toggle " + definitions[i].name;
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            bool isDown = Input.GetMouseButtonDown(0);
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                Vector3 hitPos = hit.point;
                ref TerrainGrid.Cell cell = ref terrainGrid[hitPos];
                DoAction(ref cell, terrainGrid.GetGridCoordinates(hitPos), isDown);
            }
        }

        if (Input.mouseScrollDelta.y != 0.0f)
        {
            int sign = (int)Mathf.Sign(Input.mouseScrollDelta.y);
            currentMode = (currentMode + sign + modeCount) % modeCount;
            if (text != null)
                text.text = GetModeText(currentMode) + instruction;
        }
    }

    private void DoAction(ref TerrainGrid.Cell cell, Vector2Int gridPos, bool isDown)
    {
        switch(currentMode)
        {
            case 0:
                if (isDown) ToggleAvailability(ref cell, gridPos);
                break;
            case 1:
                SetZone(ref cell, 1);
                break;
            case 2:
                SetZone(ref cell, 2);
                break;
            case 3:
                SetZone(ref cell, 3);
                break;
            default:
                IReadOnlyList<TerrainCellDefinition> definitions = terrainCells.GetData();
                int index = GetDefinitionIndex();
                if (isDown) ToggleCellDefinition(ref cell, definitions[index]);
                break;
        }
    }

    private int GetDefinitionIndex()
    {
        return currentMode - baseModes.Length;;
    }

    private void ToggleAvailability(ref TerrainGrid.Cell cell, Vector2Int gridPos)
    {
        bool available = cell.state == TerrainGrid.CellState.Available;
        cell.state = available ? TerrainGrid.CellState.Unavailable : TerrainGrid.CellState.Available;
        terrainGrid.UpdateAdjacentCells(gridPos);
    }

    private void SetZone(ref TerrainGrid.Cell cell, int zoneId)
    {
        cell.zoneId = zoneId;
    }

    private void ToggleCellDefinition(ref TerrainGrid.Cell cell, TerrainCellDefinition definition)
    {
        cell.cellDefinitionName = cell.cellDefinitionName == definition.name ? string.Empty : definition.name;
    }

    private void OnDrawGizmos()
    {
        if (currentMode < baseModes.Length || terrainCells == null)
            return;

        IReadOnlyList<TerrainCellDefinition> definitions = terrainCells.GetData();
        int index = GetDefinitionIndex();
        if (definitions == null || index >= definitions.Count)
            return;

        TerrainCellDefinition currentCellDefinition = definitions[index];

        for (int x = 0; x < terrainGrid.Width; ++x)
        {
            for (int y = 0; y < terrainGrid.Height; ++y)
            {
                TerrainGrid.Cell cell = terrainGrid[x, y];
                if (cell.cellDefinitionName == string.Empty)
                    continue;

                Gizmos.color = cell.cellDefinitionName == currentCellDefinition.name ? Color.magenta : Color.cyan;
                Vector3 position = terrainGrid.GetCellPosition(new Vector2Int(x, y));
                float tileSize = terrainGrid.TileSize;
                Gizmos.DrawWireSphere(position, tileSize * 0.25f);
            }
        }
    }
}
