using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/EnemyScriptableObject", order = 2)]
public class TurretScriptableObject : ScriptableObject
{
    public Transform towerHeadTransform;
    public float radius;
    public float attackInterval;
    public float damage;
}
