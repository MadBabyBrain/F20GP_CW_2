using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class turretScript : MonoBehaviour
{

    void _init_(ref TurretScriptableObject _stats) {
        stats = _stats;

        tHead = stats.towerHeadTransform;
        distance = stats.radius;
        attackInterval = stats.attackInterval;
        damage = stats.damage;

        initialised = true;
    }

    void enemiesInRange(out List<Enemy> container)
    {
        container = new List<Enemy>();
        Collider[] hitColliders = Physics.OverlapSphere(this.gameObject.transform.position, distance, LayerMask.NameToLayer("Enemy"));
        foreach (Collider hitCollider in hitColliders)
        {
            container.Add(hitCollider.GetComponent<Enemy>());
        }
    }

    void targetEnemy(out Enemy target) {
        target = null;
        
        // no targets in range
        if (enemies.Count < 1) {
            return;
        }

        // locate enemy with the shortest path length left
        int distanceLeft = int.MaxValue;

        foreach(Enemy enemy in enemies) {
            if (enemy.distanceLeftToTravel() < distanceLeft) {
                target = enemy;
            }
        }
    }

    // coroutine? probably would work better
    IEnumerator shoot(Enemy target) {
        // instantiate new bullet or something??

        target.takeDamage(damage);

        yield return new WaitForSeconds(attackInterval);
    }

    void Update()
    {
        // enemies <- enemies within range // overlap sphere
        enemiesInRange(out enemies);

        // target <- enemy in enemies closest to base
        targetEnemy(out target);

        // shoot(target)
        if (target != null) {
            tHead.LookAt(target.transform);
            StartCoroutine(shoot(target));
        }

        // if target > range or target == dead
        //      target <- enemy in enemies closest to base
        // e.g.: start loop all over again to get next enemy or
        // wait until an enemy in range


    }

    // private members
    private List<Enemy> enemies;
    private Enemy target;
    private Transform tHead;
    private TurretScriptableObject stats;
    private float distance;
    private float attackInterval;
    private float damage;
    private bool initialised = false;   // used to determine if _init_() has been called and member fields have been set

}
