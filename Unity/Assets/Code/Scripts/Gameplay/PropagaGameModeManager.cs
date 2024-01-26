using System.Collections.Generic;

using UnityEngine;

using NobunAtelier;
using NobunAtelier.Gameplay;
using UnityEngine.Events;

public class PropagaGameModeManager : GameModeManager
{
    List<HealthBehaviour> playerHealths = new List<HealthBehaviour>();

    [SerializeField]
    private PropagaPlayerCollection playerData = null;

    public UnityEvent OnAllPlayerDead;
    public UnityEvent OnVictory;
    
    public override void GameModeStart()
    {
        playerHealths.Clear();
        base.GameModeStart();
    }

    public override bool AddPlayer(GameModeParticipant participant)
    {
        if (!base.AddPlayer(participant))
            return false;

        HealthBehaviour health = participant.GetComponentInChildren<HealthBehaviour>();
        if (health)
            playerHealths.Add(health);
        
        var player = participant as PropagaParticipant;
        var controller = participant.Controller as PropagaPlayerController;
        player.DataDefinition = playerData.GetData()[Participants.Count - 1];
        if (controller != null)
            controller.PlayerId?.Set(player.DataDefinition);

        return true;
    }

   public override bool RemovePlayer(GameModeParticipant participant, bool destroyChildrenPrefab = true)
   {
        if (!base.RemovePlayer(participant, destroyChildrenPrefab))
            return false;

        HealthBehaviour health = participant.GetComponentInChildren<HealthBehaviour>();
        if (health)
            playerHealths.Remove(health);
        return true;
   }

    private void Update()
    {
        if (playerHealths.Count == 0)
            return;

        bool oneAlive = false;
        for (int i = 0 ; i < playerHealths.Count && !oneAlive ; ++i)
        {
            if (!playerHealths[i].IsDead)
                oneAlive = true;
        }

        if (!oneAlive)
        {
            Debug.Log("All players died");
            OnAllPlayerDead?.Invoke();
        }
    }

    public void Victory()
    {
        Debug.Log("Victory !!");
        OnVictory?.Invoke();
    }
}