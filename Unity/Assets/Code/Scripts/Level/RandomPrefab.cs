using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Splines;

using NaughtyAttributes;

[System.Serializable]
public class RandomTile
{
    public TerrainTileDefinition tile;
    public float chance;
}

public enum LastPickMode
{
    Nothing,
    PickMostCommon,
    PickLeastCommon
}

public static class TileManager
{    
    private static Dictionary<TerrainTileDefinition, int> spawnedTiles;
    private static int maxTileCount;
    private static int currentTileCount;

    [RuntimeInitializeOnLoadMethod()]
    private static void Initialize()
    {
        spawnedTiles = new Dictionary<TerrainTileDefinition, int>();
        currentTileCount = 0;
    }

    public static void RecordSpawnedTile(TerrainTileDefinition tile)
    {
        if (spawnedTiles.TryGetValue(tile, out int value))
            spawnedTiles[tile] = value + 1;
        else
            spawnedTiles.Add(tile, 1);
    }

    public static void Reset(int maxTiles)
    {
        spawnedTiles?.Clear();
        maxTileCount = maxTiles;
        currentTileCount = 0;
    }

    public static bool CanSpawn(TerrainTileDefinition tile)
    {
        return spawnedTiles.TryGetValue(tile, out int count) && count >= tile.maxCount;
    }

    public static int RecordTile(List<RandomTile> tiles)
    {
        ++currentTileCount;
        if (tiles == null || tiles.Count == 0)
            return -1;

        // There's still half the generation to go, it's ok
        int remainingTiles = maxTileCount - currentTileCount;
        float remainingPercent = (float)remainingTiles/maxTileCount;
        if (remainingPercent > 0.5f)
            return -1;

        // Check if tiles with minimum have been fulfilled
        for (int i = 0 ; i < tiles.Count ; ++i)
        {
            TerrainTileDefinition tile = tiles[i].tile;
            int count = spawnedTiles.ContainsKey(tile) ? spawnedTiles[tile] : 0;
            if (count < tile.minCount)
            {
                tiles[i].chance *= 2.0f;
            }
        }

        return -1;
    }
}

public class RandomPrefab : MonoBehaviour
{
    [SerializeField]
    List<RandomTile> possiblities = new List<RandomTile>();

    [SerializeField, Tooltip("What to do if no possibilities has been found.")]
    LastPickMode mode = LastPickMode.PickMostCommon;

    private void Start()
    {
        bool spawned = false;
        int forcedTileIndex = TileManager.RecordTile(possiblities);
        if (forcedTileIndex > 0)
        {
            spawned = InstantiateTile(possiblities[forcedTileIndex]);
        }

        if (!spawned)
        {
            int index = PickOne(possiblities, mode);
            if (index < possiblities.Count)
            {
                InstantiateTile(possiblities[index]);
            }
        }

        Destroy(gameObject);
    }

    private static int PickOne(List<RandomTile> prob, LastPickMode mode)
    {
        int index = 0;
        float r = Random.value;
        int startIndex = -1;
        int lastValidTile = -1;
        for (; index < prob.Count && r > 0 ; ++index)
        {
            if (TileManager.CanSpawn(prob[index].tile))
            {
                continue;
            }

            if (startIndex < 0)
                startIndex = index;

            r -= prob[index].chance;
            lastValidTile = index;
        }

        // None of the probabilities have been selected (r > sum of all chances)
        if (index >= prob.Count)
        {
            switch(mode)
            {
                case LastPickMode.Nothing:
                    return index;

                case LastPickMode.PickMostCommon:
                {
                    // Make sure not to spawn something above its max count
                    // If all the tiles have reached their max count, don't spawn anything
                    return startIndex < 0 ? index : startIndex;
                }
                
                case LastPickMode.PickLeastCommon:
                    return lastValidTile;
            }
        }

        return --index;
    }

    private bool InstantiateTile(RandomTile prob)
    {
        TileManager.RecordSpawnedTile(prob.tile);
        if (prob.tile == null || prob.tile.prefab == null)
        {
            return false;
        }

        GameObject obj = Instantiate(prob.tile.prefab, transform.position, transform.rotation, transform.parent);
        SplineInstantiate[] splines = obj.GetComponentsInChildren<SplineInstantiate>(true);
        if (splines != null)
        {
            for (int i = 0 ; i < splines.Length ; ++i)
                splines[i].Randomize();
        }

        return true;
    }

    [Button]
    private void Sort()
    {
        if (mode != LastPickMode.Nothing)
        {
            possiblities.Sort((RandomTile lhs, RandomTile rhs) =>
            {
                float difference = rhs.chance - lhs.chance;
                return difference == 0.0f ? 0 : (int)Mathf.Sign(difference);
            });
        }
    }
}