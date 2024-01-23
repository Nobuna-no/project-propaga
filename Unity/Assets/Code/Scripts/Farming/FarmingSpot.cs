using UnityEngine;

using NobunAtelier;
using Unity.VisualScripting;
using NobunAtelier.Gameplay;

public class FarmingSpot : MonoBehaviour
{
    [SerializeField]
    private PoolManager m_poolManager;

    [SerializeField]
    private InteractableObjectBehaviour m_interactableBehaviour;

    [SerializeField]
    ObjectDefinition m_playerCorpseDefinition;

    [SerializeField]
    private Vector3 m_offset = Vector3.zero;

    public PoolManager PoolManager => m_poolManager;

    public void Spawn(ObjectDefinition obj)
    {
        if (m_poolManager == null || obj == null || obj.plantedObject == null)
        {
            if (obj.plantedObject == m_playerCorpseDefinition)
            {

                return;
            }
            Debug.Log($"Can't spawn {obj.name}", this);
            return;
        }

        PoolableBehaviour poolBehaviour = m_poolManager.SpawnObject(obj.plantedObject, transform.position + m_offset);
        FarmingObject farmObj = poolBehaviour as FarmingObject;
        farmObj.Init(this);
        m_interactableBehaviour.gameObject.SetActive(false);
    }

    public void Spawn(InteractionInfo info)
    {
        var obj = info.ObjectDefinition;
        if (m_poolManager == null || obj == null || obj.plantedObject == null)
        {
            if (obj == m_playerCorpseDefinition)
            {
                var character = info.Target as PropagaTransportableCharacter;
                Debug.Assert(character, this);
                character.PlantCharacter(transform.position + m_offset);
                return;
            }

            Debug.Log($"Can't spawn {obj.name}", this);
            return;
        }

        PoolableBehaviour poolBehaviour = m_poolManager.SpawnObject(obj.plantedObject, transform.position + m_offset);
        FarmingObject farmObj = poolBehaviour as FarmingObject;
        farmObj.Init(this);
        m_interactableBehaviour.gameObject.SetActive(false);
    }

    public void Despawn(FarmingObject obj)
    {
        m_interactableBehaviour.gameObject.SetActive(true);
        // obj.GetComponentInChildren<HealthBehaviour>().Kill();
    }
}
