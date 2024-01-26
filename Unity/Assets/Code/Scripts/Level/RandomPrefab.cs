using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Splines;

[System.Serializable]
public struct RandomTile
{
    public TerrainTileDefinition tile;
    public float chance;
}

public class RandomPrefab : MonoBehaviour
{
    [SerializeField]
    List<RandomTile> possiblities = new List<RandomTile>();

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
        int index = Random.Range(0, possiblities.Count);//PickOne(possiblities);
        if (index < possiblities.Count)
        {
            Instantiate(possiblities[index]);
        }

        Destroy(gameObject);
    }

    private static int PickOne(List<RandomTile> prob)
    {
        int index = 0;
        float r = Random.value;
        for (int i = 0 ; i < prob.Count && r > 0 ; ++i)
        {
            if (spawnedTiles.TryGetValue(prob[i].tile, out int count) && count >= prob[i].tile.maxCount)
            {
                continue;
            }

            r -= prob[i].chance;
            ++index;
        }
        index--;

        return index;
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
}