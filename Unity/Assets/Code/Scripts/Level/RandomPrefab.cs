using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Splines;

[System.Serializable]
public struct RandomTile
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

public class RandomPrefab : MonoBehaviour
{
    [SerializeField]
    List<RandomTile> possiblities = new List<RandomTile>();

    [SerializeField, Tooltip("What to do if no possibilities has been found.")]
    LastPickMode mode = LastPickMode.PickMostCommon;

    private static Dictionary<TerrainTileDefinition, int> spawnedTiles;

    [RuntimeInitializeOnLoadMethod()]
    private static void Initialize()
    {
        spawnedTiles = new Dictionary<TerrainTileDefinition, int>();
    }

    public static void Reset()
    {
        spawnedTiles?.Clear();
    }

    private void Start()
    {
        int index = PickOne(possiblities, mode);
        if (index < possiblities.Count)
        {
            Instantiate(possiblities[index]);
        }

        Destroy(gameObject);
    }

    private static int PickOne(List<RandomTile> prob, LastPickMode mode)
    {
        int index = 0;
        float r = Random.value;
        int firstValidTile = -1;
        int lastValidTile = -1;
        for (; index < prob.Count && r > 0 ; ++index)
        {
            if (spawnedTiles.TryGetValue(prob[index].tile, out int count) && count >= prob[index].tile.maxCount)
            {
                continue;
            }

            if (firstValidTile < 0)
                firstValidTile = index;

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
                    return firstValidTile < 0 ? index : firstValidTile;
                }
                
                case LastPickMode.PickLeastCommon:
                    return lastValidTile;
            }
        }

        return --index;
    }

    private void Instantiate(RandomTile prob)
    {
        if (prob.tile == null || prob.tile.prefab == null)
        {
            return;
        }

        GameObject obj = Instantiate(prob.tile.prefab, transform.position, transform.rotation, transform.parent);
        SplineInstantiate[] splines = obj.GetComponentsInChildren<SplineInstantiate>(true);
        if (splines != null)
        {
            for (int i = 0 ; i < splines.Length ; ++i)
                splines[i].Randomize();
        }

        if (spawnedTiles.TryGetValue(prob.tile, out int value))
            spawnedTiles[prob.tile] = value + 1;
        else
            spawnedTiles.Add(prob.tile, 1);
    }

    private void OnValidate()
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