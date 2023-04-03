using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class Turret : MonoBehaviour
{

    public void _init_(TurretScriptableObject _stats)
    {
        stats = _stats;

        tHead = stats.towerHeadTransform;
        distance = stats.radius;
        attackInterval = stats.attackInterval;
        damage = stats.damage;

        initialised = true;
        this.shooting = false;
    }

    void enemiesInRange(out List<Enemy> container)
    {
        container = new List<Enemy>();
        Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, distance, LayerMask.GetMask("Enemies"));
        foreach (Collider hitCollider in hitColliders)
        {
            container.Add(hitCollider.GetComponent<Enemy>());
        }
    }

    void targetEnemy(out Enemy target)
    {
        target = null;

        // no targets in range
        if (enemies.Count < 1)
        {
            return;
        }

        // locate enemy with the shortest path length left
        float pos = -1;

        foreach (Enemy enemy in enemies)
        {
            if (enemy.getpos() >= pos)
            {
                pos = enemy.getpos();
                target = enemy;
            }
        }
    }

    // coroutine? probably would work better
    IEnumerator shoot(Enemy target)
    {
        // instantiate new bullet or something??
        this.shooting = true;
        target.takeDamage(damage);

        yield return new WaitForSeconds(attackInterval);
        this.shooting = false;
    }

    void Update()
    {
        if (!shooting)
        {
            // enemies <- enemies within range // overlap sphere
            enemiesInRange(out enemies);

            // target <- enemy in enemies closest to base
            targetEnemy(out target);

            // shoot(target)
            if (target != null)
            {
                // tHead.LookAt(target.transform);
                tHead.LookAt(new Vector3(target.transform.position.x, tHead.transform.position.y, target.transform.position.z), Vector3.up);
                if (!shooting) StartCoroutine(shoot(target));
            }

            // if target > range or target == dead
            //      target <- enemy in enemies closest to base
            // e.g.: start loop all over again to get next enemy or
            // wait until an enemy in range

        }

    }

    // private members
    public List<Enemy> enemies;
    public Enemy target;
    public Transform tHead;
    public TurretScriptableObject stats;
    public float distance;
    public float attackInterval;
    public float damage;
    public bool initialised = false;   // used to determine if _init_() has been called and member fields have been set
    public bool shooting;

}
