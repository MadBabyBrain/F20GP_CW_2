using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyStateBase : MonoBehaviour
{

    public abstract void enter(EnemyStateManager e);
    public abstract void execute();
    // public abstract void updateState();
    public abstract void exit();
}
