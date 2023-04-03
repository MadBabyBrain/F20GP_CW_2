using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateWalking : EnemyStateBase
{
    EnemyStateManager esm;
    bool isRunning = false;
    public override void enter(EnemyStateManager e)
    {
        Debug.Log("Entered walking state");
        Debug.Log("Health: " + e.health);

        esm = e;
    }

    public override void execute()
    {
        // runs every update so commented out
        //Debug.Log("Execute walking state");

        if (Vector3.Distance(esm.player.transform.position, esm.transform.position) <= 5) {
            esm.updateState(esm.attack);
        }

        // StartCoroutine(move());
        if (!isRunning) InvokeRepeating("move", 1f, 1f);
        isRunning = true;
    }

    // public override void updateState()
    // {
    //     Debug.Log("Changing from walking state");
    // }

    public override void exit()
    {
        Debug.Log("Exiting walking state");
    }

    void move() {
        if (esm.pathIndex == esm.path.Count - 1)
        {
            CancelInvoke();
            Destroy(esm.gameObject);

            // mem leak
            return;
        }
        // TODO: Terrain offset by 1/2 in every direction, fix in implementation
        esm.transform.position = esm.path[esm.pathIndex++];
    }
}
