using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateAttack : EnemyStateBase
{
    EnemyStateManager esm;
    public override void enter(EnemyStateManager e)
    {
        Debug.Log("Entered attacking state");
        Debug.Log("Health: " + e.health);

        esm = e;
    }

    public override void execute()
    {
        // Debug.Log("Execute attacking state");

        Debug.Log("GET FUCKED :middle_finger:");

        if (Vector3.Distance(esm.player.transform.position, esm.transform.position) > 5) {
            esm.updateState(esm.walking);
        }
    }

    // public override void updateState()
    // {
    //     Debug.Log("Changing from attacking state");
    // }

    public override void exit()
    {
        Debug.Log("Exiting attacking state");
    }
}
