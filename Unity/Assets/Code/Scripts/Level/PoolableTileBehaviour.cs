using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Splines;

public class PoolableTileBehaviour : PoolableBehaviour
{
    [SerializeField, InfoBox("List of gao that should not be made static.")]
    private GameObject[] m_staticException;

    [SerializeField] private UnityEvent OnTileActivation;

    [Button]
    protected override void OnReset()
    {
        gameObject.isStatic = true;
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            transform.GetChild(i).gameObject.isStatic = true;
        }

        foreach (var gao in m_staticException)
        {
            gao.isStatic = false;
        }
    }

    [Button]
    protected override void OnActivation()
    {
        OnTileActivation?.Invoke();

        var splines = GetComponentsInChildren<SplineInstantiate>();
        foreach(var spline in splines)
        {
            spline.enabled = true;
            spline.Clear();
            spline.UpdateInstances();
            spline.Randomize();
            spline.SetDirty();

            spline.Randomize();
            spline.Clear();
            spline.SetDirty();
            spline.UpdateInstances();
        }

        var spawners = GetComponentsInChildren<Spawner>();
        foreach (var spawner in spawners)
        {
            spawner.InSceneParent = transform;
            spawner.Randomize();
            spawner.SpawnAndDestroySpawner();
        }
    }
}