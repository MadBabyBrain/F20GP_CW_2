using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyStateBase
{

    public abstract void enter(EnemyStateManager e);
    public abstract void execute(EnemyStateManager e);
    // public abstract void updateState(EnemyStateManager e);
    public abstract void exit(EnemyStateManager e);
}
