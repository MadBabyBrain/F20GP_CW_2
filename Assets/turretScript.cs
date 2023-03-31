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

    private float damage = 5f;
    private float cooldown = 1f;
    private float timeSinceLastShot = 0;



    void getEnemiesInRange(out List<EnemyStateManager> container)
    {
        container = new List<EnemyStateManager>();
        Collider[] hitColliders = Physics.OverlapSphere(this.gameObject.transform.position, distance, LayerMask.NameToLayer("Enemy"));
        foreach (Collider hitCollider in hitColliders)
        {
            container.Add(hitCollider.GetComponent<EnemyStateManager>());
        }
    }

    void getTarget(out EnemyStateManager target) {
        target = null;
        
        // no targets in range
        if (enemies.Count < 1) {
            return;
        }

        // locate enemy with the shortest path length left
        target = enemies[0];
        foreach(EnemyStateManager enemy in enemies) {
            if (enemy.pathIndex > target.pathIndex) {
                target = enemy;
            }
        }
    }

    // coroutine? probably would work better
    void shoot(EnemyStateManager target) {
        target.takeDamage(damage);
        timeSinceLastShot = Time.time;
        Debug.Log("shot");
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
        getEnemiesInRange(out enemies);

        // target <- enemy in enemies closest to base
        getTarget(out target);

        // shoot(target) // update target via state manager
        if (target != null) {
            tHead.LookAt(target.transform);
            // start coroutine?
            if(Time.time - timeSinceLastShot > cooldown){
                shoot(target);
            }
                
        }

        // if target > range or target == dead
        //      target <- enemy in enemies closest to base
        // e.g.: start loop all over again to get next enemy or
        // wait until an enemy in range


    }
}