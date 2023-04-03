using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Database that stores the enemy parameters and stats
// these stats can be applied upon Enemy GameObject creation

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/EnemyScriptableObject", order = 1)]
public class EnemyScriptableObject : ScriptableObject
{
    public string enemyName;
    public uint tier  = 1;
    public float hp = 10;
    public float speed = 1f;


    // Former stratagy to create Enemy stats
    // Does not provide ways to edit through editor
    // public readonly struct EnemyStats
    // {
    //     public EnemyStats(float _hp, float _speed)
    //     {
    //         HP = _hp;
    //         SPEED = _speed;
    //     }
    //     public float HP { get; }
    //     public float SPEED { get; }
    // };

    // // Examples of how to create new emeny parameters
    // // tiers or types of enemies?
    // public EnemyStats tier1 = new EnemyStats(100, 1);
    // public EnemyStats tier2 = new EnemyStats(120, 1.01f);

    // public EnemyStats Orc = new EnemyStats(300, 0.95f);

}
