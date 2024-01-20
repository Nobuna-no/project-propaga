using System;

using UnityEngine;

using NobunAtelier;

[Flags]
public enum TerrainCellConnection
{
    None = 0,
    Top = 1 << 0,
    Bottom = 1 << 1,
    Right = 1 << 2,
    Left = 1 << 3,
    All = Top | Bottom | Right | Left,
}

public enum TerrainCellState
{
    Available, // Free to be occupied
    Occupied,
    Unavailable, // Cannot be occupied
}

public class TerrainCellDefinition : DataDefinition
{
    public TerrainCellState state;
    public int zoneId;
     // Indicates which direction has an available cell.
    public TerrainCellConnection availableAdjacentCells;
    public PoolObjectDefinition occupyingObject;
    public bool hypothetical;
}