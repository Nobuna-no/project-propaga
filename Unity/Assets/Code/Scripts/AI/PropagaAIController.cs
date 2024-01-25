using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropagaAIController : NobunAtelier.AIController
{
    private void Update()
    {
        UpdateController(Time.deltaTime);
    }
}
