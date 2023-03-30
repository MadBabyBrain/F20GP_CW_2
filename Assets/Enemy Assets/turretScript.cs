using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class turretScript : MonoBehaviour
{

    List<EnemyStateManager> enemies;
    EnemyStateManager target;
    float distance;
    public float maxDist;
    public Transform tHead;

    void getEnemiesInRange(out List<EnemyStateManager> container)
    {
        container = new List<EnemyStateManager>();
        Collider[] hitColliders = Physics.OverlapSphere(this.gameObject.transform.position, distance, LayerMask.NameToLayer("Enemy"));
        foreach (var hitCollider in hitColliders)
        {
            container.Add(hitCollider);
        }
    }

    void Update()
    {
        // works only on one enemy
        // distance = Vector3.Distance(enemy.position, transform.position);
        // if (distance <= maxDist)
        // {
        //     tHead.LookAt(enemy);
        // }

        // enemies <- enemies within range // overlap sphere
        // target <- enemy in enemies closest to base
        // shoot(target) // update target via state manager
        // if target > range or target == dead
        //      target <- enemy in enemies closest to base


    }
}
