using UnityEngine;

public class PlayerIdBehaviour : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer[] m_renderers = null;

    public void Set(PropagaPlayerDefinition data)
    {
        if (m_renderers == null)
            return;

        for (int i = 0 ; i < m_renderers.Length ; ++i)
        {
            m_renderers[i].material = data.PlayerMaterial;
        }
    }
}