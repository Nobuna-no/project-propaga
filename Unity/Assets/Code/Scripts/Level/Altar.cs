using UnityEngine;

using NobunAtelier;

public class Altar : MonoBehaviour
{
    public void OnInteract()
    {
        (GameModeManager.Instance as PropagaGameModeManager).Victory();
    }
}
