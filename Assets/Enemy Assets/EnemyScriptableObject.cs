using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/EnemyScriptableObject", order = 1)]
public class EnemyScriptableObject : ScriptableObject
{
    public readonly struct EnemyStats
    {
        public EnemyStats(float _hp, float _speed)
        {
            HP = _hp;
            SPEED = _speed;
        }
        public float HP { get; }
        public float SPEED { get; }
    };

    public EnemyStats tier1 = new EnemyStats(100, 1);
    public EnemyStats tier2 = new EnemyStats(120, 1.01f);
}
