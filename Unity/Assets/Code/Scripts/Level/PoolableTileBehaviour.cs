using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

public class PoolableTileBehaviour : PoolableBehaviour
{
    [SerializeField, InfoBox("List of gao that should not be made static.")]
    private GameObject[] m_staticException;

    [SerializeField] private UnityEvent OnTileActivation;

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

    protected override void OnActivation()
    {
        OnTileActivation?.Invoke();
    }
}