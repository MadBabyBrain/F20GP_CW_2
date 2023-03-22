using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateAttack : EnemyStateBase
{
    public override void enter(EnemyStateManager e)
    {
        Debug.Log("Entered attacking state");
        Debug.Log("Health: " + e.health);
    }

    public override void execute(EnemyStateManager e)
    {
        // Debug.Log("Execute attacking state");
        
        if (Vector3.Distance(e.player.transform.position, e.transform.position) > 5) {
            e.updateState(e.walking);
        }
    }

    // public override void updateState(EnemyStateManager e)
    // {
    //     Debug.Log("Changing from attacking state");
    // }

    public override void exit(EnemyStateManager e)
    {
        Debug.Log("Exiting attacking state");
    }
}
