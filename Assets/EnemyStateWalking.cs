using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateWalking : EnemyStateBase
{
    public override void enter(EnemyStateManager e)
    {
        Debug.Log("Entered walking state");
        Debug.Log("Health: " + e.health);
    }

    public override void execute(EnemyStateManager e)
    {
        // runs every update so commented out
        //Debug.Log("Execute walking state");

        if (Vector3.Distance(e.player.transform.position, e.transform.position) <= 5) {
            e.updateState(e.attack);
        }

        // StartCoroutine(move(e));
    }

    // public override void updateState(EnemyStateManager e)
    // {
    //     Debug.Log("Changing from walking state");
    // }

    public override void exit(EnemyStateManager e)
    {
        Debug.Log("Exiting walking state");
    }

    IEnumerator move(EnemyStateManager e) {
        if (e.pathIndex < e.path.Count) {
            e.transform.position = e.path[e.pathIndex++];
        }

        yield return new WaitForSeconds(1f);
    }
}
